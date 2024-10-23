using Godot;

public partial class HurtState : State
{

	private Goblin goblin;
	private AnimatedSprite2D sprite;
	private Node2D hurt;
	
    public override void Enter()
    {
        base.Enter();
        var goblin = (Goblin)stateMachine.GetParent();
        sprite = goblin.GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		sprite.Play("hurt");
		hurt = goblin.GetNode<Node2D>("Hurt");
		hurt.Visible = false;
		GD.Print("Goblin is hurt");
    }

    public override void Update(float delta)
    {
        base.Update(delta);
        
        if (goblin.IsAnimationFinished("hurt"))
        {
            stateMachine.Transition("Idle");
        }
    }

    public override void Exit()
    {
    }
}