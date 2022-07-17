using Godot;
using System;

public class LoadedDice : Dice
{
    public const string Group = "LoadedDice";

    public bool IsDead = false;


    [Export]
    public int Value = 2;

    public override void _Ready()
    {
        GetNode<Timer>("DeathTimer").Connect("timeout", this, nameof(ActuallyDie));
    }

    public void Kill()
    {
        GetNode<Timer>("DeathTimer").Start();
        IsDead = true;
    }

    private void ActuallyDie()
    {
        this.QueueFree();
    }

    protected override void SyncFrame()
    {
        _frame = Value;
        base.SyncFrame();
    }
}
