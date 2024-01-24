#if TOOLS
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Godot;

namespace LogicGraphs.Editor;

public partial class LogicNodeDef : RefCounted, ISerializationListener
{
    static Dictionary<Type, LogicNodeDef>? s_defs;

    static Dictionary<Type, LogicNodeDef> LoadDefs()
    {
        if (s_defs is null)
        {
            var baseType = typeof(LogicNode);
            var assembly = baseType.Assembly;
            var inheritingTypes = assembly
                .GetTypes()
                .Where(type => type.IsSubclassOf(baseType) && !type.IsAbstract);
            s_defs = inheritingTypes
                .Select(type => new LogicNodeDef(type))
                .ToDictionary(logicNode => logicNode.Type!);
        }
        return s_defs;
    }

    public static IEnumerable<LogicNodeDef> GetDefs() => LoadDefs().Values;

    public static LogicNodeDef GetDef(Type type) => LoadDefs()[type];

    string _assemblyQualifiedName;
    bool _typeFound;

    public Type? Type { get; private set; }
    public string Name { get; private set; }
    ConstructorInfo? _constructor;
    public string[] Methods { get; private set; }
    public string[] Signals { get; private set; }
    public bool Internal { get; private set; }

#pragma warning disable CS8618
    LogicNodeDef() { }
#pragma warning restore CS8618

    LogicNodeDef(Type type)
    {
        Initialize(type);
        _typeFound = true;
    }

    [MemberNotNull(
        nameof(_assemblyQualifiedName),
        nameof(Type),
        nameof(Name),
        nameof(_constructor),
        nameof(Methods),
        nameof(Signals)
    )]
    void Initialize(Type type)
    {
        _assemblyQualifiedName = type.AssemblyQualifiedName!;
        Type = type;
        Name = type.Name;
        _constructor = GetConstructor(type);
        Methods = GetDeclaredMethods(type).Select(method => method.Name).ToArray();
        Signals = GetDeclaredEvents(type).Select(@event => @event.Name).ToArray();
        Internal = Attribute.GetCustomAttribute(type, typeof(InternalAttribute)) is not null;
    }

    ConstructorInfo GetConstructor(Type type) =>
        type.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            Type.EmptyTypes
        ) ?? throw new ArgumentException($"{type} doesn't define a parameterless constructor");

    IEnumerable<MethodInfo> GetDeclaredMethods(Type type) =>
        type.GetMethods()
            .Where(method =>
                !method.IsSpecialName && IsLogicNodeMember(method) && !IsInternalMember(method)
            );

    IEnumerable<EventInfo> GetDeclaredEvents(Type type) =>
        type.GetEvents().Where(IsLogicNodeMember);

    bool IsLogicNodeMember(MemberInfo member) =>
        member.DeclaringType!.IsAssignableTo(typeof(LogicNode));

    bool IsInternalMember(MemberInfo member) =>
        Attribute.GetCustomAttribute(member, typeof(InternalAttribute)) is not null;

    public LogicNode Instantiate() => (LogicNode)_constructor!.Invoke([]);

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        var type = Type.GetType(_assemblyQualifiedName);
        if (type is not null)
            Initialize(type);
        _typeFound = type is not null;
    }
}
#endif
