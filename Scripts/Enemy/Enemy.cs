using Godot;

public partial class Enemy : Node2D
{
    protected float health = 0; 
    protected float damage = 0;   

    protected StateMachine stateMachine;
    public Area2D hitbox;
    public AnimatedSprite2D animatedSprite;

    private Timer hurtTimer;

    public override void _Ready()
    {
        GD.Print($" 'Enemy' - {Name} HP: {health}");
        hitbox = GetNode<Area2D>("Hitbox");
        hitbox.Connect("area_entered", new Callable(this, nameof(OnAreaEnter)));

        hurtTimer = new Timer();
        AddChild(hurtTimer);
        hurtTimer.WaitTime = .3f;
        hurtTimer.OneShot = true;
        hurtTimer.Connect("timeout", new Callable(this, nameof(OnHurtTimerTimeout)));
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
        animatedSprite.Play("hurt");
        
        GD.Print($" 'Enemy' - {Name} took damage. Current HP: {health}");
        if (health <= 0)
        {
            GD.Print($" 'Enemy' - {Name} defeated.");
            QueueFree(); 
        } hurtTimer.Start();
    }

    private void OnHurtTimerTimeout()
    {
        animatedSprite.Play("idle");
    }

}