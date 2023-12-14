using Godot;

[GlobalClass]
public partial class StructureNavCellController : NavCellController
{
    StructureDef _structureDef;

    bool _originalTraversability;

    public override void _Ready()
    {
        base._Ready();
        _structureDef = GetNode<StructureDefHolder>("../DefHolder").StructureDef;
    }

    void InitializeCell()
    {
        _originalTraversability = Traversable;
        Traversable = _structureDef.Traversable;
    }

    void CleanupCell()
    {
        Traversable = _originalTraversability;
    }
}
