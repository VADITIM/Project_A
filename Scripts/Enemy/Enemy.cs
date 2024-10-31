using Godot;

public partial class Enemy : Node2D
{
    protected float health = 100f; 
    protected float damage = 10f;   
    public Area2D hitbox;

    public override void _Ready()
    {
        GD.Print($" 'Enemy' - {Name} HP: {health}");
        hitbox = GetNode<Area2D>("Hitbox");
        hitbox.Connect("area_entered", new Callable(this, nameof(OnAreaEnter)));
    }

    public virtual void OnAreaEnter(Area2D area)
    {
        // if (area.IsInGroup("PlayerAttack"))
        // {
        //     TakeDamage(0);
        // }
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        GD.Print($" 'Enemy' - {Name} took damage. Current HP: {health}");
        if (health <= 0)
        {
            GD.Print($" 'Enemy' - {Name} defeated.");
            QueueFree(); 
        }
    }

}