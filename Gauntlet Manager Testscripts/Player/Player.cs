using Godot;

public partial class Player : CharacterBody2D
{
    private float speed = 300f;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private AnimatedSprite2D PlayerSprite;
    private Dodge dodge;
    private LeftAttack leftAttack;
    private RightAttack rightAttack;
    private Vector2 lookDirection;
    private GauntletManager gauntletManager;
    private Node2D leftGauntlet;
    private Node2D rightGauntlet;

    public override void _Ready()
    {
        PlayerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
        dodge = new Dodge(this, PlayerSprite);
        gauntletManager = GetNode<GauntletManager>("GauntletManager");
        gauntletManager.EquipGauntlet("left", "Air");
        gauntletManager.EquipGauntlet("right", "Fire");
        leftAttack = gauntletManager.GetLeftAttack();
        rightAttack = gauntletManager.GetRightAttack();
        leftGauntlet = GetNode<Node2D>("GauntletManager/LeftGauntlet");
        rightGauntlet = GetNode<Node2D>("GauntletManager/RightGauntlet");

        // Debugging output to verify initialization
        GD.Print("LeftAttack initialized in Player: " + (leftAttack != null));
        GD.Print("RightAttack initialized in Player: " + (rightAttack != null));
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        dodge.UpdateDodge((float)delta);
        if (!dodge.IsDodging())
        {
            handleInput();
            handleAnimation();
            leftAttack?.UpdateAttack((float)delta);
            rightAttack?.UpdateAttack((float)delta);
        }
        MoveAndSlide();

    }

    private void handleInput()
    {
        currentVelocity = Input.GetVector("left", "right", "up", "down");
        currentVelocity *= speed;
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
        if (Input.IsActionJustPressed("changeLeftGauntlet"))
        {
            gauntletManager.EquipGauntlet("left", "Fire");
            gauntletManager.EquipGauntletToHand(leftGauntlet, gauntletManager.FireGauntletScene, false);
            GD.Print("Changed left gauntlet to Fire");
                    string direction = GetDirectionFromAngle(currentDirection.Angle());
            PlayGauntletIdleAnimations(direction);
        }

    }

    private void handleAnimation()
    {
        Animation(currentVelocity);
    }

    private void Animation(Vector2 currentVelocity)
    {
        Vector2 directionToMouse = GetGlobalMousePosition() - GlobalPosition;
        string direction = GetDirectionFromAngle(directionToMouse.Angle());
        string oppositeDirection = GetOppositeDirection(direction);
        Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
        bool isLeftOrRight = (direction == "left" || direction == "right");

        // Determine flipping for the player sprite
        PlayerSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");

        // Handle the player's animation
        if (currentVelocity == Vector2.Zero)
        {
            string animationName = "gunner_idle_" + 
                (isLeftOrRight ? "downright" : 
                direction == "downleft" ? "downright" : 
                direction == "upleft" ? "upright" : direction);
            PlayerSprite.Play(animationName);
            PlayGauntletIdleAnimations(direction);
        }
        else
        {
            string animationName = "gunner_move_" + 
                (isLeftOrRight ? "downright" : 
                direction == "downleft" ? "downright" : 
                direction == "upleft" ? "upright" : direction);
            PlayerSprite.Play(animationName);
        }

        // Handle dodge animations
        if (dodge.IsDodging())
        {
            PlayDodgeAnimation(currentVelocity);
        }

    }

    public void PlayGauntletIdleAnimations(string direction)
    {
        bool isLeftOrRight = (direction == "left" || direction == "right");
        // GD.Print("PlayGauntletIdleAnimations called with direction: " + direction + ", isLeftOrRight: " + isLeftOrRight); // Debugging
    
        if (leftAttack != null)
        {
            AnimatedSprite2D leftGauntletSprite = leftAttack.GetGauntletSprite();
            if (leftGauntletSprite != null) 
            {
                leftGauntletSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");
                string baseIdleAnimation = gauntletManager.GetCurrentGauntletIdleAnimation("left");
                string leftGauntletIdleAnimation = "left_" + baseIdleAnimation + "_" + 
                    (isLeftOrRight ? "downright" : 
                    direction == "downleft" ? "downright" : 
                    direction == "upleft" ? "upright" : direction);
                leftGauntletSprite.Play(leftGauntletIdleAnimation);
                gauntletManager.SetCurrentGauntletIdleAnimation("left", leftGauntletIdleAnimation);
                // GD.Print("Left Gauntlet Animation Playing: " + leftGauntletIdleAnimation); // Debugging
            }
        }
    
        if (rightAttack != null)
        {
            AnimatedSprite2D rightGauntletSprite = rightAttack.GetGauntletSprite();
            if (rightGauntletSprite != null) 
            {
                rightGauntletSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");
                string baseIdleAnimation = gauntletManager.GetCurrentGauntletIdleAnimation("right");
                string rightGauntletIdleAnimation = "right_" + baseIdleAnimation + "_" + 
                    (isLeftOrRight ? "downright" : 
                    direction == "downleft" ? "downright" : 
                    direction == "upleft" ? "upright" : direction);
                rightGauntletSprite.Play(rightGauntletIdleAnimation);
                gauntletManager.SetCurrentGauntletIdleAnimation("right", rightGauntletIdleAnimation);
                // GD.Print("Right Gauntlet Animation Playing: " + rightGauntletIdleAnimation); // Debugging
            }
        }
    }
    private void PlayDodgeAnimation(Vector2 currentVelocity)
    {
        string dodgeDirection = GetDirectionFromAngle(currentVelocity.Angle());
        string oppositeDodgeDirection = GetOppositeDirection(dodgeDirection);
        switch (dodgeDirection)
        {
            case "up":          PlayerSprite.Play("gunner_dodge_up");          PlayerSprite.FlipH = false;      
            break;
            case "right":       PlayerSprite.Play("gunner_dodge_downright");   PlayerSprite.FlipH = false;      
            break;
            case "left":        PlayerSprite.Play("gunner_dodge_downright");   PlayerSprite.FlipH = true;       
            break;
            case "upright":     PlayerSprite.Play("gunner_dodge_upright");     PlayerSprite.FlipH = false;      
            break;
            case "upleft":      PlayerSprite.Play("gunner_dodge_upright");     PlayerSprite.FlipH = true;       
            break;
            case "down":        PlayerSprite.Play("gunner_dodge_down");        PlayerSprite.FlipH = false;      
            break;
            case "downright":   PlayerSprite.Play("gunner_dodge_downright");   PlayerSprite.FlipH = false;      
            break;
            case "downleft":    PlayerSprite.Play("gunner_dodge_downright");   PlayerSprite.FlipH = true;       
            break;
            default:            PlayerSprite.Play("gunner_dodge_downright");   PlayerSprite.FlipH = false;      
            break;
        }

        // Handle gauntlet dodge animations
        if (leftAttack != null)
        {
            AnimatedSprite2D leftGauntletSprite = leftAttack.GetGauntletSprite();
            string leftGauntletDodgeAnimation = "left_air_gauntlet_dodge_" + dodgeDirection;
            leftGauntletSprite.Play(leftGauntletDodgeAnimation);
        }
        if (rightAttack != null)
        {
            AnimatedSprite2D rightGauntletSprite = rightAttack.GetGauntletSprite();
            string rightGauntletDodgeAnimation = "right_air_gauntlet_dodge_" + oppositeDodgeDirection;
            rightGauntletSprite.Play(rightGauntletDodgeAnimation);
        }
    }

    private string GetOppositeDirection(string direction)
    {
        switch (direction)
        {
            case "up": return "down";
            case "down": return "up";
            case "left": return "right";
            case "right": return "left";
            case "upright": return "downleft";
            case "upleft": return "downright";
            case "downright": return "upleft";
            case "downleft": return "upright";
            default: return direction;
        }
    }

    private string GetDirectionFromAngle(float angle)
    {
        // ANIMATIONS ARE SWAPPED FOR SOME REASON IT DOESNT WORK ELSE
        angle = Mathf.PosMod(angle, Mathf.Tau);
        if (angle < Mathf.Pi / 8 || angle >= 15 * Mathf.Pi / 8)
            return "right";
        if (angle < 3 * Mathf.Pi / 8)
            return "downright";
        if (angle < 5 * Mathf.Pi / 8)
            return "down";
        if (angle < 7 * Mathf.Pi / 8)
            return "downleft";
        if (angle < 9 * Mathf.Pi / 8)
            return "left";
        if (angle < 11 * Mathf.Pi / 8)
            return "upleft";
        if (angle < 13 * Mathf.Pi / 8)
            return "up";
        return "upright";
    }
}