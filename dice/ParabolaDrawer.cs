using Godot;
using System;
using System.Linq;

public class ParabolaDrawer : Control
{
    private static readonly Vector2 Gravity = 
        (Vector2) ProjectSettings.GetSetting("physics/2d/default_gravity_vector") *
        (int) ProjectSettings.GetSetting("physics/2d/default_gravity");


    [Export]
    private float _duration = 1.0f;

    [Export]
    private int _resolution = 30;

    [Export] 
    private Color _color = Colors.Red;

    [Export]
    private float _width = 2.0f;


    private DiceCop _diceCop;

    public override void _Ready()
    {
        _diceCop = GetParent<DiceCop>();
    }

    public override void _PhysicsProcess(float delta)
    {
        Update();
    }

    public override void _Draw()
    {
        if (_diceCop.IsChargingJump)
        {
            var parabolicPoints = Enumerable
                .Range(0, _resolution)
                .Select(i => _duration * (float) i / (float) _resolution)
                .Select(t => CalculateParabolicPoint(_diceCop.CalculateJumpVelocity(), t))
                .ToArray();

            DrawSetTransform(Vector2.Zero, -_diceCop.Rotation, Vector2.One);
            DrawPolyline(parabolicPoints, _color, _width, true);
        }
    }

    
    private Vector2 CalculateParabolicPoint(Vector2 initialVelocity, float t)
    {
        var gravityTimeScale = 0.5f * t * t;

        return Gravity * gravityTimeScale + initialVelocity * t;
    }
}
