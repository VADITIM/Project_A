using Godot;
using System;

public partial class GauntletUnlocker : Area2D
{
    [Export]
    public string GauntletType;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is Player player)
        {
            var inventory = player.GetNode<GauntletInventory>("GauntletInventory");
            inventory.UnlockGauntlet(GauntletType);

            QueueFree();
        }
    }
}
