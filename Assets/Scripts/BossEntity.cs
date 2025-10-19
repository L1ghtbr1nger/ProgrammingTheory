using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossEntity : EnemyEntity
{
    public override void PerformAIAction()
    {
        GameObject target = GetTargetObject();
        if (target == null) return;

        var equippedGauntlets = Arms
            .Select(a => a.EquippedGauntlet)
            .Where(g => g != null)
            .ToList();

        foreach (var g in equippedGauntlets)
        {
            if (Random.value > 0.5f)
                g.AbsorbColor(target);
            else
                g.RestoreColor(target);

            g.ActivateAbility();
        }

        if (equippedGauntlets.Count == 3)
        {
            var combined = ColorCombiner.Combine(equippedGauntlets.Select(g => g.GetEffectiveColor()).ToArray());
            TriggerAlphaAbility(target, combined);
        }
    }

    private void TriggerAlphaAbility(GameObject target, Color combined)
    {
        // If combined is very bright, treat as erasure; otherwise restoration
        bool erase = (combined.r + combined.g + combined.b) / 3f > 0.85f;

        if (target != null && target.TryGetComponent<Renderer>(out var renderer) && renderer.material.HasProperty("_Color"))
        {
            var c = renderer.material.color;
            float delta = erase ? -0.25f : 0.25f;
            c.a = Mathf.Clamp01(c.a + delta);
            renderer.material.color = c;
            Debug.Log($"{name} applied alpha {(erase ? "erase" : "restore")} effect: new alpha={c.a}");
        }
    }

    public override GameObject GetTargetObject()
    {
        return GameObject.FindWithTag("Player");
    }
}
