using Godot;

public partial class Goblin : Enemy
{
    public override void _Ready()
    {
        health = 50f;
        damage = 5f;
        base._Ready();
    
        stateMachine = GetNode<StateMachine>("State Machine Component");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        GD.Print($" 'Goblin' - {Name} HP: {health}");
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        // stateMachine.Transition("HurtState");
        // animatedSprite.Play("hurt");
        GD.Print($" 'Goblin' - {Name} took damage. Current HP: {health}");
    }
    

    // public bool IsAnimationFinished(string animationName)
    // {
    //     return animatedSprite.Animation == animationName && !animatedSprite.IsPlaying();
    // }
}
