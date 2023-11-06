using Godot;

[GlobalClass]
public partial class Health : Node
{
    [Signal]
    public delegate void OnZeroHealthEventHandler();

    [Signal]
    public delegate void OnMaxHealthEventHandler();

    IHealthHolderConf _conf;

    public uint MaxHealthPoints => _conf.MaxHealthPoints;
    public uint HealthPoints { get; private set; } = 1;

    public override void _Ready()
    {
        _conf = GetNode<IHealthHolderConf>("../DefHolder");
    }

    public void AddHealth(int points)
    {
        HealthPoints = MathUint.ClampAdd(HealthPoints, points, MaxHealthPoints);
        NotifyHealthChange();
    }

    public void SetTotalHealth(float ratio)
    {
        var newHealthPoints = (uint)Mathf.RoundToInt(MaxHealthPoints * ratio);
        if (newHealthPoints != HealthPoints)
        {
            HealthPoints = newHealthPoints;
            NotifyHealthChange();
        }
    }

    void NotifyHealthChange()
    {
        if (HealthPoints == 0)
            EmitSignal(SignalName.OnZeroHealth);
        else if (HealthPoints == MaxHealthPoints)
            EmitSignal(SignalName.OnMaxHealth);
    }
}
