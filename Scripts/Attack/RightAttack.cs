using Godot;

public partial class RightAttack : BaseAttack
{
    public RightAttack(CharacterBody2D player, AnimatedSprite2D playerSprite, AnimatedSprite2D effectSprite, Area2D hitbox, AnimatedSprite2D gauntletSprite, GauntletManager gauntletManager)
        : base(player, playerSprite, effectSprite, hitbox, gauntletSprite, gauntletManager)
    {
    }

    protected override string GetGauntletType()
    {
        return "right";
    }

    public void Update(float delta)
    {
        UpdateAttack(delta, "rightAttack"); 
    }

    protected override void AreaEnter(Area2D area)
    {
        Node parent = area.GetParent();
        if (parent is Enemy enemy)
        {
            GD.Print(" 'RightAttack' - Hit enemy: " + enemy.Name);
            float damage = gauntletManager.GetGauntletDamage("right");
            enemy.TakeDamage(damage);
        }
    }
}