#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class IterationConsumer : LogicNodeTemplate
{
    [Export]
    uint _iterationCount = 1;

    uint _currentCount;

    protected override bool _Process()
    {
        if (_currentCount < _iterationCount)
            _currentCount++;
        return _currentCount == _iterationCount;
    }

    protected override void _Reset()
    {
        _currentCount = 0;
    }
}
