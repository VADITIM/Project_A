using Godot;

public partial class HurtState : State
{
    private Goblin goblin;
    private AnimatedSprite2D sprite;

    public override void Enter()
    {
        base.Enter();
        goblin = (Goblin)stateMachine.GetParent();  // Get the Goblin instance from the State Machine's parent
        sprite = goblin.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        sprite.Play("hurt");
        GD.Print("Goblin is hurt");
    }

    public override void Update(float delta)
    {
        // Check if hurt animation finished, then transition to Idle
        if (sprite.IsPlaying() && sprite.Animation == "hurt" && sprite.Frame == sprite.SpriteFrames.GetFrameCount("hurt") - 1)
        {
            stateMachine.Transition("Idle");
        }
    }

    public override void Exit()
    {
        base.Exit();
        GD.Print("Exiting Hurt State");
    }
}
