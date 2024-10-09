using System;
using Godot;

public partial class Player : CharacterBody2D
{
    private float speed = 300f;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private AnimatedSprite2D PlayerSprite;
    private AnimatedSprite2D AirSpellSprite;
    private AnimatedSprite2D defaultAttackSprite; 
    private Dodge dodge;
    private DefaultAttack defaultAttack;
    private AirSpell airSpell;

    public override void _Ready()
    {
        PlayerSprite = GetNode<AnimatedSprite2D>("PlayerSprite");
        AirSpellSprite = GetNode<AnimatedSprite2D>("AirSpell");
        defaultAttackSprite = GetNode<AnimatedSprite2D>("Default_Attack");
        AirSpellSprite.Visible = false;
        dodge = new Dodge(this, PlayerSprite);
        defaultAttack = new DefaultAttack(this, PlayerSprite, defaultAttackSprite, defaultAttackSprite.GetNode<Area2D>("Hitbox"));
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
            defaultAttack.UpdateAttack((float)delta);
            airSpell.UpdateAttack((float)delta);
        }
        MoveAndSlide();
    }

    private void handleInput()
    {
        currentVelocity = Input.GetVector("left", "right", "up", "down");
        currentVelocity *= speed;
        if (!defaultAttack.Windup() && currentVelocity != Vector2.Zero)
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

    if (currentVelocity == Vector2.Zero)
    {
        string animationName = "gunner_idle_" + 
            (isLeftOrRight ? "downright" : 
            direction == "downleft" ? "downright" : 
            direction == "upleft" ? "upright" : direction);
        PlayerSprite.Play(animationName);
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