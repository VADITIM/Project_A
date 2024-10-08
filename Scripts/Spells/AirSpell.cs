using Godot;
using System;

public partial class AirSpell : Node
{
    private bool windup = false;
    private float time = 1.55f;
    private float cooldown = 1f;
    
    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection;

    private CharacterBody2D player;
    
    private AnimatedSprite2D playerSprite;
    private AnimatedSprite2D effectSprite;

    public AirSpell(CharacterBody2D player, AnimatedSprite2D effectSprite)
    {
        this.player = player;
        this.effectSprite = effectSprite;
        this.effectSprite.Visible = false;
    }

    public void UpdateAttack(float delta)
    {
        AirSpellAttack(delta);
    }

    public void AirSpellAttack(float delta)
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
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("spell1"))
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
        GD.Print("Spell burning in direction: " + attackDirection);
        effectSprite.Visible = true;

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (Math.Abs(attackDirection.X) > Math.Abs(attackDirection.Y))
        {
            if (attackDirection.X > 0)
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(70, 0);
                effectSprite.FlipH = false;
                effectSprite.Rotation = 0;
            }
            else
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(-70, 0);
                effectSprite.FlipH = true;
                effectSprite.Rotation = 0;
            }
        }
        else
        {
            if (attackDirection.Y > 0)
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(0, 70);
                effectSprite.FlipH = false;
                effectSprite.Rotation = 90;
            }
            else
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(0, -70);
                effectSprite.FlipH = false;
                effectSprite.Rotation = -90;
            }
        }
        effectSprite.Stop();
        effectSprite.Play();
        effectSprite.Frame = 0; 
    }


    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }
}
