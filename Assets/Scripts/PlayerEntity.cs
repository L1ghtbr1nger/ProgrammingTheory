using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEntity : EntityBase
{
    private bool isAbsorbMode = true;
    private Gauntlet _hoveredGauntlet;
    private Gauntlet _inventoryGauntlet; // single inventory slot

    private void Awake()
    {
        EnsureStandardArms();
    }

    // Commands invoked by PlayerController
    public void WieldLeftCommand()  => HandleWield(ArmSlot.Left);
    public void WieldRightCommand() => HandleWield(ArmSlot.Right);
    public void AbilityLeftCommand()  => ActivateAbility(ArmSlot.Left);
    public void AbilityRightCommand() => ActivateAbility(ArmSlot.Right);
    public void SwitchModeCommand() => SwitchMode();
    public void InteractCommand()   => Interact();
    public void SwitchHandsCommand() => SwitchHands();

    private void Update()
    {
        UpdateGauntletHoverHighlight();
    }

    private void HandleWield(ArmSlot slot)
    {
        Gauntlet gauntlet = GetGauntletInHand(slot);
        if (gauntlet == null) return;

        GameObject target = GetPlayerTarget();
        if (target == null) return;

        if (isAbsorbMode)
            gauntlet.AbsorbColor(target);
        else
            gauntlet.RestoreColor(target);
    }

    private void SwitchMode()
    {
        isAbsorbMode = !isAbsorbMode;
        Debug.Log("Player mode switched to: " + (isAbsorbMode ? "Absorb" : "Restore"));
    }

    // Use hovered gauntlet so click matches the highlighted object
    private void Interact()
    {
        EnsureStandardArms();

        var gauntlet = _hoveredGauntlet;
        if (gauntlet == null) return;
        if (gauntlet.IsEquipped && gauntlet.EquippedBy != null && gauntlet.EquippedBy != this) return;

        var right = GetArm(ArmSlot.Right);
        var left  = GetArm(ArmSlot.Left);

        if (!right.HasGauntlet)
        {
            right.Equip(gauntlet, this);
            return;
        }

        if (!left.HasGauntlet)
        {
            left.Equip(gauntlet, this);
            return;
        }

        if (_inventoryGauntlet == null)
        {
            StoreInInventory(gauntlet);
            return;
        }
    }

    private void SwitchHands()
    {
        EnsureStandardArms();
        var left  = GetArm(ArmSlot.Left);
        var right = GetArm(ArmSlot.Right);

        var l = left.EquippedGauntlet;
        var r = right.EquippedGauntlet;
        var i = _inventoryGauntlet;

        // If inventory empty, just swap hands (skip inventory)
        if (i == null)
        {
            if (l == r) return;
            if (l == null && r == null) return;

            if (l == null && r != null)
            {
                left.Equip(r, this);
                right.Unequip();
            }
            else if (l != null && r == null)
            {
                right.Equip(l, this);
                left.Unequip();
            }
            else
            {
                left.Equip(r, this);
                right.Equip(l, this);
            }
            return;
        }

        // Full clockwise rotation:
        // Right -> Left, Left -> Inventory, Inventory -> Right
        // 1) Right -> Left
        if (r != null) left.Equip(r, this);
        else           left.Unequip();

        // 2) Inventory -> Right
        right.Unequip();
        if (i != null)
        {
            i.gameObject.SetActive(true);
            right.Equip(i, this);
        }

        // 3) Left(original) -> Inventory
        _inventoryGauntlet = l;
        if (_inventoryGauntlet != null)
        {
            _inventoryGauntlet.SetHighlighted(false);
            _inventoryGauntlet.gameObject.SetActive(false);
        }
    }

    private void StoreInInventory(Gauntlet g)
    {
        _inventoryGauntlet = g;
        _inventoryGauntlet.SetHighlighted(false);
        _inventoryGauntlet.gameObject.SetActive(false);
    }

    private Gauntlet GetGauntletInHand(ArmSlot slot)
    {
        var arm = Arms.FirstOrDefault(a => a.Slot == slot);
        return arm?.EquippedGauntlet;
    }

    private GameObject GetPlayerTarget()
    {
        var cam = Camera.main;
        if (cam == null) return null;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    // Reticle-based (center of screen) targeting using tag "Gauntlet"
    private bool TryGetGauntletUnderReticle(out Gauntlet gauntlet)
    {
        gauntlet = null;
        var cam = Camera.main;
        if (cam == null) return false;

        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = cam.ScreenPointToRay(center);

        var hits = Physics.RaycastAll(ray, 100f, ~0, QueryTriggerInteraction.Collide);
        if (hits.Length == 0) return false;

        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var h in hits)
        {
            // Find the nearest ancestor with the "Gauntlet" tag
            var tagged = FindTaggedAncestor(h.collider.transform, "Gauntlet");
            if (tagged == null) continue;

            // Resolve the actual component placed on the gauntlet (SimpleGauntlet)
            var sg = tagged.GetComponent<SimpleGauntlet>() ?? tagged.GetComponentInChildren<SimpleGauntlet>(true);
            if (sg != null)
            {
                gauntlet = sg; // SimpleGauntlet derives from Gauntlet
                return true;
            }
        }

        return false;
    }

    private static Transform FindTaggedAncestor(Transform t, string tag)
    {
        while (t != null)
        {
            if (t.CompareTag(tag)) return t;
            t = t.parent;
        }
        return null;
    }

    private void UpdateGauntletHoverHighlight()
    {
        // Optional gate: only when mouse-look is active
        // if (Cursor.lockState != CursorLockMode.Locked) return;

        Gauntlet newHover = null;
        if (TryGetGauntletUnderReticle(out var g))
            newHover = g;

        if (newHover == _hoveredGauntlet) return;

        // Turn off previous
        if (_hoveredGauntlet != null)
            _hoveredGauntlet.SetHighlighted(false);

        _hoveredGauntlet = newHover;

        // Turn on current
        if (_hoveredGauntlet != null)
            _hoveredGauntlet.SetHighlighted(true);
    }

    private void EnsureStandardArms()
    {
        if (Arms.FirstOrDefault(a => a.Slot == ArmSlot.Left) == null)
            AddArm(ArmSlot.Left);
        if (Arms.FirstOrDefault(a => a.Slot == ArmSlot.Right) == null)
            AddArm(ArmSlot.Right);
    }

    private Arm GetArm(ArmSlot slot)
    {
        var arm = Arms.FirstOrDefault(a => a.Slot == slot);
        if (arm == null)
        {
            AddArm(slot);
            arm = Arms.FirstOrDefault(a => a.Slot == slot);
        }
        return arm;
    }

    private void ActivateAbility(ArmSlot slot)
    {
        var gauntlet = GetGauntletInHand(slot);
        if (gauntlet != null)
        {
            gauntlet.ActivateAbility();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the reticle ray in Scene view
        var cam = Camera.main;
        if (cam == null) return;
        Vector3 center = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = cam.ScreenPointToRay(center);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(ray.origin, ray.direction * 3f);
    }
}
