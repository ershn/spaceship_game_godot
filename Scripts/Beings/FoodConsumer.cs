using Godot;

[GlobalClass]
public partial class FoodConsumer : Node, IWork
{
    [Export]
    Backpack _backpack;

    [Export]
    Stomach _stomach;

    [Export]
    double _timePerKiloGramConsumed = 2d;

    double _accumulatedTime;

    public bool Work(double time)
    {
        _accumulatedTime += time;
        if (_accumulatedTime < .5f)
            return false;

        var massConsumed = MassConsumed(_accumulatedTime);
        var isBackpackEmpty = !ConsumeMass(massConsumed);
        _accumulatedTime = 0f;
        return isBackpackEmpty;
    }

    ulong MassConsumed(double time) => (ulong)(time / _timePerKiloGramConsumed * 1.KiloGram());

    bool ConsumeMass(ulong targetMass)
    {
        var foodItem = _backpack.First<FoodItemDef>();
        do
        {
            var (itemDef, itemMass) = foodItem;
            if (targetMass > itemMass)
            {
                ConsumeItem(itemDef, itemMass);
                targetMass -= itemMass;
            }
            else
            {
                ConsumeItem(itemDef, targetMass);
                return true;
            }
        } while (_backpack.TryFirst(out foodItem));

        return false;
    }

    void ConsumeItem(FoodItemDef foodItemDef, ulong mass)
    {
        var consumedCalories = foodItemDef.MassToCalories(mass);
        _stomach.AddCalories(consumedCalories);
        _backpack.Remove(foodItemDef, mass);
    }

    public Vector2 GlobalPosition => GetOwner<Node2D>().GlobalPosition;

    public Vector2 Position => GetOwner<Node2D>().Position;
}
