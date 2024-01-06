using System;
using Godot;

[GlobalClass]
public partial class FurnitureConstructor : Node
{
    [Export]
    GridPosition _gridPosition;

    [Export]
    Destructor _destructor;

    [Export]
    Canceler _canceler;

    [Export]
    StructureConstructor _structureConstructor;

    Action _cleanup;

    public override void _Ready()
    {
        var floorGrid = Owner.GetNode<EntityGrids>("../%EntityGrids").FloorGrid;
        var floor = floorGrid.Get(_gridPosition.Coord);

        floor.GetNode<Destructor>("Destructor").OnDestruction += Destroy;

        var floorConstructor = floor.GetNodeOrNull<StructureConstructor>("StructureConstructor");
        if (floorConstructor is not null)
        {
            floorConstructor.OnConstructionCompleted += Construct;
            _cleanup += () => floorConstructor.OnConstructionCompleted -= Construct;
            _canceler.OnCancel += Destroy;
            _cleanup += () => _canceler.OnCancel -= Destroy;
        }
        else
            Construct();
    }

    void Construct()
    {
        _cleanup?.Invoke();
        _ = _structureConstructor.Construct();
    }

    void Destroy()
    {
        _cleanup?.Invoke();
        _canceler.Cancel();
        _destructor.Destroy();
    }
}
