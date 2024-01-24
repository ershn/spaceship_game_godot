#if TOOLS
#nullable enable
using Godot;

namespace LogicGraphs.Editor;

[Tool]
public partial class GraphEditorNodeSlot : HBoxContainer
{
    [Export]
    Label _leftLabel = null!;

    [Export]
    Label _rightLabel = null!;

    StringName? _method;
    public StringName? Method => _method;

    public void SetMethod(StringName method)
    {
        _method = method;
        _leftLabel.Text = method;
        _leftLabel.Show();
    }

    StringName? _signal;
    public StringName? Signal => _signal;

    public void SetSignal(StringName signal)
    {
        _signal = signal;
        _rightLabel.Text = signal;
        _rightLabel.Show();
    }
}
#endif
