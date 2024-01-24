#nullable enable
using System;

namespace LogicGraphs;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class InternalAttribute : Attribute { }
