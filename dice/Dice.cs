using Godot;
using System;

public class Dice : RigidBody2D
{
    protected const int MaxJumpFrames = 6;
    protected const int MinJumpFrames = 0;

    protected int _frame = 0;

    public override void _PhysicsProcess(float delta)
    {
        SyncFrame();
    }

    protected virtual void SyncFrame()
    {
        GetNode<AnimatedSprite>("Sprite").Frame = Mathf.Clamp(_frame, MinJumpFrames, MaxJumpFrames);
    }
}
