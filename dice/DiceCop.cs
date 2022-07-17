using Godot;
using System;

public class DiceCop : Dice
{
    private static readonly Vector2 UnstickImpulse = new Vector2(0, 10);

    [Export] 
    private int _slowdownModulo = 2;
    
    [Export]
    private int _jumpsLeft = 5;

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

    private bool _isStuck = false;

    private Vector2 _slowdownLinearVelocity = Vector2.Zero;
    private float _slowdownAngularVelocity = 0;

    private float _cantStickTime = 0.5f;
    private float _cantStickTimer = 0;


    public override void _Ready()
    {
        base._Ready();

        GetNode<Area2D>("LoadedDiceDetector").Connect("body_entered", this, "OnBodyEntered");
        SyncFrame();
    }

    public override void _PhysicsProcess(float delta)
    {

        if (CanJump())
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

        if (_isStuck)
        {
            Stop();

            if (Input.IsActionJustPressed("down"))
            {
                Unstick();
                ApplyCentralImpulse(UnstickImpulse);
                _cantStickTimer = 0;
            }
        }

        _cantStickTimer += delta;
        base._PhysicsProcess(delta);
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
            if (!dice.IsDead)
            {
                _jumpsLeft += dice.Value;
                dice.Kill();
            }
        }
    }

    internal Vector2 CalculateJumpVelocity()
    {
        var direction = Input.IsActionPressed("left")
            ? Direction.Left : Input.IsActionPressed("right")
            ? Direction.Right : Direction.None;

        return JumpStrength * GetJumpDirectionVector(direction);
    }

    public void Stick()
    {
        if (CanStick())
        {
            GD.Print("Stick");
            Stop();
            _isChargingJump = false;
            _isStuck = true;
            this.GravityScale = 0.0f;
        }
    }

    private void Unstick()
    {
        GD.Print("Unstick");
        _isStuck = false;
        this.GravityScale = 1.0f;
    }

    private void Stop()
    {
        ApplyCentralImpulse(-LinearVelocity);
        AngularVelocity = 0;
    }

    private void Jump(Direction direction)
    {
        Stop();
        Unstick();
        _isChargingJump = false;
        _jumpsLeft--;
        var d = GetJumpDirectionVector(direction);

        ApplyCentralImpulse(JumpStrength * d);
        ApplyTorqueImpulse(JumpRotation * GetJumpAngle() * (float)direction);
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

    protected override void SyncFrame()
    {
        _frame = _jumpsLeft;

        base.SyncFrame();
    }

    private bool CanJump() => _jumpsLeft > 0;

    private bool CanStick() => _cantStickTimer >= _cantStickTime;

    private bool IsSlowdownFrame(int frame) => frame % _slowdownModulo == 0;

    private float Smooth(float t) => Mathf.Ease(t, 1.3f);
}
