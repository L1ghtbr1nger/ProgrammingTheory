using System.Collections.Generic;
using UnityEngine;

public interface IGauntletUser
{
    List<Arm> Arms { get; }
    void EquipGauntlet(Gauntlet gauntlet, Arm arm);
    void UnequipGauntlet(Arm arm);
    void ActivateAbilityOnAllEquipped();
}
