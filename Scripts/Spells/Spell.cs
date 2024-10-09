using Godot;

public abstract partial class Spell : Node
{
    protected bool windup = false;
    protected float time = 1.55f; 
    protected float cooldown = 1f;

    protected float attackCooldownTimer = 0f;
    protected float currentAttackCooldownTimer = 0f;
    protected Vector2 attackDirection;

    protected CharacterBody2D player;
    protected AnimatedSprite2D effectSprite;

    public Spell(CharacterBody2D player, AnimatedSprite2D effectSprite)
    {
        this.player = player;
        this.effectSprite = effectSprite;
        this.effectSprite.Visible = false;
    }

    public virtual void UpdateAttack(float delta)
    {
        if (windup)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                windup = false;
                attackCooldownTimer = cooldown;
                effectSprite.Visible = false;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("spell1"))
            {
                StartAttack();
            }
        }
    }

    protected virtual void StartAttack()
    {
        windup = true;
        currentAttackCooldownTimer = time;
        Vector2 directionToMouse = player.GetGlobalMousePosition() - player.GlobalPosition;
        attackDirection = directionToMouse.Normalized();
        GD.Print("Spell attacking in direction: " + attackDirection);
        effectSprite.Visible = true;

        UpdateAnimation();
    }

    protected abstract void UpdateAnimation(); 

    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }
}
