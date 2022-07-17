using Godot;
using System;

public class Door : Area2D
{
    [Export(PropertyHint.File)]
    private string _nextLevelPath;

    public bool IsOpen = false;

    public override void _Ready()
    {
        base._Ready();

        this.Connect("body_entered", this, nameof(TryGotoNextLevel));
    }

    private void TryGotoNextLevel(Node body)
    {
        if (IsOpen && body is DiceCop diceCop)
        {
            GetTree().ChangeScene(_nextLevelPath);
        }
    }

    public override void _Process(float delta)
    {
        var loadedDiceCount = GetTree().GetNodesInGroup(LoadedDice.Group).Count;

        IsOpen = loadedDiceCount == 0;

        GetNode<AnimatedSprite>("Sprite").Frame = IsOpen? 1 : 0;
    }
}
