using Godot;

public partial class Enemy : Node2D
{
    protected float health = 100f; 
    protected float damage = 10f;   
    public Area2D hitbox;

    public override void _Ready()
    {
        GD.Print($"{Name} HP: {health}");
        hitbox = GetNode<Area2D>("Hitbox");
        hitbox.Connect("area_entered", new Callable(this, nameof(OnHitboxAreaEntered)));
    }

    public override void _Process(double delta)
    {
    }

    private void OnHitboxAreaEntered(Area2D area)
    {
        if (area.IsInGroup("PlayerAttack"))
        {
            TakeDamage(damage); 
        }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        GD.Print($"{Name} took damage. Current HP: {health}");
        if (health <= 0)
        {
            GD.Print($"{Name} defeated.");
            QueueFree(); 
        }
    }
}
