using Godot;
using System;

public class DiceCop : RigidBody2D
{
    [Export] 
    private int _slowdownModulo = 2;


    private enum Direction
    {
        Left = -1,
        Right = 1
    }

    private bool _isSlowdown = false;
    private int _slowdownFrameCount = 0;

    private Vector2 _slowdownLinearVelocity = Vector2.Zero;
    private float _slowdownAngularVelocity = 0;


    public override void _PhysicsProcess(float delta)
    {
        if (Input.IsActionJustPressed("left"))
        {
            Jump(Direction.Left);
        }

        if (Input.IsActionJustPressed("right"))
        {
            Jump(Direction.Right);
        }
    }

    public override void _IntegrateForces(Physics2DDirectBodyState state)
    {
        if (!_isSlowdown && Input.IsActionPressed("slow"))
        {
            _isSlowdown = true;
            _slowdownLinearVelocity = state.LinearVelocity;
            _slowdownAngularVelocity = state.AngularVelocity;
        }
        else if (_isSlowdown && !Input.IsActionPressed("slow"))
        {
            _isSlowdown = false;
        }

        if (_isSlowdown)
        {
            if (IsSlowdownFrame(++_slowdownFrameCount))
            {
                state.LinearVelocity = Vector2.Zero;
                state.AngularVelocity = 0;
            }
            else 
            {
                state.LinearVelocity = _slowdownLinearVelocity;
                state.AngularVelocity = _slowdownAngularVelocity;
            }
        }

        state.IntegrateForces();

        if (!IsSlowdownFrame(_slowdownFrameCount))
        {
            _slowdownLinearVelocity = state.LinearVelocity;
            _slowdownAngularVelocity = state.AngularVelocity;
        }
    }

    private void Stop()
    {
        ApplyCentralImpulse(-LinearVelocity);
        AngularVelocity = 0;
    }

    private void Jump(Direction direction)
    {
        Stop();
        ApplyCentralImpulse(new Vector2((int)direction * 100, -50));
        ApplyTorqueImpulse((int)direction * 360);
    }

    private bool IsSlowdownFrame(int frame) => frame % _slowdownModulo == 0;
}
