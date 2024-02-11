#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class IterationConsumer : ProgressingNode
{
    [Export]
    uint _iterationCount = 1;

    uint _currentCount;

    protected override float _Process()
    {
        if (_currentCount < _iterationCount)
            _currentCount++;
        return _currentCount / (float)_iterationCount;
    }

    protected override void _Reset()
    {
        _currentCount = 0;
    }
}
