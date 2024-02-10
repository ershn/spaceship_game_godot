using System.Threading;
using System.Threading.Tasks;
using Godot;

public class WorkOnJob : IJob
{
    readonly IWork _work;

    public WorkOnJob(IWork work)
    {
        _work = work;
    }

    public bool Retriable { get; init; }

    public async Task Execute(PhysicsBody2D executor, CancellationToken ct)
    {
        var mover = executor.GetNode<Mover>("Mover");
        await mover.MoveTo(_work.GlobalPosition, ct);

        var worker = executor.GetNode<Worker>("Worker");
        await worker.WorkOn(_work, ct);
    }

    public override string ToString() => $"[{nameof(WorkOnJob)}: at {_work.GlobalPosition}]";
}
