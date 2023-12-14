using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class ItemAllotter : Node
{
    class ItemRequest : IDisposable
    {
        public event Action OnCompleted;
        public event Action OnAllotmentCanceled;

        readonly ItemDef _itemDef;

        readonly ulong _requestedAmount;
        ulong _deliveredAmount;
        ulong _allottedAmount;

        readonly IInventoryAdd _inventory;
        readonly Action<ulong> _onAmountDelivered;

        readonly JobScheduler _jobScheduler;
        readonly TaskCompletionSource _taskCompletionSource;
        readonly CancellationToken _cancellationToken;

        readonly CancellationTokenRegistration _cancellationRegistration;

        public ItemRequest(
            ItemDef itemDef,
            ulong amount,
            IInventoryAdd inventory,
            Action<ulong> onAmountDelivered,
            JobScheduler jobScheduler,
            TaskCompletionSource taskCompletionSource,
            CancellationToken cancellationToken
        )
        {
            _itemDef = itemDef;
            _requestedAmount = amount;
            _inventory = inventory;
            _onAmountDelivered = onAmountDelivered;

            _jobScheduler = jobScheduler;
            _taskCompletionSource = taskCompletionSource;
            _cancellationToken = cancellationToken;

            _cancellationRegistration = _cancellationToken.Register(() =>
            {
                _taskCompletionSource.SetCanceled();
                OnCompleted();
            });
        }

        public void Dispose()
        {
            _cancellationRegistration.Dispose();
        }

        public ItemDef ItemDef => _itemDef;

        public ulong UnallottedAmount => _requestedAmount - _allottedAmount;

        public async Task Allot(ItemAmount item, ulong amount)
        {
            _allottedAmount += amount;
            try
            {
                using var job = new DeliverItemJob(item, amount, _inventory);
                await _jobScheduler.Execute(job, _cancellationToken);

                _deliveredAmount += amount;
                _onAmountDelivered?.Invoke(amount);

                if (_deliveredAmount == _requestedAmount)
                {
                    _taskCompletionSource.SetResult();
                    OnCompleted();
                }
            }
            catch (TaskCanceledException)
            {
                if (!_cancellationToken.IsCancellationRequested)
                {
                    _allottedAmount -= amount;
                    OnAllotmentCanceled();
                }
                throw;
            }
        }
    }

    [Export]
    EntityGrids _entityGrids;

    [Export]
    JobScheduler _jobScheduler;

    [Export]
    ItemCreator _itemCreator;

    readonly Dictionary<ItemDef, LinkedList<ItemRequest>> _itemRequests = new();

    public override void _Ready()
    {
        _itemCreator.OnItemCreated += AllotNewItem;
    }

    ItemGrid ItemGrid => _entityGrids.ItemGrid;

    public Task Request(
        ItemDef itemDef,
        ulong amount,
        IInventoryAdd inventory,
        CancellationToken ct,
        Action<ulong> onAmountDelivered = null
    )
    {
        var tcs = new TaskCompletionSource();

        var request = new ItemRequest(
            itemDef,
            amount,
            inventory,
            onAmountDelivered,
            _jobScheduler,
            tcs,
            ct
        );

        Register(request);
        AllotExistingItems(request);

        return tcs.Task;
    }

    void Register(ItemRequest request)
    {
        if (!_itemRequests.TryGetValue(request.ItemDef, out var requests))
        {
            requests = new();
            _itemRequests[request.ItemDef] = requests;
        }

        requests.AddLast(request);

        request.OnCompleted += () =>
        {
            requests.Remove(request);
            request.Dispose();
        };
        request.OnAllotmentCanceled += () =>
        {
            AllotExistingItems(request);
        };
    }

    void AllotExistingItems(ItemRequest request)
    {
        var items = ItemGrid.Filter(request.ItemDef).CumulateAmount(request.UnallottedAmount);
        foreach (var (item, markedAmount) in items)
            _ = request.Allot(item, markedAmount);
    }

    void AllotNewItem(ItemAmount item)
    {
        if (!_itemRequests.TryGetValue(item.Def, out var requests))
            return;

        foreach (var request in requests.Where(request => request.UnallottedAmount > 0))
        {
            var markedAmount = Math.Min(item.Amount, request.UnallottedAmount);
            _ = request.Allot(item, markedAmount);
            if (item.Amount == 0)
                break;
        }
    }
}
