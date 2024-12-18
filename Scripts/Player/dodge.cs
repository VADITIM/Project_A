using Godot;

public partial class Dodge : Node
{
    private bool isDodging = false;
    private float dodgeSpeed = 350f;
    private float dodgeTime = .40f;   
    private float dodgeCooldown = .6f; 
    private float dodgeCooldownTimer = 0f;   
    private float currentDodgeCooldownTimer;   
    private Vector2 dodgeDirection;

    private Player player;
    private AnimatedSprite2D playerSprite;

    public Dodge(Player player, AnimatedSprite2D playerSprite)
    {
        this.player = player;
        this.playerSprite = playerSprite;
    }

    public void StartDodge(Vector2 direction)
    {
        if (dodgeCooldownTimer <= 0 && player.Velocity.Length() > 0)
        {
            isDodging = true;
            player.StopAttack();
            currentDodgeCooldownTimer = dodgeTime; 
            dodgeCooldownTimer = dodgeCooldown;
            dodgeDirection = direction.Normalized();
            player.Velocity = dodgeDirection * dodgeSpeed;
        }
    }

    // Method in which the direction can be slightly adjusted during the dodge
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

            Vector2 adjustedDirection = dodgeDirection + inputDirection * 0.25f; 
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

    // Method in which the direction cannot be adjusted during the dodge
    // public void UpdateDodge(float delta)
    // {
    //     if (isDodging)
    //     {
    //         currentDodgeCooldownTimer -= delta;
    
    //         if (currentDodgeCooldownTimer <= 0f)
    //         {
    //             isDodging = false;
    //             player.Velocity = Vector2.Zero;
    //         }
    //         else
    //         {
    //             player.Velocity = dodgeDirection * dodgeSpeed;
    //         }
    //     }
    
    //     if (dodgeCooldownTimer > 0f)
    //     {
    //         dodgeCooldownTimer -= delta;
    //     }
    // }

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
