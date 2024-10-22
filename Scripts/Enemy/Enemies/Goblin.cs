using Godot;

public partial class Goblin : Enemy
{
    public override void _Ready()
    {
        health = 50f;
        damage = 5f; 
        
        base._Ready();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
