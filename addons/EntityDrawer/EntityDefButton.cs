#if TOOLS
using Godot;

namespace EntityDrawer;

[Tool, GlobalClass]
public partial class EntityDefButton : TextureButton
{
    [Signal]
    public delegate void SelectedEventHandler(EntityDef entityDef);

    Control _selectedOverlay;

    EntityDef _entityDef;
    public EntityDef EntityDef
    {
        get => _entityDef;
        set
        {
            _entityDef = value;

            TextureNormal = _entityDef?.PreviewSprite;
            TooltipText = _entityDef?.ResourceName;
        }
    }

    public override void _Ready()
    {
        _selectedOverlay = GetNode<Control>("SelectedOverlay");

        Toggled += OnToggled;
    }

    void OnToggled(bool pressed)
    {
        _selectedOverlay.Visible = pressed;
        if (pressed)
            EmitSignal(SignalName.Selected, EntityDef);
    }
}
#endif
