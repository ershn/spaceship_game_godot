using Godot;

[GlobalClass]
public partial class AmountSprite : Resource
{
    // TODO: display as mass/count
    [Export]
    public ulong MinAmount;

    [Export]
    public Texture2D Sprite;
}
