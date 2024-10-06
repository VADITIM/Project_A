using Godot;

public partial class FireSpell : Node
{
	private bool fireSpellWindup = false;
    private float fireSpellTime = .5f;
    private float fireSpellCooldown = 1f;

    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection; 

	private CharacterBody2D player;

	public FireSpell(CharacterBody2D player)
	{
		this.player = player;
	}

	public void UpdateAttack(float delta)
	{
		SpellBurnAttack(delta);
	}

    public void SpellBurnAttack(float delta)
    {
        if (fireSpellWindup)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                fireSpellWindup = false;
                attackCooldownTimer = fireSpellCooldown;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("spell1"))
            {
                StartAttack();
            }
        }
    }

    private void StartAttack()
    {
        fireSpellWindup = true;
        currentAttackCooldownTimer = fireSpellTime;

        Vector2 directionToMouse = player.GetGlobalMousePosition() - player.GlobalPosition;
        attackDirection = directionToMouse.Normalized(); 
        GD.Print("Spell burning in direction: " + attackDirection);
    }

    public bool SpellBurnWindup()
    {
        return fireSpellWindup;
    }
	
    public Vector2 GetAttackDirection()
    {
        return attackDirection; 
    }
}
