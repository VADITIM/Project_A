using Godot;
using System.Collections.Generic;

public partial class GauntletManager : Node
{
    [Export] public PackedScene AirGauntletScene;
    [Export] public PackedScene FireGauntletScene;
    [Export] public PackedScene AirAttackScene;
    [Export] public PackedScene FireAttackScene;
    [Export] public PackedScene WaterGauntletScene;
    [Export] public PackedScene WaterAttackScene;
    [Export] private Node2D leftGauntlet; 
    [Export] private Node2D rightGauntlet;

    private LeftAttack leftAttack;
    private RightAttack rightAttack;

    private string currentLeftGauntletIdleAnimation;
    private string currentRightGauntletIdleAnimation;

    private string currentLeftGauntletAttackAnimation;
    private string currentRightGauntletAttackAnimation;
    
    private Dictionary<string, string> equippedGauntlets = new Dictionary<string, string>();

    public override void _Ready()
    {
        EquipGauntletToHand(leftGauntlet, WaterGauntletScene, WaterAttackScene, false);
        EquipGauntletToHand(rightGauntlet, FireGauntletScene, FireAttackScene, true);
    }


    public void EquipGauntlet(string side, string gauntletType)
    {
        if (side == "left")
        {
            if (gauntletType == "Air")
            {
                EquipGauntletToHand(leftGauntlet, AirGauntletScene, AirAttackScene, false);
            }
             if (gauntletType == "Fire")
            {
                EquipGauntletToHand(leftGauntlet, FireGauntletScene, FireAttackScene, false);
            }
             if (gauntletType == "Water")
            {
                EquipGauntletToHand(leftGauntlet, WaterGauntletScene, WaterAttackScene, false);
            }
        }
        else if (side == "right")
        {
            if (gauntletType == "Air")
            {
                EquipGauntletToHand(rightGauntlet, AirGauntletScene, AirAttackScene, true);
            }
            if (gauntletType == "Fire")
            {
                EquipGauntletToHand(rightGauntlet, FireGauntletScene, FireAttackScene, true);
            }
             if (gauntletType == "Water")
            {
                EquipGauntletToHand(leftGauntlet, WaterGauntletScene, WaterAttackScene, false);
            }
        }
        equippedGauntlets[side] = gauntletType;
    }

    public string GetGauntletType(string side)
    {
        return equippedGauntlets.ContainsKey(side) ? equippedGauntlets[side] : null;
    }

    public string GetCurrentGauntletIdleAnimation(string side)
    {
        string gauntletType = GetGauntletType(side);
        // GD.Print($"Retrieving idle animation for {side} gauntlet: {gauntletType}");
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
        // GD.Print($"Retrieving attack animation for {side} gauntlet: {gauntletType}");
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

    public LeftAttack GetLeftAttack()
    {
        return leftAttack;
    }

    public RightAttack GetRightAttack()
    {
        return rightAttack;
    }
}