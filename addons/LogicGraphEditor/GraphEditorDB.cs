#if TOOLS
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GDC = Godot.Collections;

namespace LogicGraphs.Editor;

public partial class GraphEditorDB : GodotObject
{
    LogicGraph _logicGraph;
    GDC.Dictionary<LogicNode, GraphEditorNode> _editorNodes = [];
    GDC.Dictionary<GraphEditorNode, GDC.Array<ConnectionEndpoint?>> _outgoingConnections = [];
    GDC.Dictionary<GraphEditorNode, GDC.Array<ConnectionEndpoint?>> _incomingConnections = [];

    public bool Saved { get; private set; }

#pragma warning disable CS8618
    GraphEditorDB() { }
#pragma warning restore CS8618

    public GraphEditorDB(LogicGraph logicGraph)
    {
        _logicGraph = logicGraph;
        Load();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationPredelete)
            OnFree();
    }

    void OnFree()
    {
        _editorNodes.Clear();
        FreeConnections(_outgoingConnections);
        FreeConnections(_incomingConnections);

        static void FreeConnections(
            GDC.Dictionary<GraphEditorNode, GDC.Array<ConnectionEndpoint?>> connections
        )
        {
            foreach (var endpoints in connections.Values)
            {
                foreach (var endpoint in endpoints)
                    endpoint?.Free();
                endpoints.Clear();
            }
            connections.Clear();
        }
    }

    public IEnumerable<GraphEditorNode> GetNodes() => _editorNodes.Values;

    public GraphEditorNode GetNode(LogicNode logicNode) => _editorNodes[logicNode];

    public void AddNode(GraphEditorNode editorNode)
    {
        _editorNodes.Add(editorNode.LogicNode, editorNode);

        var outgoingConnections = new GDC.Array<ConnectionEndpoint?>();
        outgoingConnections.Resize(editorNode.GetOutputPortCount());
        _outgoingConnections.Add(editorNode, outgoingConnections);

        var incomingConnections = new GDC.Array<ConnectionEndpoint?>();
        incomingConnections.Resize(editorNode.GetInputPortCount());
        _incomingConnections.Add(editorNode, incomingConnections);

        editorNode.LogicNode.Connect(
            Resource.SignalName.Changed,
            new Callable(this, nameof(MarkAsUnsaved))
        );
        editorNode.Connect(
            GraphEditorNode.SignalName.OutputPortAdded,
            new Callable(this, nameof(OnOutputPortAdded))
        );
        editorNode.Connect(
            GraphEditorNode.SignalName.OutputPortRemoved,
            new Callable(this, nameof(OnOutputPortRemoved))
        );

        Saved = false;
    }

    public void RemoveNode(GraphEditorNode editorNode)
    {
        _editorNodes.Remove(editorNode.LogicNode);

        foreach (var (fromNode, fromPort, toNode, toPort) in GetConnections(editorNode))
            RemoveConnection(fromNode, fromPort, toNode, toPort);
        _outgoingConnections.Remove(editorNode);
        _incomingConnections.Remove(editorNode);

        Saved = false;
    }

    public IEnumerable<(
        GraphEditorNode fromNode,
        int fromPort,
        GraphEditorNode toNode,
        int toPort
    )> GetConnections(GraphEditorNode editorNode) =>
        GetIncomingConnections(editorNode).Concat(GetOutgoingConnections(editorNode));

    public IEnumerable<(
        GraphEditorNode fromNode,
        int fromPort,
        GraphEditorNode toNode,
        int toPort
    )> GetIncomingConnections(GraphEditorNode editorNode)
    {
        var incomingConnections = _incomingConnections[editorNode];
        for (int index = 0; index < incomingConnections.Count; index++)
        {
            if (incomingConnections[index] is ConnectionEndpoint source)
                yield return (source.EditorNode, source.Port, editorNode, index);
        }
    }

    public IEnumerable<(
        GraphEditorNode fromNode,
        int fromPort,
        GraphEditorNode toNode,
        int toPort
    )> GetOutgoingConnections(GraphEditorNode editorNode)
    {
        var outgoingConnections = _outgoingConnections[editorNode];
        for (int index = 0; index < outgoingConnections.Count; index++)
        {
            if (outgoingConnections[index] is ConnectionEndpoint target)
                yield return (editorNode, index, target.EditorNode, target.Port);
        }
    }

    public IEnumerable<(
        GraphEditorNode fromNode,
        int fromPort,
        GraphEditorNode toNode,
        int toPort
    )> GetOutgoingConnections() => _editorNodes.Values.SelectMany(GetOutgoingConnections);

    public void AddConnection(
        GraphEditorNode fromNode,
        int fromPort,
        GraphEditorNode toNode,
        int toPort
    )
    {
        _incomingConnections[toNode][toPort] = new(fromNode, fromPort);
        _outgoingConnections[fromNode][fromPort] = new(toNode, toPort);

        fromNode.OnOutputPortConnected(fromPort);

        Saved = false;
    }

    public void RemoveConnection(
        GraphEditorNode fromNode,
        int fromPort,
        GraphEditorNode toNode,
        int toPort
    )
    {
        _incomingConnections[toNode][toPort]!.Free();
        _incomingConnections[toNode][toPort] = null;
        _outgoingConnections[fromNode][fromPort]!.Free();
        _outgoingConnections[fromNode][fromPort] = null;

        fromNode.OnOutputPortDisconnected(fromPort);

        Saved = false;
    }

    void OnOutputPortAdded(GraphEditorNode node)
    {
        var connections = _outgoingConnections[node];
        connections.Add(null);
    }

    void OnOutputPortRemoved(GraphEditorNode node)
    {
        var connections = _outgoingConnections[node];
        connections.RemoveAt(connections.Count - 1);
    }

    public Vector2 EditorScrollOffset => _logicGraph.GraphEditorScrollOffset;

    public float EditorZoom => _logicGraph.GraphEditorZoom;

    public void SetEditorState(Vector2 scrollOffset, float zoom)
    {
        _logicGraph.GraphEditorScrollOffset = scrollOffset;
        _logicGraph.GraphEditorZoom = zoom;

        Saved = false;
    }

    void Load()
    {
        Deserialize();
        Saved = true;
    }

    void MarkAsUnsaved()
    {
        Saved = false;
    }

    public void Save()
    {
        Serialize();
        Saved = WriteToDisk();
    }

    void Deserialize()
    {
        AddNode(new GraphEditorNode(_logicGraph.Entrypoint));
        foreach (var logicNode in _logicGraph.Nodes)
            AddNode(new GraphEditorNode(logicNode));

        foreach (var logicConnection in _logicGraph.Connections)
        {
            var fromNode = _editorNodes[IndexToLogicNode(logicConnection.SourceNodeIndex)];
            var toNode = _editorNodes[IndexToLogicNode(logicConnection.TargetNodeIndex)];
            AddConnection(
                fromNode,
                fromNode.GetPortOfSignal(logicConnection.Signal),
                toNode,
                toNode.GetPortOfMethod(logicConnection.Method)
            );
        }

        foreach (var editorNode in _editorNodes.Values)
            DeserializeNode(editorNode);
    }

    void Serialize()
    {
        _logicGraph.Nodes = _editorNodes
            .Keys.Where(logicNode => logicNode != _logicGraph.Entrypoint)
            .ToArray();

        var logicNodeToIndex = LogicNodeToIndexFunc();
        _logicGraph.Connections = _editorNodes
            .Values.SelectMany(editorNode =>
                GetOutgoingLogicConnections(editorNode, logicNodeToIndex)
            )
            .ToArray();

        foreach (var editorNode in _editorNodes.Values)
            SerializeNode(editorNode, logicNodeToIndex);

        _logicGraph.EmitChanged();
    }

    LogicNode IndexToLogicNode(int index) =>
        index >= 0 ? _logicGraph.Nodes[index] : _logicGraph.Entrypoint;

    Func<LogicNode, int> LogicNodeToIndexFunc()
    {
        var logicNodeIndexes = _logicGraph
            .Nodes.Select((logicNode, index) => (logicNode, index))
            .ToDictionary();
        return logicNode => logicNodeIndexes.TryGetValue(logicNode, out var index) ? index : -1;
    }

    IEnumerable<LogicConnection> GetOutgoingLogicConnections(
        GraphEditorNode editorNode,
        Func<LogicNode, int> logicNodeToIndex
    )
    {
        if (editorNode.HasDynamicOutputs)
            return [];
        else
            return GetOutgoingConnections(editorNode)
                .Select(connection => new LogicConnection()
                {
                    SourceNodeIndex = logicNodeToIndex(connection.fromNode.LogicNode),
                    Signal = connection.fromNode.GetSignalOfPort(connection.fromPort)!,
                    TargetNodeIndex = logicNodeToIndex(connection.toNode.LogicNode),
                    Method = connection.toNode.GetMethodOfPort(connection.toPort)!
                });
    }

    void SerializeNode(GraphEditorNode editorNode, Func<LogicNode, int> logicNodeToIndex)
    {
        if (!editorNode.HasDynamicOutputs)
            return;

        var outputs = GetOutgoingConnections(editorNode)
            .Select(connection =>
                (
                    logicNodeToIndex(connection.toNode.LogicNode),
                    connection.toNode.GetMethodOfPort(connection.toPort)
                )
            );
        editorNode.SetDynamicOutputs(outputs);
    }

    void DeserializeNode(GraphEditorNode editorNode)
    {
        if (!editorNode.HasDynamicOutputs)
            return;

        int index = 0;
        foreach (var (nodeIndex, method) in editorNode.GetDynamicOutputs())
        {
            var outputNode = _editorNodes[IndexToLogicNode(nodeIndex)];
            int outputNodePort = 0;
            if (method is not null)
                outputNodePort = outputNode.GetPortOfMethod(method);
            AddConnection(editorNode, index, outputNode, outputNodePort);
            index++;
        }
    }

    bool WriteToDisk()
    {
        var resource = (Resource)_logicGraph;
        var path = _logicGraph.ResourcePath;
        if (path.Length == 0)
            return false;

        if (path.Contains("::"))
        {
            path = path.Split("::")[0];
            resource = GD.Load(path);
        }

        var error = ResourceSaver.Save(resource);
        if (error != Error.Ok)
        {
            GD.PushError("Failed to save logic graph at ", _logicGraph.ResourcePath, ": ", error);
            return false;
        }
        return true;
    }
}
#endif
