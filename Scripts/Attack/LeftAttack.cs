using Godot;

public partial class LeftAttack : BaseAttack
{
    public LeftAttack(CharacterBody2D player, AnimatedSprite2D playerSprite, AnimatedSprite2D effectSprite, Area2D hitbox, AnimatedSprite2D gauntletSprite, GauntletManager gauntletManager)
        : base(player, playerSprite, effectSprite, hitbox, gauntletSprite, gauntletManager)
    {
    }

    protected override string GetGauntletType()
    {
        return "left";
    }

    public void Update(float delta)
    {
        UpdateAttack(delta, "leftAttack"); 
    }

    protected override void AreaEnter(Area2D area)
    {
        Node parent = area.GetParent();
        if (parent is Enemy enemy)
        {
            GD.Print(" 'LeftAttack' - Hit enemy: " + enemy.Name);
            float damage = gauntletManager.GetGauntletDamage("left");
            enemy.TakeDamage(damage); 
        }
    }
}