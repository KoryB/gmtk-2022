using Godot;
using System;

[Tool]
public class Wall : RigidBody2D
{
    [Export]
    public bool IsBouncy = false;

    [Export]
    public bool IsSticky = false;

    [Export]
    public float Bounciness = 0.5f;

    [Export]
    public Color BouncyColor = Colors.DeepPink;

    [Export]
    public Color StickyColor = Colors.WebGreen;

    [Export]
    public Color NormalColor = Colors.SandyBrown;


    public override void _Ready()
    {
        SyncSettings();
    }

    private void SyncSettings(bool initialize = true)
    {
        if (IsBouncy)
        {
            this.Bounce = Bounciness;
            this.GetNode<Node2D>("Sprite").SelfModulate = BouncyColor;
        }
        else if (IsSticky)
        {
            this.PhysicsMaterialOverride = new PhysicsMaterial();
            this.PhysicsMaterialOverride.Rough = true;
            this.PhysicsMaterialOverride.Absorbent = true;
            this.PhysicsMaterialOverride.Bounce = 1.0f;

            this.GetNode<Node2D>("Sprite").SelfModulate = StickyColor;

            if (initialize)
            {
                this.GetNode<Area2D>("PlayerDetector").Connect("body_entered", this, nameof(Stick));
            }
        }
        else
        {
            this.GetNode<Node2D>("Sprite").SelfModulate = NormalColor;
        }
    }

    public override void _Process(float delta)
    {
        base._Process(delta);

        if (Engine.EditorHint)
        {
            SyncSettings(false);
        }
    }


    private void Stick(Node body)
    {
        if (body is DiceCop diceCop)
        {
            diceCop.Stick();
        }
    }
}
