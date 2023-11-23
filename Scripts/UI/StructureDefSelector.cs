using Godot;

[GlobalClass]
public partial class StructureDefSelector : TextureButton
{
    [Signal]
    public delegate void SelectedEventHandler(StructureDef structureDef);

    [Export]
    StructureDef _structureDef;

    void OnPressed()
    {
        EmitSignal(SignalName.Selected, _structureDef);
    }
}
