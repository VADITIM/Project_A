using Godot;

public partial class Dodge : Node
{
    private bool isDodging = false;
    private float dodgeSpeed = 1200f;
    private float dodgeTime = .12f;   
    private float dodgeCooldown = .6f; 
    private float dodgeCooldownTimer = 0f;   
    private float currentDodgeCooldownTimer;   
    private Vector2 dodgeDirection;

    private CharacterBody2D player;
    private AnimatedSprite2D playerSprite;


    public Dodge(CharacterBody2D player, AnimatedSprite2D playerSprite)
    {
        this.player = player;
        this.playerSprite = playerSprite;
        this.player = player;
    }

    public void StartDodge(Vector2 direction)
    {
        if (dodgeCooldownTimer <= 0)
        {
            isDodging = true;
            currentDodgeCooldownTimer = dodgeTime; 
            dodgeCooldownTimer = dodgeCooldown;
            dodgeDirection = direction.Normalized();
            player.Velocity = dodgeDirection * dodgeSpeed;
            HandleDodgeAnimation(player.Velocity);
        }
    }

    public void UpdateDodge(float delta)
    {
        if (isDodging)
        {
            currentDodgeCooldownTimer -= delta;

            Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
            
            if (inputDirection.Dot(dodgeDirection) < 0) 
            {
                inputDirection = Vector2.Zero;
            }

            Vector2 adjustedDirection = dodgeDirection + inputDirection * 0.35f; 
            player.Velocity = adjustedDirection.Normalized() * dodgeSpeed;

            if (currentDodgeCooldownTimer <= 0)
            {
                EndDodge();
            }
        }

        if (dodgeCooldownTimer > 0)
        {
            dodgeCooldownTimer -= delta;
        }
    }

    public void HandleDodgeAnimation(Vector2 currentVelocity)
    {
        float movementAngle = player.Velocity.Angle();
        string direction = GetDirectionFromAngle(movementAngle);

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
            string animationName = "gunner_dodge_" + 
                (isLeftOrRight ? "downright" : 
                direction == "downleft" ? "downright" : 
                direction == "upleft" ? "upright" : direction);
            playerSprite.Play(animationName);
        }
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

    private void EndDodge()
    {
        isDodging = false;
        player.Velocity = Vector2.Zero; 
    }

    public bool IsDodging()
    {
        return isDodging;
    }
}
