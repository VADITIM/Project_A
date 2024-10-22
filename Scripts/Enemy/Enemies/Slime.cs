using Godot;
using System;

public partial class Slime : Enemy
{
    public override void _Ready() 
    {
        health = 30f;
        damage = 3f;

        base._Ready();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
