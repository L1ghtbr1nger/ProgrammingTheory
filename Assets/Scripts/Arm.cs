using UnityEngine;

/// <summary>
/// Represents an arm that can hold a gauntlet.
/// For PlayerEntity, Slot distinguishes Left/Right hand.
/// For EnemyEntity/BossEntity, Slot can be None.
/// </summary>
public class Arm
{
    public ArmSlot Slot { get; private set; }
    public Gauntlet EquippedGauntlet { get; private set; }

    public Arm(ArmSlot slot = ArmSlot.None)
    {
        Slot = slot;
        EquippedGauntlet = null;
    }

    public bool HasGauntlet => EquippedGauntlet != null;

    public void Equip(Gauntlet gauntlet, EntityBase owner)
    {
        Debug.Log($"[Arm:{Slot}] Equip attempt. Incoming={(gauntlet ? gauntlet.GauntletName : "null")}, Owner={(owner ? owner.name : "null")}, Previous={(EquippedGauntlet ? EquippedGauntlet.GauntletName : "null")}");

        if (EquippedGauntlet == gauntlet)
        {
            Debug.Log($"[Arm:{Slot}] Equip skipped: same gauntlet already equipped.");
            return;
        }

        if (EquippedGauntlet != null)
        {
            Debug.Log($"[Arm:{Slot}] Unequipping current '{EquippedGauntlet.GauntletName}' before equip.");
            Unequip();
        }

        EquippedGauntlet = gauntlet;

        if (gauntlet != null)
        {
            gauntlet.Equip(owner);

            // Find mounts on owner (be tolerant about hierarchy placement)
            var mounts = owner ? owner.GetComponent<GauntletMounts>() ?? owner.GetComponentInChildren<GauntletMounts>() ?? owner.GetComponentInParent<GauntletMounts>() : null;
            Transform target = null;
            if (mounts != null)
            {
                target = Slot == ArmSlot.Left ? mounts.LeftHandMount : mounts.RightHandMount;
                if (target == null)
                    Debug.LogWarning($"[Arm:{Slot}] Hand mount not assigned on '{owner.name}'. Falling back to owner transform.");
            }
            else
            {
                Debug.LogWarning($"[Arm:{Slot}] No GauntletMounts found on '{owner?.name}'. Falling back to owner transform.");
            }

            if (target == null && owner != null)
                target = owner.transform;

            if (target != null)
            {
                gauntlet.transform.SetParent(target, false);
                gauntlet.transform.localPosition = Vector3.zero;
                gauntlet.transform.localRotation = Quaternion.identity;
                gauntlet.gameObject.SetActive(true);
                SetEquippedState(gauntlet, true);
                Debug.Log($"[Arm:{Slot}] Equipped '{gauntlet.GauntletName}' at mount '{target.name}'.");
            }
        }
        else
        {
            Debug.Log($"[Arm:{Slot}] Equipped set to null (no gauntlet).");
        }
    }

    public void Unequip()
    {
        if (EquippedGauntlet == null) return;

        Debug.Log($"[Arm:{Slot}] Unequip '{EquippedGauntlet.GauntletName}'");
        var g = EquippedGauntlet;
        g.transform.SetParent(null, true);
        SetEquippedState(g, false);

        g.Unequip();
        EquippedGauntlet = null;
        Debug.Log($"[Arm:{Slot}] Unequipped.");
    }

    private static void SetEquippedState(Gauntlet g, bool equipped)
    {
        // While equipped: disable physics/collisions to avoid interference
        foreach (var rb in g.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = equipped;
            rb.detectCollisions = !equipped;
        }
        foreach (var col in g.GetComponentsInChildren<Collider>(true))
        {
            col.enabled = !equipped;
        }
    }
}

/// <summary>
/// Player-specific hand distinction
/// </summary>
public enum ArmSlot
{
    None,   // For enemies / unspecified
    Left,
    Right
}
