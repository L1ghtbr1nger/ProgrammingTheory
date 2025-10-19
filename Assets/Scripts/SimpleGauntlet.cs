using UnityEngine;

[DisallowMultipleComponent]
public sealed class SimpleGauntlet : Gauntlet
{
    // Set these in the inspector or via prefab
    [SerializeField] private string gauntletName;
    [SerializeField] private Color baseColor = Color.white;
    [SerializeField] private string energyAbilityName;

    private void Awake()
    {
        // Sync base class properties from serialized fields
        GauntletName = gauntletName;
        BaseColor = baseColor;
        EnergyAbilityName = energyAbilityName;
    }

    private void OnValidate()
    {
        // Keep base properties in sync while editing in inspector
        GauntletName = gauntletName;
        BaseColor = baseColor;
        EnergyAbilityName = energyAbilityName;
    }

    public override void AbsorbColor(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning($"{GauntletName}: AbsorbColor target is null.", this);
            return;
        }
        Debug.Log($"{GauntletName} absorbs color from {target.name} (Intensity {Intensity:0.00})", this);
        // Implement absorption logic
    }

    public override void RestoreColor(GameObject target)
    {
        if (target == null)
        {
            Debug.LogWarning($"{GauntletName}: RestoreColor target is null.", this);
            return;
        }
        Debug.Log($"{GauntletName} restores color to {target.name} (Intensity {Intensity:0.00})", this);
        // Implement restoration logic
    }

    public override void ActivateAbility()
    {
        var ability = string.IsNullOrWhiteSpace(EnergyAbilityName) ? "<unnamed ability>" : EnergyAbilityName;
        Debug.Log($"{GauntletName} activates {ability} (Intensity {Intensity:0.00})", this);
        // Implement energy ability
    }
}
