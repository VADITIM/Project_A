using Godot;

public partial class RightAttack : Node
{
    private bool windup = false;
    private float time = 0.3f;
    private float cooldown = 0.0f;

    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection;

    private CharacterBody2D player;
    private AnimatedSprite2D playerSprite;
    private AnimatedSprite2D effectSprite;
    private AnimatedSprite2D gauntletSprite;
    private Area2D hitbox; 

    public RightAttack(CharacterBody2D player, AnimatedSprite2D playerSprite, AnimatedSprite2D effectSprite, Area2D hitbox)
    {
        this.player = player;
        this.playerSprite = playerSprite;
        this.effectSprite = effectSprite;
        this.hitbox = hitbox;

        hitbox.AreaEntered += OnAreaEntered;

        hitbox.Monitoring = false;
    }

    public void UpdateAttack(float delta)
    {
        Attack(delta);
    }

    public void Attack(float delta)
    {
        if (windup)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                windup = false;
                attackCooldownTimer = cooldown;
                effectSprite.Visible = false;

                hitbox.Monitoring = false;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("meleeAttack"))
            {
                StartAttack();
            }
        }
    }

    private void StartAttack()
    {
        windup = true;
        currentAttackCooldownTimer = time;

        Vector2 directionToMouse = player.GetGlobalMousePosition() - player.GlobalPosition;
        attackDirection = directionToMouse.Normalized();

        effectSprite.Visible = true;

        hitbox.Monitoring = true;

        PlayAttackAnimations();
    }

    private void PlayAttackAnimations()
    {
        Vector2 effectOffset = attackDirection * 30; 
        bool isMouseOnLeft = attackDirection.X < 0;
        float angleToMouse = attackDirection.Angle();

        string effectAnimation = isMouseOnLeft ? "air_attack_1_flipped" : "air_attack_1";
        effectSprite.FlipH = isMouseOnLeft;
        effectSprite.GlobalPosition = player.GlobalPosition + effectOffset;
        effectSprite.Rotation = angleToMouse;
        effectSprite.Stop();
        effectSprite.Play(effectAnimation);
        effectSprite.Frame = 0;
    }

    private void OnAreaEntered(Area2D other)
    {
        if (other is Area2D target)
        {
            GD.Print("Hit: " + target.Name);
        }
    }

    public Vector2 GetAttackDirection()
    {
        return attackDirection;
    }

    public bool Windup()
    {
        return windup;
    }
}
