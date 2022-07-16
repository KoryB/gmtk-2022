using Godot;
using System;

public class LoadedDice : RigidBody2D
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GetNode<Timer>("DeathTimer").Connect("timeout", this, nameof(ActuallyDie));
    }

    public void Kill()
    {
        GetNode<Timer>("DeathTimer").Start();
    }

    private void ActuallyDie()
    {
        this.QueueFree();
    }
}
