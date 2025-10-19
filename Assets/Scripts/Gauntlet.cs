using UnityEngine;

public abstract class Gauntlet : MonoBehaviour
{
    public string GauntletName { get; protected set; }
    public Color BaseColor { get; protected set; }   // RGB base color
    public float Intensity { get; protected set; } = 1f; // 0-1 multiplier
    public string EnergyAbilityName { get; protected set; }

    [Header("Highlight")]
    [Tooltip("Inactive-by-default child (e.g., a Quad) that shows when targeted.")]
    [SerializeField] private GameObject highlightObject;

    public EntityBase EquippedBy { get; private set; }
    public bool IsEquipped => EquippedBy != null;

    public void Equip(EntityBase owner)
    {
        EquippedBy = owner;
        Debug.Log($"[Gauntlet:{name}] Equipped by '{owner?.name}'.");
        OnEquipped();
    }

    public void Unequip()
    {
        Debug.Log($"[Gauntlet:{name}] Unequipped from '{EquippedBy?.name}'.");
        OnUnequipped();
        EquippedBy = null;
    }

    protected virtual void OnEquipped() { }
    protected virtual void OnUnequipped() { }

    public abstract void AbsorbColor(GameObject target);
    public abstract void RestoreColor(GameObject target);
    public abstract void ActivateAbility();

    public virtual void SetIntensity(float newIntensity)
    {
        Intensity = Mathf.Clamp01(newIntensity);
    }

    public Color GetEffectiveColor()
    {
        // Scale RGB by intensity but keep alpha unchanged
        return new Color(BaseColor.r * Intensity, BaseColor.g * Intensity, BaseColor.b * Intensity, BaseColor.a);
    }

    // Highlight support lives in the Gauntlet itself (no extra component needed)
    public virtual void SetHighlighted(bool on)
    {
        if (highlightObject != null)
            highlightObject.SetActive(on);
    }

    protected virtual void OnDisable()
    {
        SetHighlighted(false);
    }

    // Convenience: auto-find a child named "Quad" if not assigned
    protected virtual void Reset()
    {
        if (highlightObject == null)
        {
            var t = transform.Find("Quad");
            if (t != null) highlightObject = t.gameObject;
        }
    }
}
