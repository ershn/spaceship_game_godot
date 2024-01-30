#if TOOLS
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace LogicGraphs.Editor;

[Tool]
public partial class GraphEditorNode : GraphNode
{
    const string SlotScenePath = "res://addons/LogicGraphEditor/graph_editor_node_slot.tscn";

    [Signal]
    public delegate void OutputPortAddedEventHandler(GraphEditorNode node);

    [Signal]
    public delegate void OutputPortRemovedEventHandler(GraphEditorNode node);

    LogicNodeDef _def;

    LogicNode _logicNode;
    public LogicNode LogicNode => _logicNode;

    Godot.Collections.Array<bool> _connectedOutputPorts = [];

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

    public bool HasDynamicOutputs => _def.HasDynamicOutputs;

    public IEnumerable<(int nodeIndex, string? method)> GetDynamicOutputs()
    {
        if (_def.HasDynamicOutputNodes)
            return GetDynamicOutputNodes();
        else if (_def.HasDynamicOutputMethods)
            return GetDynamicOutputMethods();
        else
            throw new ArgumentException($"This graph node doesn't have dynamic outputs");
    }

    IEnumerable<(int nodeIndex, string? method)> GetDynamicOutputNodes() =>
        _def.GetOutputNodeIndexes(_logicNode).Select(nodeIndex => (nodeIndex, (string?)null));

    IEnumerable<(int nodeIndex, string? method)> GetDynamicOutputMethods() =>
        _def.GetOutputMethodReferences(_logicNode)
            .Select(logicEndpoint => (logicEndpoint.NodeIndex, (string?)logicEndpoint.Member));

    public void SetDynamicOutputs(IEnumerable<(int nodeIndex, string? method)> outputs)
    {
        if (_def.HasDynamicOutputNodes)
            SetDynamicOutputNodes(outputs);
        else if (_def.HasDynamicOutputMethods)
            SetDynamicOutputMethods(outputs);
        else
            throw new ArgumentException($"This graph node doesn't have dynamic outputs");
    }

    void SetDynamicOutputNodes(IEnumerable<(int nodeIndex, string? method)> outputs)
    {
        var indexes = outputs.Select(output => output.nodeIndex);
        _def.SetOutputNodeIndexes(_logicNode, indexes.ToArray());
    }

    void SetDynamicOutputMethods(IEnumerable<(int nodeIndex, string? method)> outputs)
    {
        var references = outputs.Select(output => new LogicEndpoint()
        {
            NodeIndex = output.nodeIndex,
            Member = output.method
        });
        _def.SetOutputMethodReferences(_logicNode, references.ToArray());
    }

    int GetSlotCount() => GetChildCount();

    GraphEditorNodeSlot GetSlot(int index) => GetChild<GraphEditorNodeSlot>(index);

    int FirstSlotIndex(Func<GraphEditorNodeSlot, bool> matcher)
    {
        for (int index = 0; index < GetSlotCount(); index++)
        {
            if (matcher(GetSlot(index)))
                return index;
        }
        throw new ArgumentException("No matching slot found");
    }

    public string? GetMethodOfPort(int index) => GetSlot(GetInputPortSlot(index)).Method;

    public string? GetSignalOfPort(int index) => GetSlot(GetOutputPortSlot(index)).Signal;

    public int GetPortOfMethod(string method) => FirstSlotIndex(slot => slot.Method == method);

    public int GetPortOfSignal(string signal) => FirstSlotIndex(slot => slot.Signal == signal);

    GraphEditorNodeSlot InstantiateSlot() =>
        GD.Load<PackedScene>(SlotScenePath).Instantiate<GraphEditorNodeSlot>();

    void AddSlots()
    {
        var inputNames = GetInputNames();
        var outputNames = GetOutputNames();
        for (int index = 0; index < inputNames.Length || index < outputNames.Length; index++)
        {
            var slot = InstantiateSlot();
            if (index < inputNames.Length)
            {
                if (inputNames[index] is string inputName)
                    slot.SetMethod(inputName);
                SetSlotEnabledLeft(index, true);
            }
            if (index < outputNames.Length)
            {
                if (outputNames[index] is string outputName)
                    slot.SetSignal(outputName);
                SetSlotEnabledRight(index, true);
                _connectedOutputPorts.Add(false);
            }
            AddChild(slot);
        }
    }

    string?[] GetInputNames() => _def.IsInputNode ? new string?[1] : _def.Methods;

    string?[] GetOutputNames()
    {
        if (_def.HasDynamicOutputs)
            return new string?[GetInitialDynamicOutputPortCount()];
        else
            return _def.Signals;
    }

    int GetInitialDynamicOutputPortCount() => GetDynamicOutputs().Count() + 1;

    void AddOutputPort()
    {
        if (GetOutputPortCount() == GetSlotCount())
            AddChild(InstantiateSlot());
        SetSlotEnabledRight(GetOutputPortCount(), true);
        _connectedOutputPorts.Add(false);
        EmitSignal(SignalName.OutputPortAdded, this);
    }

    void RemoveOutputPort()
    {
        SetSlotEnabledRight(GetOutputPortCount() - 1, false);
        var lastSlotIndex = GetSlotCount() - 1;
        if (!IsSlotEnabledLeft(lastSlotIndex) && !IsSlotEnabledRight(lastSlotIndex))
        {
            GetSlot(lastSlotIndex).Free();
            Size = Vector2.Zero; // Reset the Container's size
        }
        _connectedOutputPorts.RemoveAt(_connectedOutputPorts.Count - 1);
        EmitSignal(SignalName.OutputPortRemoved, this);
    }

    public void OnOutputPortConnected(int portIndex)
    {
        _connectedOutputPorts[portIndex] = true;
        if (HasDynamicOutputs)
        {
            if (_connectedOutputPorts.All(connected => connected))
                AddOutputPort();
        }
    }

    public void OnOutputPortDisconnected(int portIndex)
    {
        _connectedOutputPorts[portIndex] = false;
        if (HasDynamicOutputs)
        {
            while (
                !_connectedOutputPorts[^1]
                && _connectedOutputPorts.Count > 1
                && !_connectedOutputPorts[^2]
            )
                RemoveOutputPort();
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
