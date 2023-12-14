using Godot;

[GlobalClass]
public partial class NavCell : NavigationRegion2D
{
    const int EnabledNavLayer = 1;
    const int DisabledNavLayer = 2;

    bool _traversable = true;

    public NavCell(Vector2I size)
    {
        var polygon = new NavigationPolygon();
        polygon.AddOutline(
            new Vector2[] { new(0f, 0f), new(size.X, 0f), new(size.X, size.Y), new(0f, size.Y) }
        );
        polygon.MakePolygonsFromOutlines();

        NavigationPolygon = polygon;
    }

    public bool Traversable
    {
        get => _traversable;
        set
        {
            if (_traversable == value)
                return;

            _traversable = value;
            if (_traversable)
            {
                SetNavigationLayerValue(EnabledNavLayer, true);
                SetNavigationLayerValue(DisabledNavLayer, false);
            }
            else
            {
                SetNavigationLayerValue(EnabledNavLayer, false);
                SetNavigationLayerValue(DisabledNavLayer, true);
            }
        }
    }
}
