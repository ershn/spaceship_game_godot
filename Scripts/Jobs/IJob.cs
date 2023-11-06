using System.Threading;
using System.Threading.Tasks;
using Godot;

public interface IJob
{
    Task Execute(PhysicsBody2D executor, CancellationToken ct);
}
