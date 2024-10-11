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
    }

    private void PlayAttackAnimations()
    {
        Vector2 gauntletPosition = leftGauntletSprite.GlobalPosition;
        Vector2 effectOffset = attackDirection * 25; 

        bool isMouseOnLeft = attackDirection.X < 0;
        float angleToMouse = attackDirection.Angle();

        string direction = isMouseOnLeft ? "flipped" : "";
        
        GauntletManager.GauntletType gauntletType = gauntletManager.GetLeftGauntletType();
        string effectAnimation = gauntletManager.GetAttackAnimation(gauntletType, true, direction);

        effectSprite.FlipH = isMouseOnLeft;
        effectSprite.GlobalPosition = gauntletPosition + effectOffset;
        effectSprite.Rotation = angleToMouse;
        effectSprite.Stop();
        effectSprite.Play(effectAnimation);
        effectSprite.Frame = 0;
        GD.Print("Playing animation: " + effectAnimation);
    }

    private void UpdateHitbox(string animationName)
    {
        switch (animationName)
        {
            case "fire_attack_1":
                hitboxShape.Shape = new RectangleShape2D { Size = new Vector2(45, 11) };
                hitbox.Position = new Vector2(30, 0);
                break;
            case "air_attack_1":
                hitboxShape.Shape = new RectangleShape2D { Size = new Vector2(22, 17) };
                hitbox.Position = new Vector2(0, 30);
                break;
        }
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
