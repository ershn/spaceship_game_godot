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

    string _method = string.Empty;
    public string? Method => _method.Length > 0 ? _method : null;

    public void SetMethod(string method)
    {
        _method = method;
        _leftLabel.Text = method;
        _leftLabel.Show();
    }

    string _signal = string.Empty;
    public string? Signal => _signal.Length > 0 ? _signal : null;

    public void SetSignal(string signal)
    {
        _signal = signal;
        _rightLabel.Text = signal;
        _rightLabel.Show();
    }
}
#endif
