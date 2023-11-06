using System;
using Godot;

[GlobalClass]
public partial class Death : Node
{
    public event Action OnDeath;

    public void Die()
    {
        OnDeath?.Invoke();
    }
}
