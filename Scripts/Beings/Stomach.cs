using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class Stomach : Node
{
    ItemGrid _itemGrid;
    JobScheduler _jobScheduler;

    [Export]
    Death _death;

    [Export]
    JobExecutor _jobExecutor;

    [Export]
    FoodConsumer _foodConsumer;

    [Export]
    ulong _maxCalories = 7500.KiloCalories();

    [Export]
    ulong _startCalories = 5000.KiloCalories();

    [Export]
    ulong _caloriesConsumedPerCycle = 2500.KiloCalories();

    [Export]
    ulong _foodConsumptionThreshold = 4000.KiloCalories();

    ulong _caloriesAfterLastMeal;
    double _timeSinceLastMeal;

    public override void _Ready()
    {
        _itemGrid = Owner.GetNode<Grids>("../%Grids").ItemGrid;
        _jobScheduler = Owner.GetNode<JobScheduler>("../%JobScheduler");

        _death.OnDeath += OnDeath;

        _caloriesAfterLastMeal = _startCalories;
    }

    void OnDeath()
    {
        ProcessMode = ProcessModeEnum.Disabled;
    }

    public override void _Process(double delta)
    {
        _timeSinceLastMeal += delta;

        var currentCalories = CurrentCalories();
        if (currentCalories == 0)
            _death.Die();
        else if (currentCalories < _foodConsumptionThreshold)
            _ = TryConsumeFood();
    }

    double CyclesSinceLastMeal() => _timeSinceLastMeal / Clock.CycleLength;

    ulong CaloriesConsumedSinceLastMeal() =>
        (ulong)(_caloriesConsumedPerCycle * CyclesSinceLastMeal());

    ulong CurrentCalories()
    {
        var caloriesConsumed = CaloriesConsumedSinceLastMeal();
        return caloriesConsumed > _caloriesAfterLastMeal
            ? 0
            : _caloriesAfterLastMeal - caloriesConsumed;
    }

    public void AddCalories(ulong calories)
    {
        _caloriesAfterLastMeal = CurrentCalories() + calories;
        _timeSinceLastMeal = 0d;
    }

    bool _isConsumingFood;
    ulong _nextConsumptionThreshold = ulong.MaxValue;

    async Task TryConsumeFood()
    {
        if (_isConsumingFood)
            return;

        var currentCalories = CurrentCalories();
        if (currentCalories > _nextConsumptionThreshold)
            return;

        GD.Print($"Try to consume food (stored: {currentCalories / 1.KiloCalorie()} kcal)");

        _isConsumingFood = true;
        if (await ConsumeFood(_maxCalories - currentCalories))
            _nextConsumptionThreshold = ulong.MaxValue;
        else
            _nextConsumptionThreshold = NextConsumptionThreshold(currentCalories);
        _isConsumingFood = false;
    }

    ulong NextConsumptionThreshold(ulong currentCalories)
    {
        var multiplier =
            currentCalories < 1000.KiloCalories() ? 100.KiloCalories() : 500.KiloCalories();
        return currentCalories / multiplier * multiplier;
    }

    async Task<bool> ConsumeFood(ulong calories)
    {
        var foodItems = _itemGrid.Filter<FoodItemDef>().CumulateCalories(calories);
        if (!foodItems.Any())
            return false;

        try
        {
            var markedCalories = foodItems.Sum(item => item.markedCalories);
            GD.Print($"Consume food: {markedCalories / 1.KiloCalorie()} kcal");

            var job = new EatFoodJob(foodItems.CaloriesToMass(), _foodConsumer);
            await _jobScheduler.Execute(job, _jobExecutor, CancellationToken.None);

            GD.Print("Food consumption completed");
            return true;
        }
        catch (TaskCanceledException)
        {
            GD.Print("Food consumption canceled");
            return false;
        }
    }
}
