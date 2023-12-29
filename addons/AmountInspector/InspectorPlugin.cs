#if TOOLS
using Godot;

namespace AmountInspector;

public partial class InspectorPlugin : EditorInspectorPlugin
{
    public override bool _CanHandle(GodotObject @object) => true;

    public override bool _ParseProperty(
        GodotObject @object,
        Variant.Type type,
        string name,
        PropertyHint hint,
        string hintString,
        PropertyUsageFlags usageFlags,
        bool wide
    )
    {
        if (!(type == Variant.Type.Int && hint == PropertyHint.None && Hint.IsValid(hintString)))
            return false;

        var hintType = Hint.Parse(hintString, out var hintParam);
        var property = new Property(hintType, hintParam);
        AddPropertyEditor(name, property);
        return true;
    }
}
#endif
