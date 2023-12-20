using Godot;

[GlobalClass]
public partial class AmountSprite : Resource
{
    [Export(hintString: AmountHint.Any)]
    public ulong MinAmount;

    [Export]
    public Texture2D Sprite;
}
