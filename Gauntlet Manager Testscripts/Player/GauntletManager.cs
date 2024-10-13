using Godot;
using System.Collections.Generic;

public partial class GauntletManager : Node
{
    [Export] public PackedScene AirGauntletScene;
    [Export] public PackedScene FireGauntletScene;
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
        GD.Print(leftGauntlet != null ? "Left Gauntlet Initialized" : "Left Gauntlet is Null");
        GD.Print(rightGauntlet != null ? "Right Gauntlet Initialized" : "Right Gauntlet is Null");


        // Initialize gauntlets
        EquipGauntletToHand(leftGauntlet, AirGauntletScene, false);
        EquipGauntletToHand(rightGauntlet, FireGauntletScene, true);
        
    }


    public void EquipGauntlet(string side, string gauntletType)
    {
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

    public void EquipGauntletToHand(Node2D handNode, PackedScene gauntletScene, bool isRightHand)
    {
        if (gauntletScene == null)
        {
            GD.Print("Gauntlet scene is null");
            return;
        }

        foreach (Node child in handNode.GetChildren())
        {
            child.QueueFree();
        }

        var newGauntlet = (Node2D)gauntletScene.Instantiate();
        handNode.AddChild(newGauntlet);

        var gauntletSprite = newGauntlet.GetNode<AnimatedSprite2D>("GauntletSprite");
        var effectSprite = newGauntlet.GetNode<AnimatedSprite2D>("AttackAnimation/EffectSprite");
        var hitbox = newGauntlet.GetNode<Area2D>("AttackAnimation/EffectSprite/Hitbox");

        if (gauntletSprite == null || effectSprite == null || hitbox == null)
        {
            GD.Print("Failed to retrieve one or more components from the gauntlet node");
            return;
        }

        var player = GetParent().GetParent().GetNode<CharacterBody2D>("Player");
        if (player == null)
        {
            GD.Print("Failed to retrieve player");
            return;
        }

        var playerSprite = player.GetNode<AnimatedSprite2D>("PlayerSprite");
        if (playerSprite == null)
        {
            GD.Print("Failed to retrieve PlayerSprite from the player node");
            return;
        }

        if (isRightHand)
        {
            rightAttack = new RightAttack(player, playerSprite, effectSprite, hitbox, gauntletSprite, this);
            GD.Print("RightAttack initialized");
        }
        else
        {
            leftAttack = new LeftAttack(player, playerSprite, effectSprite, hitbox, gauntletSprite, this);
            GD.Print("LeftAttack initialized");
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