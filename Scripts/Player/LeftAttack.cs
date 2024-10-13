using Godot;

public partial class LeftAttack : Node
{
    private bool windup = false;
    private float time = 0.3f;
    private float cooldown = 0.0f;
    private float attackCooldownTimer = 0f;
    private float currentAttackCooldownTimer = 0f;
    private Vector2 attackDirection;

    private AnimatedSprite2D playerSprite;
    private AnimatedSprite2D effectSprite;
    private AnimatedSprite2D leftGauntletSprite;
    private Area2D hitbox;
    private CollisionShape2D hitboxShape;
    private CharacterBody2D player;

    private GauntletManager gauntletManager;

    public LeftAttack(CharacterBody2D player, AnimatedSprite2D playerSprite, AnimatedSprite2D effectSprite, Area2D hitbox, AnimatedSprite2D leftGauntletSprite, GauntletManager gauntletManager)
    {
        this.player = player;
        this.playerSprite = playerSprite;
        this.effectSprite = effectSprite;
        this.hitbox = hitbox;
        this.leftGauntletSprite = leftGauntletSprite;
        this.gauntletManager = gauntletManager;

        hitboxShape = hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
        hitbox.AreaEntered += OnAreaEntered;
        hitbox.Monitoring = false;

        leftGauntletSprite.ZIndex = 3;
        effectSprite.ZIndex = 2;
        playerSprite.ZIndex = 3;
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
                effectSprite.Stop();
                hitbox.Monitoring = false;
            effectSprite.Visible = false;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed("leftAttack"))
            {
                GD.Print("Left attack");
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
        UpdateEffectSpriteZIndex();
    }

    private void PlayAttackAnimations()
    {
        Vector2 gauntletPosition = leftGauntletSprite.GlobalPosition;
        Vector2 effectOffset = attackDirection * 25;
        bool isMouseOnLeft = attackDirection.X < 0;
        float angleToMouse = attackDirection.Angle();
        string direction = isMouseOnLeft ? "1_flipped" : "1";

        effectSprite.FlipH = isMouseOnLeft;
        effectSprite.GlobalPosition = gauntletPosition + effectOffset;
        effectSprite.Rotation = angleToMouse;

        string baseAttackAnimation = gauntletManager.GetCurrentGauntletAttackAnimation("left");
        string leftGauntletAttackAnimation = $"{baseAttackAnimation}_{direction}";

        effectSprite.Play(leftGauntletAttackAnimation);

        GD.Print("Playing attack animation: " + leftGauntletAttackAnimation);
    }

    private void OnAreaEntered(Area2D other)
    {
        if (other is Area2D target)
        {
            GD.Print("Hit: " + target.Name);
        }
    }

    private void UpdateEffectSpriteZIndex()
    {
        if (attackDirection.Y < 0)
        {
            effectSprite.ZIndex = playerSprite.ZIndex - 1;
        }
        else
        {
            effectSprite.ZIndex = playerSprite.ZIndex + 1;
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
        return leftGauntletSprite;
    }
}
