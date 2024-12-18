using Godot;
using System;
using System.Collections.Generic;

public partial class GauntletManager : Node
{
    [Export] public PackedScene AirGauntletScene;
    [Export] public PackedScene FireGauntletScene;
    [Export] public PackedScene WaterGauntletScene;
    [Export] public PackedScene AirAttackScene;
    [Export] public PackedScene FireAttackScene;
    [Export] public PackedScene WaterAttackScene;
    [Export] public Node2D leftGauntlet; 
    [Export] public Node2D rightGauntlet;

    private LeftAttack leftAttack;
    private RightAttack rightAttack;

    private string currentLeftGauntletIdleAnimation;
    private string currentRightGauntletIdleAnimation;

    private string currentLeftGauntletAttackAnimation;
    private string currentRightGauntletAttackAnimation;
    
    private Dictionary<string, string> equippedGauntlets = new Dictionary<string, string>();

    private Dictionary<string, float> attackOffsets = new Dictionary<string, float>
    {
        { "Air", 15f },
        { "Fire", 25f },
        { "Water", 15f }
    };

    private Dictionary<string, float> gauntletDamages = new Dictionary<string, float>
    {
        { "Air", 10f },
        { "Fire", 20f },
        { "Water", 15f }
    };

    public override void _Ready()
    {
        EquipGauntletToHand(leftGauntlet, WaterGauntletScene, WaterAttackScene, false);
        EquipGauntletToHand(rightGauntlet, FireGauntletScene, FireAttackScene, true);
    }

    public void EquipGauntlet(string side, string gauntletType)
    {
        if (side == "left")
        {
            if (leftAttack != null)
            {
                leftAttack.StopAttack();
            }
            EquipGauntletToHand(leftGauntlet, GetGauntletScene(gauntletType), GetAttackScene(gauntletType), false);
        }
        else if (side == "right")
        {
            if (rightAttack != null)
            {
                rightAttack.StopAttack();
            }
            EquipGauntletToHand(rightGauntlet, GetGauntletScene(gauntletType), GetAttackScene(gauntletType), true);
        }
        equippedGauntlets[side] = gauntletType;
    }

    public void EquipGauntletToHand(Node2D handNode, PackedScene gauntletScene, PackedScene attackScene, bool isRightHand)
    {
        foreach (Node child in handNode.GetChildren())
        {
            child.QueueFree();
        }
        var newGauntlet = (Node2D)gauntletScene.Instantiate();
        handNode.AddChild(newGauntlet);
        var attackAnimation = (Node2D)attackScene.Instantiate();
        newGauntlet.AddChild(attackAnimation);
        var gauntletSprite = newGauntlet.GetNode<AnimatedSprite2D>("GauntletSprite");
        var effectSprite = attackAnimation.GetNode<AnimatedSprite2D>("EffectSprite");
        var hitbox = attackAnimation.GetNode<Area2D>("EffectSprite/Hitbox");
        var player = GetParent().GetParent().GetNode<CharacterBody2D>("Player");
        var playerSprite = player.GetNode<AnimatedSprite2D>("PlayerSprite");
        if (isRightHand)
        {
            rightAttack = new RightAttack(player, playerSprite, effectSprite, hitbox, gauntletSprite, this);
        }
        else
        {
            leftAttack = new LeftAttack(player, playerSprite, effectSprite, hitbox, gauntletSprite, this);
        }
    }

    private PackedScene GetGauntletScene(string gauntletType)
    {
        switch (gauntletType)
        {
            case "Air":
                return AirGauntletScene;
            case "Fire":
                return FireGauntletScene;
            case "Water":
                return WaterGauntletScene;
            default:
                return null;
        }
    }

    private PackedScene GetAttackScene(string gauntletType)
    {
        switch (gauntletType)
        {
            case "Air":
                return AirAttackScene;
            case "Fire":
                return FireAttackScene;
            case "Water":
                return WaterAttackScene;
            default:
                return null;
        }
    }

    public string GetGauntletType(string side)
    {
        return equippedGauntlets.ContainsKey(side) ? equippedGauntlets[side] : null;
    }

    public float GetGauntletDamage(string side)
    {
        string gauntletType = GetGauntletType(side);
        return gauntletDamages.ContainsKey(gauntletType) ? gauntletDamages[gauntletType] : 0f;
    }

    public void UpdateGauntletAnimations(string side)
    {
        if (side == "left")
        {
            currentLeftGauntletIdleAnimation = GetCurrentGauntletIdleAnimation("left");
            currentLeftGauntletAttackAnimation = GetCurrentGauntletAttackAnimation("left");
        }
        else if (side == "right")
        {
            currentRightGauntletIdleAnimation = GetCurrentGauntletIdleAnimation("right");
            currentRightGauntletAttackAnimation = GetCurrentGauntletAttackAnimation("right");
        }
    }

    public string GetCurrentGauntletIdleAnimation(string side)
    {
        string gauntletType = GetGauntletType(side);
        if (gauntletType == "Air")
        {
            return "air_gauntlet_idle"; 
        }
        else if (gauntletType == "Fire")
        {
            return "fire_gauntlet_idle"; 
        }
        else if (gauntletType == "Water")
        {
            return "water_gauntlet_idle"; 
        }
        return "default_idle";
    }

    public string GetCurrentGauntletAttackAnimation(string side)
    {
        string gauntletType = GetGauntletType(side);
        if (gauntletType == "Air")
        {
            return "air_gauntlet_attack"; 
        }
        else if (gauntletType == "Fire")
        {
            return "fire_gauntlet_attack"; 
        }
        else if (gauntletType == "Water")
        {
            return "water_gauntlet_attack"; 
        }
        return "default_attack";
    }

    public void SetCurrentGauntletIdleAnimation(string hand, string animation)
    {
        if (hand == "left")
        {
            currentLeftGauntletIdleAnimation = animation;
        }
        else if (hand == "right")
        {
            currentRightGauntletIdleAnimation = animation;
        }
    }

    public float GetAttackOffset(string gauntletType)
    {
        return attackOffsets.ContainsKey(gauntletType) ? attackOffsets[gauntletType] : 25f;
    }

    public LeftAttack GetLeftAttack()
    {
        return leftAttack;
    }

    public RightAttack GetRightAttack()
    {
        return rightAttack;
    }
}