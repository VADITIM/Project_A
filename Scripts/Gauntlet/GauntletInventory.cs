using Godot;
using System.Collections.Generic;

public partial class GauntletInventory : Node
{
    private List<string> unlockedGauntlets = new List<string>();

    public void UnlockGauntlet(string gauntletType)
    {
        if (!unlockedGauntlets.Contains(gauntletType))
        {
            unlockedGauntlets.Add(gauntletType);
        }
    }

    public List<string> GetUnlockedGauntlets()
    {
        return unlockedGauntlets;
    }

    public bool IsGauntletUnlocked(string gauntletType)
    {
        return unlockedGauntlets.Contains(gauntletType);
    }
}