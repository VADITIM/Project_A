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

        Vector2 effectOffset = attackDirection * 20; 

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


    private string GetDirectionFromAngle(float angle)
    {
        angle = Mathf.PosMod(angle, Mathf.Tau); 

        if (angle < Mathf.Pi / 8 || angle >= 15 * Mathf.Pi / 8) return "right";
        if (angle < 3 * Mathf.Pi / 8) return "downright";
        if (angle < 5 * Mathf.Pi / 8) return "down";
        if (angle < 7 * Mathf.Pi / 8) return "downleft";
        if (angle < 9 * Mathf.Pi / 8) return "left";
        if (angle < 11 * Mathf.Pi / 8) return "upleft";
        if (angle < 13 * Mathf.Pi / 8) return "up";
        if (angle < 15 * Mathf.Pi / 8) return "upright";

        return "down";
    }

    private Vector2 GetEffectOffset(string direction)
    {
        return direction switch
        {
            "right" => new Vector2(20, -10),
            "downright" => new Vector2(15, 0),
            "down" => new Vector2(0, 10),
            "downleft" => new Vector2(-15, 0),  
            "left" => new Vector2(-20, -10),
            "upleft" => new Vector2(-15, -20), 
            "up" => new Vector2(0, -20),
            "upright" => new Vector2(15, -20),
            _ => Vector2.Zero,
        };
    }

    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }

    public bool Windup()
    {
        return windup;
    }

    public void HandleAttackAnimation(Vector2 currentVelocity)
    {
        if (!Windup())
        {
            Vector2 directionToMouse = player.GetGlobalMousePosition() - player.GlobalPosition;
            string direction = GetDirectionFromAngle(directionToMouse.Angle());

            bool isLeftOrRight = (direction == "left" || direction == "right");
            playerSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");

            if (currentVelocity == Vector2.Zero)
            {
                string animationName = "gunner_idle_" + 
                    (isLeftOrRight ? "downright" : 
                    direction == "downleft" ? "downright" : 
                    direction == "upleft" ? "upright" : direction);
                playerSprite.Play(animationName);
            }
            else
            {
                string animationName = "gunner_move_" + 
                    (isLeftOrRight ? "downright" : 
                    direction == "downleft" ? "downright" : 
                    direction == "upleft" ? "upright" : direction);
                playerSprite.Play(animationName);
            }
        }
    }
}
