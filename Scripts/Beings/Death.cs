using System;
using Godot;

[GlobalClass]
public partial class Death : Node
{
    public event Action Died;

    public void Die()
    {
        Died?.Invoke();
    }
}
