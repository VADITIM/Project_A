using Godot;
using System;

public partial class Slime : Enemy
{
    public override void _Ready() 
    {
        health = 300f;
        damage = 3f;
        base._Ready();

        stateMachine = GetNode<StateMachine>("State Machine Component");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        GD.Print($" 'Slime' - {Name} HP: {health}");
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        GD.Print($" 'Slime' - {Name} took damage. Current HP: {health}");
    }
}
