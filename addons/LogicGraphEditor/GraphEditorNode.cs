#if TOOLS
#nullable enable
using System;
using Godot;

namespace LogicGraphs.Editor;

[Tool]
public partial class GraphEditorNode : GraphNode
{
    const string SlotScenePath = "res://addons/LogicGraphEditor/graph_editor_node_slot.tscn";

    LogicNodeDef _def;

    LogicNode _logicNode;
    public LogicNode LogicNode => _logicNode;

#pragma warning disable CS8618
    GraphEditorNode() { }
#pragma warning restore CS8618

    public GraphEditorNode(LogicNode logicNode)
    {
        _def = LogicNodeDef.GetDef(logicNode.GetType());
        _logicNode = logicNode;

        Title = _def.Name;
        PositionOffset = _logicNode.GraphEditorPosition;

        AddSlots();

        PositionOffsetChanged += OnPositionChanged;
        RaiseRequest += OnFocused;
    }

    public GraphEditorNodeSlot GetSlot(int index) => GetChild<GraphEditorNodeSlot>(index);

    public GraphEditorNodeSlot GetSlotOfInputPort(int index) => GetSlot(GetInputPortSlot(index));

    public GraphEditorNodeSlot GetSlotOfOutputPort(int index) => GetSlot(GetOutputPortSlot(index));

    int FirstSlotIndex(Func<GraphEditorNodeSlot, bool> matcher)
    {
        for (int index = 0; index < GetChildCount(); index++)
        {
            if (matcher(GetSlot(index)))
                return index;
        }
        throw new ArgumentException("No matching slot found");
    }

    public int GetPortOfMethod(StringName method) => FirstSlotIndex(slot => slot.Method == method);

    public int GetPortOfSignal(StringName signal) => FirstSlotIndex(slot => slot.Signal == signal);

    void AddSlots()
    {
        for (int index = 0; index < _def.Methods.Length || index < _def.Signals.Length; index++)
        {
            var slot = GD.Load<PackedScene>(SlotScenePath).Instantiate<GraphEditorNodeSlot>();
            if (index < _def.Methods.Length)
            {
                slot.SetMethod(_def.Methods[index]);
                SetSlotEnabledLeft(index, true);
            }
            if (index < _def.Signals.Length)
            {
                slot.SetSignal(_def.Signals[index]);
                SetSlotEnabledRight(index, true);
            }
            AddChild(slot);
        }
    }

    void OnPositionChanged()
    {
        _logicNode.GraphEditorPosition = PositionOffset;
        _logicNode.EmitChanged();
    }

    void OnFocused()
    {
        EditorInterface.Singleton.EditResource(_logicNode);
    }
}
#endif
