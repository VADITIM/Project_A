using Godot;
using System;

public partial class IdleState : State
{
    public override void Enter()
    {
    	GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");
    }
}
