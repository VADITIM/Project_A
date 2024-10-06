using Godot;

public partial class attack : Node
{
    private bool meleeWindup = false;
    private float meleeTime = .7f;
    private float meleeCooldown = .1f;

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
        MeleeAttack(delta);
    }

    public void MeleeAttack(float delta)
    {
        if (meleeWindup)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                meleeWindup = false;
                attackCooldownTimer = meleeCooldown;
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
        meleeWindup = true;
        currentAttackCooldownTimer = meleeTime;

        Vector2 directionToMouse = player.GetGlobalMousePosition() - player.GlobalPosition;
        attackDirection = directionToMouse.Normalized();
        GD.Print("Attacking in direction: " + attackDirection);
    }


    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }

    public bool MeleeWindup()
    {
        return meleeWindup;
    }

}
