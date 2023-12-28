#if TOOLS
using Godot;

namespace AmountInspector;

[Tool]
public partial class Plugin : EditorPlugin, ISerializationListener
{
    InspectorPlugin _inspectorPlugin;

    public override void _EnterTree()
    {
        _inspectorPlugin = new InspectorPlugin(GetEditorInterface());
        AddInspectorPlugin(_inspectorPlugin);
    }

    public override void _ExitTree()
    {
        RemoveInspectorPlugin(_inspectorPlugin);
        _inspectorPlugin = null;
    }

    public void OnAfterDeserialize() => _EnterTree();

    public void OnBeforeSerialize() => _ExitTree();
}
#endif
