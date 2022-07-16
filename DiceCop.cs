using Godot;
using System;

public class DiceCop : RigidBody2D
{
    [Export] 
    private int _slowdownModulo = 2;

    [Export]
    public float JumpStrength = 100.0f;
    
    [Export]
    public float JumpRotation = 400.0f;

    [Export]
    public Vector2 JumpAngleBounds = new Vector2(Mathf.Pi / 6.0f, Mathf.Pi / 2.0f);
    
    [Export]
    public float JumpAngleChangeDuration = 2.0f;


    public bool IsChargingJump => _isChargingJump;


    private enum Direction
    {
        Left = -1,
        Right = 1,
        None = 0
    }

    private bool _isSlowdown = false;
    private int _slowdownFrameCount = 0;

    private bool _isChargingJump = false;
    private float _jumpChargeTime = 0.0f;

    private Vector2 _slowdownLinearVelocity = Vector2.Zero;
    private float _slowdownAngularVelocity = 0;


    public override void _Ready()
    {
        base._Ready();

        GetNode<Area2D>("LoadedDiceDetector").Connect("body_entered", this, "OnBodyEntered");
    }


    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed("left") || Input.IsActionJustPressed("right"))
        {
            _isSlowdown = true;
            _isChargingJump = true;
            _jumpChargeTime = 0.0f;
        }

        if (Input.IsActionJustReleased("left"))
        {
            _isSlowdown = false;
            Jump(Direction.Left);
        }
        else if (Input.IsActionJustReleased("right"))
        {
            _isSlowdown = false;
            Jump(Direction.Right);
        }

        if (_isChargingJump)
        {
            _jumpChargeTime += delta;
        }

        if (_isSlowdown)
        {
            Engine.TimeScale = 1.0f / _slowdownModulo;
        }
        else 
        {
            Engine.TimeScale = 1.0f;
        }
    }

    // public override void _IntegrateForces(Physics2DDirectBodyState state)
    // {
    //     if (!_isSlowdown && Input.IsActionPressed("slow"))
    //     {
    //         _isSlowdown = true;
    //         _slowdownLinearVelocity = state.LinearVelocity;
    //         _slowdownAngularVelocity = state.AngularVelocity;
    //     }
    //     else if (_isSlowdown && !Input.IsActionPressed("slow"))
    //     {
    //         _isSlowdown = false;
    //     }

    //     if (_isSlowdown)
    //     {
    //         if (IsSlowdownFrame(++_slowdownFrameCount))
    //         {
    //             state.LinearVelocity = Vector2.Zero;
    //             state.AngularVelocity = 0;
    //         }
    //         else 
    //         {
    //             state.LinearVelocity = _slowdownLinearVelocity;
    //             state.AngularVelocity = _slowdownAngularVelocity;
    //         }
    //     }

    //     state.IntegrateForces();

    //     if (!IsSlowdownFrame(_slowdownFrameCount))
    //     {
    //         _slowdownLinearVelocity = state.LinearVelocity;
    //         _slowdownAngularVelocity = state.AngularVelocity;
    //     }
    // }

    private void OnBodyEntered(Node body)
    {
        if (body is LoadedDice dice)
        {
            dice.Kill();
        }
    }

    internal Vector2 CalculateJumpVelocity()
    {
        var direction = Input.IsActionPressed("left")
            ? Direction.Left : Input.IsActionPressed("right")
            ? Direction.Right : Direction.None;

        return JumpStrength * GetJumpDirectionVector(direction);
    }

    private void Stop()
    {
        ApplyCentralImpulse(-LinearVelocity);
        AngularVelocity = 0;
    }

    private void Jump(Direction direction)
    {
        Stop();
        _isChargingJump = false;
        var d = GetJumpDirectionVector(direction);

        ApplyCentralImpulse(JumpStrength * d);
        ApplyTorqueImpulse(JumpRotation * (float)direction);
    }

    private Vector2 GetJumpDirectionVector(Direction direction)
    {
        var angle = GetJumpAngle();
        return new Vector2((float)direction * Mathf.Cos(angle), -Mathf.Sin(angle));
    }

    private float GetJumpAngle() => 
        (JumpAngleBounds.y - JumpAngleBounds.x) * 
        Smooth((Mathf.Cos(2.0f * Mathf.Pi * _jumpChargeTime / JumpAngleChangeDuration - Mathf.Pi) + 1.0f) / 2.0f) + 
        JumpAngleBounds.x;

    private bool IsSlowdownFrame(int frame) => frame % _slowdownModulo == 0;

    private float Smooth(float t) => Mathf.Ease(t, 1.3f);
}
