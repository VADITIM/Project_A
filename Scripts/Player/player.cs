using Godot;

public partial class Player : CharacterBody2D
{
    private float speed = 300f;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private AnimatedSprite2D PlayerSprite;
    private AnimatedSprite2D AirSpellSprite;
    private AnimatedSprite2D leftAttackSprite; 

    private AnimatedSprite2D gauntletSprite;
    private AnimatedSprite2D leftGauntletSprite;
    private AnimatedSprite2D rightGauntletSprite;

    private Dodge dodge;
    private LeftAttack leftAttack;
    private AirSpell airSpell;
    private Vector2 lookDirection;

    public override void _Ready()
    {
        PlayerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
        AirSpellSprite = GetNode<AnimatedSprite2D>("AirSpell");
        AirSpellSprite.Visible = false;
        leftAttackSprite = GetNode<AnimatedSprite2D>("Left_Attack");
        gauntletSprite = GetNode<AnimatedSprite2D>("Gauntlet");
        gauntletSprite.Visible = false;
        leftGauntletSprite = GetNode<AnimatedSprite2D>("Gauntlet_Left");
        leftGauntletSprite.Visible = false;
        rightGauntletSprite = GetNode<AnimatedSprite2D>("Gauntlet_Right");
        rightGauntletSprite.Visible = false;
        
        dodge = new Dodge(this, PlayerSprite);
        leftAttack = new LeftAttack(this, PlayerSprite, leftAttackSprite, leftAttackSprite.GetNode<Area2D>("Hitbox"));
        airSpell = new AirSpell(this, AirSpellSprite);
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
            airSpell.UpdateAttack((float)delta);
        }
        MoveAndSlide();
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
        PlayerSprite.Play(animationName);

        string animationLeftGauntlet = "gauntlet_idle_" +
            (isLeftOrRight ? "downright" :
            direction == "downleft" ? "downright" :
            direction == "upleft" ? "upright" : direction);
        leftGauntletSprite.Visible = true;
        leftGauntletSprite.Play(animationLeftGauntlet);    

        string animationRightGauntlet = "gauntlet_idle_" +
            (isLeftOrRight ? "downright" :
            direction == "downleft" ? "downright" :
            direction == "upleft" ? "upright" : direction);
        rightGauntletSprite.Visible = true;
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
        string animationGauntlet = "gauntlet_attack_" +
            (isLeftOrRight ? "downright" :
            direction == "downleft" ? "downright" :
            direction == "upleft" ? "upright" : direction);
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
}