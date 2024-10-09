using Godot;
using System;

public partial class DefaultAttack : Node
{
    private bool windup = false;
    private float time = 0.3f;
    private float cooldown = 0.0f;

    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection;

    private CharacterBody2D player;

    private AnimatedSprite2D playerSprite;
    private AnimatedSprite2D effectSprite;

    public DefaultAttack(CharacterBody2D player, AnimatedSprite2D playerSprite, AnimatedSprite2D effectSprite)
    {
        this.player = player;
        this.playerSprite = playerSprite;
        this.effectSprite = effectSprite;
    }

    public void UpdateAttack(float delta)
    {
        Attack(delta);
    }

    public void Attack(float delta)
    {
        if (windup)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                windup = false;
                attackCooldownTimer = cooldown;
                effectSprite.Visible = false;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("meleeAttack"))
            {
                StartAttack();
            }
        }
    }

    private void StartAttack()
    {
        windup = true;
        currentAttackCooldownTimer = time;

        Vector2 directionToMouse = player.GetGlobalMousePosition() - player.GlobalPosition;
        attackDirection = directionToMouse.Normalized();

        effectSprite.Visible = true;

        PlayAttackAnimations();
    }

    private void PlayAttackAnimations()
    {
        bool isMouseOnLeft = attackDirection.X < 0;

        string effectAnimation = isMouseOnLeft ? "default_attack_left" : "default_attack";

        effectSprite.FlipH = isMouseOnLeft;

        Vector2 effectOffset = attackDirection * 40; 

        effectSprite.GlobalPosition = player.GlobalPosition + effectOffset;

        float angleToMouse = attackDirection.Angle();

        effectSprite.Rotation = angleToMouse;

        playerSprite.Stop();
        playerSprite.Play("default_attack"); 
        playerSprite.Frame = 0;

        effectSprite.Stop();
        effectSprite.Play(effectAnimation);
        effectSprite.Frame = 0;
    }

    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }

    public bool Windup()
    {
        return windup;
    }
}
