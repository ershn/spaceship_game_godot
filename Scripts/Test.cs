using Godot;

public partial class Test : Node2D
{
    [Export]
    NavigationRegion2D _region;

    public override void _Ready()
    {
        NavigationServer2D.MapChanged += map => GD.Print("Map changed");
        Callable.From(Setup).CallDeferred();
    }

    async void Setup()
    {
        await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        GD.Print($"Before change: {_region.NavigationLayers}");
        _region.SetNavigationLayerValue(1, false);
        _region.SetNavigationLayerValue(2, true);
        GD.Print($"After change: {_region.NavigationLayers}");
    }
}
