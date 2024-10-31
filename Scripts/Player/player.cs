using Godot;
using System.Collections.Generic;

public partial class Player : CharacterBody2D
{
    private float speed = 200f;
    public Vector2 currentVelocity;
    public Vector2 currentDirection;
    
    public AnimatedSprite2D PlayerSprite;

    public Dodge dodge;
    private LeftAttack leftAttack;
    private RightAttack rightAttack;

    private GauntletManager gauntletManager;
    private GauntletInventory gauntletInventory;

    public override void _Ready()
    {
        PlayerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
        dodge = new Dodge(this, PlayerSprite);
        gauntletManager = GetNode<GauntletManager>("GauntletManager");
        gauntletInventory = GetNode<GauntletInventory>("GauntletInventory");

        gauntletManager.EquipGauntlet("left", "Air");
        gauntletManager.EquipGauntlet("right", "Fire");

        leftAttack = gauntletManager.GetLeftAttack();
        rightAttack = gauntletManager.GetRightAttack();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        dodge.UpdateDodge((float)delta);

        if (!dodge.IsDodging())
        {
            handleInput();
            handleAnimation();
            leftAttack?.UpdateAttack((float)delta, "leftAttack");
            rightAttack?.UpdateAttack((float)delta, "rightAttack");
        }

        MoveAndSlide();
    }

    private void handleInput()
    {
        currentVelocity = Input.GetVector("left", "right", "up", "down") * speed;

        if (Input.IsActionJustPressed("dodge"))
        {
            dodge.StartDodge(currentVelocity);
        }
        else
        {
            Velocity = currentVelocity;
        }

        if (!dodge.IsDodging())
        {
            if (Input.IsActionJustPressed("leftAttack"))
            {
                leftAttack?.StartAttack();
            }

            if (Input.IsActionJustPressed("rightAttack"))
            {
                rightAttack?.StartAttack();
            }
        }

        if (Input.IsActionJustPressed("changeLeftGauntlet"))
        {
            SwitchLeftGauntlet();
        }

        if (Input.IsActionJustPressed("changeRightGauntlet"))
        {
            SwitchRightGauntlet();
        }
    }

    private void StartAttack()
    {
        leftAttack?.StartAttack();
        rightAttack?.StartAttack();
    }

    public void StopAttack()
    {
        leftAttack?.StopAttack();
        rightAttack?.StopAttack();
    }

    public void SwitchLeftGauntlet()
    {
        string currentGauntlet = gauntletManager.GetGauntletType("left");
        List<string> unlockedGauntlets = gauntletInventory.GetUnlockedGauntlets();
        int currentIndex = unlockedGauntlets.IndexOf(currentGauntlet);
        int nextIndex = (currentIndex + 1) % unlockedGauntlets.Count;
        string nextGauntlet = unlockedGauntlets[nextIndex];
    
        gauntletManager.EquipGauntlet("left", nextGauntlet);
        leftAttack = gauntletManager.GetLeftAttack();
        gauntletManager.UpdateGauntletAnimations("left");
    }

    public void SwitchRightGauntlet()
    {
        string currentGauntlet = gauntletManager.GetGauntletType("right");
        List<string> unlockedGauntlets = gauntletInventory.GetUnlockedGauntlets();
        int currentIndex = unlockedGauntlets.IndexOf(currentGauntlet);
        int nextIndex = (currentIndex + 1) % unlockedGauntlets.Count;
        string nextGauntlet = unlockedGauntlets[nextIndex];
    
        gauntletManager.EquipGauntlet("right", nextGauntlet);
        rightAttack = gauntletManager.GetRightAttack();
        gauntletManager.UpdateGauntletAnimations("right");
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
        PlayerSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");

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

        if (dodge.IsDodging())
        {
            PlayDodgeAnimation(currentVelocity);
        }

    }

    public void PlayGauntletIdleAnimations(string direction)
    {
        bool isLeftOrRight = (direction == "left" || direction == "right");
    
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