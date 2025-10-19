using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IGauntletUser
{
    // Keep as a property to satisfy IGauntletUser and allow controlled mutation
    public List<Arm> Arms { get; private set; } = new List<Arm>();

    public void AddArm(ArmSlot slot = ArmSlot.None)
    {
        Arms.Add(new Arm(slot));
    }

    public bool RemoveArm(Arm arm)
    {
        if (arm == null) return false;
        arm.Unequip();
        return Arms.Remove(arm);
    }

    /// <summary>
    /// Equip a gauntlet on the specified arm index
    /// </summary>
    public virtual void EquipGauntlet(Gauntlet g, int armIndex)
    {
        if (g == null) return;
        if (armIndex < 0 || armIndex >= Arms.Count) return;

        Arm arm = Arms[armIndex];
        arm.Equip(g, this);
    }

    /// <summary>
    /// Unequip gauntlet from the specified arm index
    /// </summary>
    public virtual void UnequipGauntlet(int armIndex)
    {
        if (armIndex < 0 || armIndex >= Arms.Count) return;
        Arms[armIndex].Unequip();
    }

    // IGauntletUser convenience overloads
    public void EquipGauntlet(Gauntlet gauntlet, Arm arm)
    {
        if (gauntlet == null || arm == null) return;
        if (!Arms.Contains(arm)) return;
        arm.Equip(gauntlet, this);
    }

    public void UnequipGauntlet(Arm arm)
    {
        if (arm == null) return;
        if (!Arms.Contains(arm)) return;
        arm.Unequip();
    }

    public void ActivateAbilityOnAllEquipped()
    {
        foreach (var arm in Arms)
        {
            arm.EquippedGauntlet?.ActivateAbility();
        }
    }
}
