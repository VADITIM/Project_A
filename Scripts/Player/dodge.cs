using Godot;

public partial class Dodge : Node
{
    private bool isDodging = false;
    private float dodgeSpeed = 450f;
    private float dodgeTime = .42f;   
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
