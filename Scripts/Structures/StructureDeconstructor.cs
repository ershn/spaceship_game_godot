using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class StructureDeconstructor : Node
{
    JobScheduler _jobScheduler;

    [Export]
    Destructor _destructor;

    [Export]
    Canceler _canceler;

    [Export]
    DeconstructionWork _deconstructionWork;

    bool _allowed;
    bool _started;

    public override void _Ready()
    {
        _jobScheduler = Owner.GetNode<JobScheduler>("../%JobScheduler");
    }

    public void Allow()
    {
        _allowed = true;
    }

    public async Task Deconstruct()
    {
        if (!_allowed || _started)
            return;

        using var cts = new CancellationTokenSource();
        var ct = cts.Token;

        _canceler.OnCancel += cts.Cancel;
        try
        {
            _started = true;
            var job = new WorkOnJob(_deconstructionWork);
            await _jobScheduler.Execute(job, ct);
            _destructor.Destroy();
        }
        catch (TaskCanceledException)
        {
            _started = false;
            _deconstructionWork.Reset();
            throw;
        }
        finally
        {
            _canceler.OnCancel -= cts.Cancel;
        }
    }
}
