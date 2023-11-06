using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class Worker : Node
{
    IWork _work;
    TaskCompletionSource _taskCompletionSource;
    CancellationToken _cancellationToken;

    public Task WorkOn(IWork work, CancellationToken ct)
    {
        Debug.Assert(_work is null);

        _work = work;
        _taskCompletionSource = new();
        _cancellationToken = ct;

        return _taskCompletionSource.Task;
    }

    public override void _Process(double deltaTime)
    {
        if (_work is null)
            return;

        if (_cancellationToken.IsCancellationRequested)
        {
            _taskCompletionSource.SetCanceled();
            Reset();
            return;
        }

        var finished = _work.Work(deltaTime);
        if (finished)
        {
            _taskCompletionSource.SetResult();
            Reset();
        }
    }

    void Reset()
    {
        _work = null;
        _taskCompletionSource = null;
        _cancellationToken = default;
    }
}
