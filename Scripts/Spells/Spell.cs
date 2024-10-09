using Godot;

public abstract partial class Spell : Node
{
    protected CharacterBody2D player;
    protected bool windup = false;
    protected float time;
    protected float cooldown;

    protected float attackCooldownTimer;
    protected float currentAttackCooldownTimer;
    protected Vector2 attackdirection;

    public Spell(CharacterBody2D player, float cooldown, float time)
    {
        this.player = player;
        this.cooldown = cooldown;
        this.time = time;
        windup = false;
    }

    public abstract void Activate();
    public abstract void Deactivate();
    public abstract void Update(float delta);
}