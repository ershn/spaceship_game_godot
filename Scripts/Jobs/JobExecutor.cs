using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class JobExecutor : Node
{
    JobScheduler _jobScheduler;

    [Export]
    Death _death;

    readonly CancellationTokenSource _taskCanceller = new();
    bool _isExecuting;

    public override void _Ready()
    {
        _jobScheduler = Owner.GetNode<JobScheduler>("../%JobScheduler");
        _jobScheduler.AddExecutor(this);

        _death.OnDeath += OnDeath;
    }

    public override void _Notification(int what)
    {
        switch ((long)what)
        {
            case NotificationEnabled:
                _jobScheduler.AddExecutor(this);
                break;
            case NotificationDisabled:
                _jobScheduler.RemoveExecutor(this);
                break;
            case NotificationPredelete:
                _taskCanceller.Dispose();
                break;
        }
    }

    void OnDeath()
    {
        ProcessMode = ProcessModeEnum.Disabled;
        _taskCanceller.Cancel();
    }

    public async Task Execute(IJob job, CancellationToken ct)
    {
        Debug.Assert(CanProcess());
        Debug.Assert(!_isExecuting);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(_taskCanceller.Token, ct);

        _isExecuting = true;
        try
        {
            await job.Execute(GetOwner<PhysicsBody2D>(), cts.Token);
        }
        finally
        {
            _isExecuting = false;
        }
    }
}
