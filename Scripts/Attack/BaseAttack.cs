using Godot;

public partial class BaseAttack : Node
{
    protected bool windup = false;
    protected float time = 0.3f;
    protected float cooldown = 0.0f;
    protected float attackCooldownTimer = 0f;
    protected float currentAttackCooldownTimer = 0f;
    protected Vector2 attackDirection;

    protected AnimatedSprite2D playerSprite;
    protected AnimatedSprite2D effectSprite;
    protected AnimatedSprite2D gauntletSprite; 
    protected Area2D hitbox;
    protected CollisionShape2D hitboxShape;
    protected CharacterBody2D player;

    protected GauntletManager gauntletManager;
    private Enemy enemy;

    public BaseAttack(CharacterBody2D player, AnimatedSprite2D playerSprite, AnimatedSprite2D effectSprite, Area2D hitbox, AnimatedSprite2D gauntletSprite, GauntletManager gauntletManager)
    {
        this.player = player;
        this.playerSprite = playerSprite;
        this.effectSprite = effectSprite;
        this.hitbox = hitbox;
        this.gauntletSprite = gauntletSprite;
        this.gauntletManager = gauntletManager;

        hitboxShape = hitbox.GetNode<CollisionShape2D>("CollisionShape2D");
        hitbox.Connect("area_entered", new Callable(this, nameof(AreaEnter)));
        hitbox.Monitoring = false;

        gauntletSprite.ZIndex = 3;
        effectSprite.ZIndex = 2;
        playerSprite.ZIndex = 3;
    }

    public override void _Ready()
    {
        enemy = GetNode<Enemy>("/root/Main/Enemy");
    }

    public virtual void UpdateAttack(float delta, string inputAction)
    {
        if (windup)
        {
            currentAttackCooldownTimer -= delta;
            if (currentAttackCooldownTimer <= 0)
            {
                windup = false;
                attackCooldownTimer = cooldown;
                effectSprite.Stop();
                effectSprite.Visible = false;
                hitbox.Monitoring = false;
            }
        }
        else
        {
            attackCooldownTimer -= delta;
            if (attackCooldownTimer <= 0 && Input.IsActionJustPressed(inputAction))
            {
                StartAttack();
            }
        }
    }

    public virtual void StartAttack()
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

    protected virtual void PlayAttackAnimations()
    {
        Vector2 gauntletPosition = gauntletSprite.GlobalPosition;
        float effectOffset = gauntletManager.GetAttackOffset(gauntletManager.GetGauntletType(GetGauntletType()));
        Vector2 offsetVector = new Vector2(effectOffset, effectOffset) * attackDirection;
        
        bool isMouseOnLeft = attackDirection.X < 0;
        float angleToMouse = attackDirection.Angle();
        string direction = isMouseOnLeft ? "1_flipped" : "1";

        effectSprite.FlipH = isMouseOnLeft;
        effectSprite.GlobalPosition = gauntletPosition + offsetVector;
        effectSprite.Rotation = angleToMouse;

        string baseAttackAnimation = gauntletManager.GetCurrentGauntletAttackAnimation(GetGauntletType());
        string attackAnimation = $"{baseAttackAnimation}_{direction}";

        effectSprite.Play(attackAnimation);
    }

    protected virtual string GetGauntletType()
    {
        return "right";  
    }

    protected void UpdateEffectSpriteZIndex()
    {
        effectSprite.ZIndex = attackDirection.Y < 0 ? playerSprite.ZIndex - 1 : playerSprite.ZIndex + 1;
    }

    public void StopAttack()
    {
        windup = false;
        effectSprite.Stop();
        effectSprite.Visible = false;
        hitbox.Monitoring = false;
    }

    public AnimatedSprite2D GetGauntletSprite()
    {
        return gauntletSprite;
    }

    protected virtual void AreaEnter(Area2D other) { }
}
