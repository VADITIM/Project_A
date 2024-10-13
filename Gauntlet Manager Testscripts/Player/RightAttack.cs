using Godot;

public partial class RightAttack : Node
{
    private bool windup = false;
    private float time = 0.3f;
    private float cooldown = 0.0f;
    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection;

    private AnimatedSprite2D playerSprite;
    private AnimatedSprite2D effectSprite;
    private AnimatedSprite2D rightGauntletSprite;
    private Area2D hitbox;
    private CollisionShape2D hitboxShape;
    private CharacterBody2D player;
    private GauntletManager gauntletManager;

    public RightAttack(CharacterBody2D player, AnimatedSprite2D playerSprite, AnimatedSprite2D effectSprite, Area2D hitbox, AnimatedSprite2D rightGauntletSprite, GauntletManager gauntletManager)
    {
        this.player = player;
        this.playerSprite = playerSprite;
        this.effectSprite = effectSprite;
        this.hitbox = hitbox;
        this.rightGauntletSprite = rightGauntletSprite;
        this.gauntletManager = gauntletManager;

        hitboxShape = hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
        hitbox.AreaEntered += OnAreaEntered;
        hitbox.Monitoring = false;
    }

    public void UpdateAttack(float delta)
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
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("rightAttack"))
            {
                GD.Print("Reft attack");
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
        Vector2 gauntletPosition = rightGauntletSprite.GlobalPosition;
        Vector2 effectOffset = attackDirection * 25;
        bool isMouseOnLeft = attackDirection.X < 0;
        float angleToMouse = attackDirection.Angle();
        string direction = isMouseOnLeft ? "flipped" : "";

        effectSprite.FlipH = isMouseOnLeft;
        effectSprite.GlobalPosition = gauntletPosition + effectOffset;
        effectSprite.Rotation = angleToMouse;
        effectSprite.Stop();
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

    public AnimatedSprite2D GetGauntletSprite()
    {
        return rightGauntletSprite;
    }
}
