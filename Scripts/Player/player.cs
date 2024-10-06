using System;
using Godot;

public partial class player : CharacterBody2D
{
    private float speed = 300f;
    private Vector2 currentVelocity;
    private Vector2 currentDirection;
    private Vector2 attackDirection; 

    private AnimatedSprite2D animatedSprite;
    private AnimatedSprite2D fireSpell;
    private AnimatedSprite2D iceSpell;


    private dodge dodge;
    private attack attack;
    private FireSpell FireSpell;
    private IceSpell IceSpell;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        fireSpell = GetNode<AnimatedSprite2D>("FireSpell");
        iceSpell = GetNode<AnimatedSprite2D>("IceSpell");

        fireSpell.Visible = false;
        iceSpell.Visible = false;

        dodge = new dodge(this);
        attack = new attack(this);
        FireSpell = new FireSpell(this);
        IceSpell = new IceSpell(this);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        dodge.UpdateDodge((float)delta);
        if (!dodge.IsDodging())
        {
            handleInput();
            handleAnimation();

            attack.UpdateAttack((float)delta);
            FireSpell.UpdateAttack((float)delta);
            IceSpell.UpdateAttack((float)delta);
        }
        MoveAndSlide();
    }

    #region Input
    private void handleInput()
    {
        currentVelocity = Input.GetVector("left", "right", "up", "down");
        currentVelocity *= speed;

        if (!attack.MeleeWindup() && currentVelocity != Vector2.Zero)
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
    #endregion

    public void handleAnimation()
    {
        Vector2 directionToMouse = GetGlobalMousePosition() - GlobalPosition;

        #region Idle/Movement
        if (currentVelocity == Vector2.Zero)
        {
            if (Math.Abs(directionToMouse.X) > Math.Abs(directionToMouse.Y))
            {
                if (directionToMouse.X > 0)
                {
                    animatedSprite.Play("idle_right");
                    animatedSprite.FlipH = false;

                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
                else
                {
                    animatedSprite.Play("idle_right");
                    animatedSprite.FlipH = true;

                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
            }
            else
            {
                if (directionToMouse.Y > 0)
                {
                    animatedSprite.Play("idle_down");

                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
                else
                {
                    animatedSprite.Play("idle_up");

                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
            }
        }
        else
        {
            // Movement animation based on mouse position
            if (Math.Abs(directionToMouse.X) > Math.Abs(directionToMouse.Y))
            {
                if (directionToMouse.X > 0)
                {
                    animatedSprite.Play("move_right");
                    animatedSprite.FlipH = false;
                    
                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
                else
                {
                    animatedSprite.Play("move_right");
                    animatedSprite.FlipH = true;
                    
                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
            }
            else
            {
                if (directionToMouse.Y > 0)
                {
                    animatedSprite.Play("move_down");
                    
                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
                else
                {
                    animatedSprite.Play("move_up");
                    
                    fireSpell.Visible = false;
                    iceSpell.Visible = false;
                }
            }
        }
        #endregion


        #region FireSpell
        if (FireSpell.SpellBurnWindup())
        {
            Vector2 direction = FireSpell.GetAttackDirection();
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                {
                    fireSpell.Play("default");
                    fireSpell.Visible = true;
                    fireSpell.Position = new Vector2(40, 0);
                }
                else
                {
                    fireSpell.Play("default");
                    fireSpell.Visible = true;
                    fireSpell.Position = new Vector2(-40, 0);
                }
            }
            else
            {
                if (direction.Y > 0)
                {
                    fireSpell.Play("default");
                    fireSpell.Visible = true;
                    fireSpell.Position = new Vector2(0, 40);

                }
                else
                {
                    fireSpell.Play("default");
                    fireSpell.Visible = true;
                    fireSpell.Position = new Vector2(0, -40);

                }
            }
            return; // Exit, don't process movement/idle animations during an attack
        }
        #endregion

        #region IceSpell
        if (IceSpell.IceSpellWindup())
        {
            Vector2 direction = IceSpell.GetAttackDirection();
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                {
                    iceSpell.Play("default");
                    iceSpell.Visible = true;
                    iceSpell.Position = new Vector2(50, 0);
                    iceSpell.FlipH = true;
                    iceSpell.FlipV = false;
                    iceSpell.Rotation = 30;
                }
                else
                {
                    iceSpell.Play("default");
                    iceSpell.Visible = true;
                    iceSpell.Position = new Vector2(-50, 0);
                    iceSpell.FlipH = false;
                    iceSpell.FlipV = false;
                    iceSpell.Rotation = -30;
                }
            }
            else
            {
                if (direction.Y > 0)
                {
                    iceSpell.Play("default");
                    iceSpell.Visible = true;
                    iceSpell.Rotation = 0;
                    iceSpell.Position = new Vector2(0, 55);
                    iceSpell.FlipH = false;
                    iceSpell.FlipV = false;

                }
                else
                {
                    iceSpell.Play("default");
                    iceSpell.Visible = true;
                    iceSpell.Rotation = 0;
                    iceSpell.Position = new Vector2(0, -55);
                    iceSpell.FlipH = false;
                    iceSpell.FlipV = true;
                }
            }
            return; // Exit, don't process movement/idle animations during an attack
        }
        #endregion


        #region MeleeAttack
        if (attack.MeleeWindup())
        {
            // During attack, use locked attackDirection
            Vector2 direction = attack.GetAttackDirection();


            // Determine attack animation based on locked direction
            if (Math.Abs(direction.X) > Math.Abs(direction.Y))
            {
                if (direction.X > 0)
                {
                    animatedSprite.Play("attack_right");
                    animatedSprite.FlipH = false;
                    fireSpell.Play("default");
                }
                else
                {
                    animatedSprite.Play("attack_right");
                    animatedSprite.FlipH = true;
                    fireSpell.Play("default");
                }
            }
            else
            {
                if (direction.Y > 0)
                {
                    animatedSprite.Play("attack_down");
                    animatedSprite.FlipH = false;
                    fireSpell.Visible = false;
                }
                else
                {
                    animatedSprite.Play("attack_up");
                    animatedSprite.FlipH = false;
                    fireSpell.Visible = false;
                }
            }
            return; // Exit, don't process movement/idle animations during an attack
        }
        #endregion

    }
}
