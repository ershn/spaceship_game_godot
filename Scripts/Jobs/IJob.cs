using System.Threading;
using System.Threading.Tasks;
using Godot;

public interface IJob
{
    bool Retriable => false;

    Task Execute(PhysicsBody2D executor, CancellationToken ct);
}
