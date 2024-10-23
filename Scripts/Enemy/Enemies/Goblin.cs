using Godot;

public partial class Goblin : Enemy
{
    private StateMachine stateMachine;
    private AnimatedSprite2D animatedSprite;

    public override void _Ready()
    {
        health = 50f;
        damage = 5f;
        base._Ready();

        stateMachine = GetNode<StateMachine>("State Machine Component");
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);

        stateMachine.Transition("HurtState");
    }

    public bool IsAnimationFinished(string animationName)
    {
        return animatedSprite.Animation == animationName && !animatedSprite.IsPlaying();
    }
}
