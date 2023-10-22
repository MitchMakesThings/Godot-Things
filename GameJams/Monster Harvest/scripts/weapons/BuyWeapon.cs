using System;
using Godot;
using BloodHarvest.scripts.interfaces;

public partial class BuyWeapon : StaticBody3D, IInteractable
{
    [Export]
    private int Cost;

    [Export]
    private PackedScene Weapon;
    
    [Export]
    public string Title { get; set; } = "Buy me an AK!";

    private string _subtitle = "";
    [Export]
    public string Subtitle {
        get
        {
            if (!String.IsNullOrEmpty(_subtitle))
            {
                return _subtitle;
            }

            return $"{Cost} {GameState.Instance.ResourceName}";
        }
        private set => _subtitle = value;
    }
    
    public void Interact(InteractionSystem interactionSystem)
    {
        if (GameState.Instance.Resource < Cost)
        {
            GameState.Instance.PlayRejectSound();
            return;
        }
        
        // Pay the toll
        GameState.Instance.RemoveResource(Cost);
        
        // Get the gun
        var boughtWeapon = Weapon.Instantiate<Pistol>();
        interactionSystem.Player.EquipWeapon(boughtWeapon);
        
        // TODO Change into a restock station? Maybe you only have to buy a weapon once...
    }
}
