using Godot;

public abstract partial class Spell : Node
{
    protected CharacterBody2D player;
    protected float cooldown;
    protected float duration;
    protected bool isActive;
    protected Vector2 direction;

    public Spell(CharacterBody2D player, float cooldown, float duration)
    {
        this.player = player;
        this.cooldown = cooldown;
        this.duration = duration;
        isActive = false;
    }

    public abstract void Activate();
    public abstract void Deactivate();
    public abstract void Update(float delta);

    public bool IsActive() => isActive;
}