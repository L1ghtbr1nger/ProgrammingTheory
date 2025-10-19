using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : EntityBase, IAIEntity
{
    public virtual void PerformAIAction()
    {
        GameObject target = GetTargetObject();
        if (target == null) return;

        foreach (var arm in Arms)
        {
            var g = arm.EquippedGauntlet;
            if (g == null) continue;

            if (Random.value > 0.5f)
                g.AbsorbColor(target);
            else
                g.RestoreColor(target);

            g.ActivateAbility();
        }
    }

    public virtual GameObject GetTargetObject()
    {
        return GameObject.FindWithTag("Player");
    }
}
