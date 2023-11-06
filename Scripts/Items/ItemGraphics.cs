using Godot;

[GlobalClass]
public partial class ItemGraphics : Sprite2D, ITemplate<ItemDef>
{
    public void Template(ItemDef itemDef)
    {
        Texture = itemDef.AmountSprites[0].Sprite;
    }

    ItemDef _itemDef;

    AmountSprite[] _amountSprites;

    public override void _Ready()
    {
        _itemDef = GetNode<ItemDefHolder>("../DefHolder").ItemDef;

        _amountSprites = _itemDef.AmountSprites;
    }

    public void ToAmountSprite(ulong amount)
    {
        for (int index = _amountSprites.Length - 1; index >= 0; index--)
        {
            var amountSprite = _amountSprites[index];
            if (amount >= amountSprite.MinAmount)
            {
                Texture = amountSprite.Sprite;
                break;
            }
        }
    }
}
