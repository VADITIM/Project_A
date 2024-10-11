using Godot;

public partial class Player : CharacterBody2D
{
    private float speed = 300f;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private AnimatedSprite2D PlayerSprite;
    private AnimatedSprite2D AirSpellSprite;
    private AnimatedSprite2D leftAttackSprite; 
    private AnimatedSprite2D rightAttackSprite;
    private AnimatedSprite2D gauntletSprite;
    private AnimatedSprite2D leftGauntletSprite;
    private AnimatedSprite2D rightGauntletSprite;
    private Dodge dodge;
    private LeftAttack leftAttack;  // Ensure leftAttack is declared here
    private RightAttack rightAttack;
    private AirSpell airSpell;
    private Vector2 lookDirection;
    private GauntletManager gauntletManager;

    public override void _Ready()
    {
        PlayerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
        AirSpellSprite = GetNode<AnimatedSprite2D>("AirSpell");
        AirSpellSprite.Visible = false;
        leftAttackSprite = GetNode<AnimatedSprite2D>("Left_Attack");
        rightAttackSprite = GetNode<AnimatedSprite2D>("Right_Attack");
        leftGauntletSprite = GetNode<AnimatedSprite2D>("Gauntlet_Left");
        leftGauntletSprite.Visible = true;
        rightGauntletSprite = GetNode<AnimatedSprite2D>("Gauntlet_Right");
        rightGauntletSprite.Visible = true;

        dodge = new Dodge(this, PlayerSprite);
        airSpell = new AirSpell(this, AirSpellSprite);

        gauntletManager = new GauntletManager();
        gauntletManager.InitGauntlets(leftGauntletSprite, rightGauntletSprite, leftAttackSprite, rightAttackSprite);

        gauntletManager.EquipGauntlet(GauntletManager.GauntletType.Air, false); // Left gauntlet
        gauntletManager.EquipGauntlet(GauntletManager.GauntletType.Fire, true);  // Right gauntlet

        leftAttack = new LeftAttack(this, PlayerSprite, leftAttackSprite, leftAttackSprite.GetNode<Area2D>("Hitbox"), leftGauntletSprite, gauntletManager);
        rightAttack = new RightAttack(this, PlayerSprite, rightAttackSprite, rightAttackSprite.GetNode<Area2D>("Hitbox"), rightGauntletSprite, gauntletManager);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        dodge.UpdateDodge((float)delta);
        if (!dodge.IsDodging())
        {
            handleInput();
            handleAnimation();
            leftAttack.UpdateAttack((float)delta);
            rightAttack.UpdateAttack((float)delta);
            airSpell.UpdateAttack((float)delta);
        }
        MoveAndSlide();
    }

    public void ChangeGauntlet(GauntletManager.GauntletType newType, bool isRightHand)
    {
        gauntletManager.EquipGauntlet(newType, isRightHand);
        if (isRightHand)
        {
            GD.Print("Right gauntlet changed to: " + newType);
        }
        else
        {
            GD.Print("Left gauntlet changed to: " + newType);
        }
    }

    private void handleInput()
    {
        currentVelocity = Input.GetVector("left", "right", "up", "down");
        currentVelocity *= speed;
        if (!leftAttack.Windup() && currentVelocity != Vector2.Zero)
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
    
    private void handleAnimation()
    {
        Animation(currentVelocity);
    }
    
    private void Animation(Vector2 currentVelocity)
    {
        Vector2 directionToMouse = GetGlobalMousePosition() - GlobalPosition;
        string direction = GetDirectionFromAngle(directionToMouse.Angle());
        Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
        bool isLeftOrRight = (direction == "left" || direction == "right");
        PlayerSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");
        leftGauntletSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");
        rightGauntletSprite.FlipH = (direction == "downleft" || direction == "upleft" || direction == "left");
    
        if (currentVelocity == Vector2.Zero)
        {
            string animationName = "gunner_idle_" + 
                (isLeftOrRight ? "downright" : 
                direction == "downleft" ? "downright" : 
                direction == "upleft" ? "upright" : direction);
            leftGauntletSprite.Visible = true;
            rightGauntletSprite.Visible = true;
            PlayerSprite.Play(animationName);
    
            string animationLeftGauntlet = gauntletManager.GetGauntletTypeString(gauntletManager.GetLeftGauntletType()) + 
                "idle_" + (isLeftOrRight ? "downright" :
                direction == "downleft" ? "downright" :
                direction == "upleft" ? "upright" : direction);
            leftGauntletSprite.Play(animationLeftGauntlet);
    
            string animationRightGauntlet = gauntletManager.GetGauntletTypeString(gauntletManager.GetRightGauntletType()) + 
                "idle_" + (isLeftOrRight ? "downright" :
                direction == "downleft" ? "downright" :
                direction == "upleft" ? "upright" : direction);
            rightGauntletSprite.Play(animationRightGauntlet);
        }
        else
        {
            string animationName = "gunner_move_" + 
                (isLeftOrRight ? "downright" : 
                direction == "downleft" ? "downright" : 
                direction == "upleft" ? "upright" : direction);
            leftGauntletSprite.Visible = false;
            rightGauntletSprite.Visible = false;
            PlayerSprite.Play(animationName);
        }
    
        if (leftAttack.Windup())
        {
            string animationGauntlet = gauntletManager.GetAttackAnimation(gauntletManager.GetLeftGauntletType(), false, direction);
            leftGauntletSprite.Visible = true;
            leftGauntletSprite.Play(animationGauntlet);
        }
    
        if (dodge.IsDodging())
        {
            string dodgeDirection = GetDirectionFromAngle(currentVelocity.Angle());
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