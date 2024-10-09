using System;
using Godot;

public partial class AirSpell : Spell
{
    public AirSpell(CharacterBody2D player, AnimatedSprite2D effectSprite) 
        : base(player, effectSprite) 
    {
        time = 1.55f;
        cooldown = .3f; 
    }

    protected override void UpdateAnimation()
    {
        if (Math.Abs(attackDirection.X) > Math.Abs(attackDirection.Y))
        {
            if (attackDirection.X > 0)
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(70, 0);
                effectSprite.FlipH = false;
                effectSprite.Rotation = 0;
            }
            else
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(-70, 0);
                effectSprite.FlipH = true;
                effectSprite.Rotation = 0;
            }
        }
        else
        {
            if (attackDirection.Y > 0)
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(0, 70);
                effectSprite.FlipH = false;
                effectSprite.Rotation = 90;
            }
            else
            {
                effectSprite.Play("default");
                effectSprite.Visible = true;
                effectSprite.Position = new Vector2(0, -70);
                effectSprite.FlipH = false;
                effectSprite.Rotation = -90;
            }
        }
        effectSprite.Stop();
        effectSprite.Play();
        effectSprite.Frame = 0;
    }
}
