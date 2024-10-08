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

        defaultAttack = new DefaultAttack(this, PlayerSprite, defaultAttackSprite);

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
        defaultAttack.HandleAttackAnimation(currentVelocity);
    }
}
