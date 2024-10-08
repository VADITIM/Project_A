using Godot;

public partial class SpellIce : Node
{
	private bool iceSpellWindup = false;
    private float iceSpellTime = .5f;
    private float iceSpellCooldown = 1f;

    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection; 

	private CharacterBody2D player;

	public SpellIce(CharacterBody2D player)
	{
		this.player = player;
	}

	public void UpdateAttack(float delta)
	{
		AirSpellAttack(delta);
	}

    public void AirSpellAttack(float delta)
    {
        if (iceSpellWindup)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                iceSpellWindup = false;
                attackCooldownTimer = iceSpellCooldown;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("spell2"))
            {
                StartAttack();
            }
        }
    }

    private void StartAttack()
    {
        iceSpellWindup = true;
        currentAttackCooldownTimer = iceSpellTime;

        Vector2 directionToMouse = player.GetGlobalMousePosition() - player.GlobalPosition;
        attackDirection = directionToMouse.Normalized();
        GD.Print("Spell burning in direction: " + attackDirection);
    }


    public bool IceSpellWindup()
    {
        return iceSpellWindup;
    }

	
    public Vector2 GetAttackDirection()
    {
        return attackDirection; 
    }
}
