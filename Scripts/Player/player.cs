using System;
using Godot;

public partial class player : CharacterBody2D
{
    private float speed = 300f;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private AnimatedSprite2D animatedSprite;
    private dodge dodge; 
    private attack attackScript;
    private bool isAttackAnimationPlaying = false;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        dodge = new dodge(this); 
        attackScript = new attack(this); // Correctly instantiate the attack script
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        dodge.UpdateDodge((float)delta);
        if (!dodge.IsDodging())
        {
            handleInput();
            attackScript.UpdateAttack((float)delta); // Update attack logic
            handleAnimation();
        }
        MoveAndSlide();
    }

    private void handleInput()
    {
        currentVelocity = Input.GetVector("left", "right", "up", "down");
        currentVelocity *= speed;

        // Update current direction based on velocity
        if (currentVelocity != Vector2.Zero)
        {
            currentDirection = currentVelocity.Normalized();
        }

        if (Input.IsActionJustPressed("dodge"))
        {
            dodge.StartDodge(currentVelocity);
        }
        else
        {
            Velocity = currentVelocity; 
        }
    }

    public void handleAnimation()
    {
        Vector2 directionToMouse = GetGlobalMousePosition() - GlobalPosition;

        if (attackScript.IsAttacking())
        {
            if (Math.Abs(directionToMouse.X) > Math.Abs(directionToMouse.Y))
            {
                if (directionToMouse.X > 0)
                {
                    animatedSprite.Play("attack_right");
                    animatedSprite.FlipH = false;
                }
                else
                {
                    animatedSprite.Play("attack_right");
                    animatedSprite.FlipH = true;
                }
            }
            else
            {
                if (directionToMouse.Y > 0)
                {
                    animatedSprite.Play("attack_down");
                    animatedSprite.FlipH = false;
                }
                else
                {
                    animatedSprite.Play("attack_up");
                    animatedSprite.FlipH = false;
                }
            }
            return; // Exit early to ensure attack animation is not interrupted
        }

        if (dodge.IsDodging())
        {
            animatedSprite.Play("dodge");
            return; // Exit early to ensure dodge animation is not interrupted
        }

        if (currentVelocity == Vector2.Zero)
        {
            #region Idle Animation
            // Idle Animation Handling
            if (Math.Abs(directionToMouse.X) > Math.Abs(directionToMouse.Y))
            {
                if (directionToMouse.X > 0) 
                {
                    animatedSprite.Play("idle_right");
                    animatedSprite.FlipH = false;
                }
                else 
                {
                    animatedSprite.Play("idle_right");
                    animatedSprite.FlipH = true;
                }
            }
            else 
            {
                if (directionToMouse.Y > 0) 
                {
                    animatedSprite.Play("idle_down");
                    animatedSprite.FlipH = false;
                }
                else 
                {
                    animatedSprite.Play("idle_up");
                    animatedSprite.FlipH = false;
                }
            }
            #endregion
        }
        else
        {
            #region Movement Animation
            // Movement Animation Handling
            if (currentVelocity.Y < 0)
            {
                animatedSprite.Play("move_up");
                animatedSprite.FlipH = false;
            }
            else if (currentVelocity.Y > 0)
            {
                animatedSprite.Play("move_down");
                animatedSprite.FlipH = false;
            }
            else if (currentVelocity.X < 0)
            {
                animatedSprite.Play("move_right");
                animatedSprite.FlipH = true;
            }
            else if (currentVelocity.X > 0)
            {
                animatedSprite.Play("move_right");
                animatedSprite.FlipH = false;
            }
            #endregion
        }
    }
}