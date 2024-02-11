#nullable enable
using Godot;

namespace LogicGraphs;

[Tool]
public partial class TimeConsumer : ProgressingNode
{
    [Export]
    double _timeInterval = 1d;

    double _consumedTime;

    protected override float _Process()
    {
        if (_consumedTime < _timeInterval)
            _consumedTime += Entity.GetProcessDeltaTime();
        return (float)(_consumedTime / _timeInterval);
    }

    protected override void _Reset()
    {
        _consumedTime -= _timeInterval;
    }
}
