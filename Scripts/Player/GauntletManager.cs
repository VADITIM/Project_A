using Godot;

public partial class GauntletManager : Node
{
    public enum GauntletType
    {
        Air,
        Fire,
    }

    private AnimatedSprite2D leftGauntletSprite;
    private AnimatedSprite2D rightGauntletSprite;
    private AnimatedSprite2D leftAttackSprite;
    private AnimatedSprite2D rightAttackSprite;
    private GauntletType leftGauntletType;
    private GauntletType rightGauntletType;

    public void InitGauntlets(AnimatedSprite2D leftSprite, AnimatedSprite2D rightSprite, 
                              AnimatedSprite2D leftAttack, AnimatedSprite2D rightAttack)
    {
        leftGauntletSprite = leftSprite;
        rightGauntletSprite = rightSprite;
        leftAttackSprite = leftAttack;
        rightAttackSprite = rightAttack;
    }

    public void EquipGauntlet(GauntletType type, bool isRightHand)
    {
        if (isRightHand)
        {
            rightGauntletType = type;
            UpdateGauntletAnimation(rightGauntletSprite, rightGauntletType, isRightHand);
        }
        else
        {
            leftGauntletType = type;
            UpdateGauntletAnimation(leftGauntletSprite, leftGauntletType, isRightHand);
        }
    }

    public void UpdateGauntletAnimation(AnimatedSprite2D gauntletSprite, GauntletType type, bool isRightHand)
    {
        string baseAnimation = type switch
        {
            GauntletType.Air => "air_gauntlet",
            GauntletType.Fire => "fire_gauntlet",
            _ => "default_gauntlet"
        };
        gauntletSprite.Visible = true;
        
        gauntletSprite.Play($"{baseAnimation}_idle_down");
    }

    public string GetAttackAnimation(GauntletType type, bool isRightHand, string direction)
    {
        string baseAnimation = type switch
        {
            GauntletType.Air => "air_attack_1",
            GauntletType.Fire => "fire_attack_1",
            _ => "default_attack"
        };
        return $"{baseAnimation}_{direction}".TrimEnd('_');
    }

    public string GetGauntletTypeString(GauntletType gauntletType)
    {
        return gauntletType.ToString().ToLower() + "_";
    }

    public void SetRightGauntletType(GauntletType type)
    {
        rightGauntletType = type;
        GD.Print("Right gauntlet updated to: " + rightGauntletType);
    }


    public GauntletType GetLeftGauntletType()
    {
        return leftGauntletType;
    }

    public GauntletType GetRightGauntletType()
    {
        return rightGauntletType;
    }
}