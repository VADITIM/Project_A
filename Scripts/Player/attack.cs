using Godot;

public partial class attack : Node
{
    private bool isAttacking = false;
    private float attackTime = .7f;
    private float attackCooldown = .1f;
    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection;

	private CharacterBody2D player;


    public attack(CharacterBody2D player)
    {
        this.player = player;
	}

    public void UpdateAttack(float delta)
    {
        if (isAttacking)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                isAttacking = false;
                attackCooldownTimer = attackCooldown;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("attack"))
            {
                StartAttack();
            }
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        currentAttackCooldownTimer = attackTime;
        attackDirection = GetAttackDirection();
        GD.Print("Attacking in direction: " + attackDirection);
    }

    private Vector2 GetAttackDirection()
    {
        return Input.GetVector("left", "right", "up", "down");
    }

	public bool IsAttacking()
	{
		return isAttacking;
	}
}