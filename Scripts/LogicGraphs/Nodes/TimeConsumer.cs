#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class TimeConsumer : LogicNodeTemplate
{
    [Export]
    double _timeInterval = 1d;

    double _consumedTime;

    protected override bool _Process()
    {
        if (_consumedTime < _timeInterval)
            _consumedTime += Entity.GetProcessDeltaTime();
        return _consumedTime >= _timeInterval;
    }

    protected override void _Reset()
    {
        _consumedTime -= _timeInterval;
    }
}
