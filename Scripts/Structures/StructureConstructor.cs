using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class StructureConstructor : Node
{
    [Signal]
    public delegate void OnConstructionCompletedEventHandler();

    [Signal]
    public delegate void OnConstructionCanceledEventHandler();

    [Export]
    Destructor _destructor;

    [Export]
    Canceler _canceler;

    [Export]
    StructureComponentInventory _componentInventory;

    [Export]
    ConstructionWork _constructionWork;

    public async Task Construct()
    {
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        _canceler.OnCancel += cts.Cancel;
        try
        {
            await RequestComponents(ct);
            await RequestConstruction(ct);
            EmitSignal(SignalName.OnConstructionCompleted);
        }
        catch (TaskCanceledException)
        {
            _destructor.Destroy();
            EmitSignal(SignalName.OnConstructionCanceled);
            throw;
        }
        finally
        {
            _canceler.OnCancel -= cts.Cancel;
        }
    }

    async Task RequestComponents(CancellationToken ct)
    {
        if (_componentInventory.Full)
            return;

        var itemAllotter = Owner.GetNode<ItemAllotter>("../%ItemAllotter");
        var requests = new List<Task>();
        foreach (var (itemDef, missingAmount) in _componentInventory.UnfilledSlots())
        {
            var request = itemAllotter.Request(itemDef, missingAmount, _componentInventory, ct);
            requests.Add(request);
        }

        await Task.WhenAll(requests);
    }

    async Task RequestConstruction(CancellationToken ct)
    {
        var jobScheduler = Owner.GetNode<JobScheduler>("../%JobScheduler");

        var job = new WorkOnJob(_constructionWork) { Retriable = true };
        await jobScheduler.Execute(job, ct);

        QueueFree();
        _constructionWork.QueueFree();
    }
}
