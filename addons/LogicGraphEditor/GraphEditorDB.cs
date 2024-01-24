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

        void FreeConnections(
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

        Saved = false;
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
    ) =>
        GetOutgoingConnections(editorNode)
            .Select(connection => new LogicConnection()
            {
                SourceNodeIndex = logicNodeToIndex(connection.fromNode.LogicNode),
                Signal = connection.fromNode.GetSlotOfOutputPort(connection.fromPort).Signal!,
                TargetNodeIndex = logicNodeToIndex(connection.toNode.LogicNode),
                Method = connection.toNode.GetSlotOfInputPort(connection.toPort).Method!
            });

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
            GD.PushError($"Failed to save logic graph at ", _logicGraph.ResourcePath, ": ", error);
            return false;
        }
        return true;
    }
}
#endif
