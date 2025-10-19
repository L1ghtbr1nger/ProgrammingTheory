using UnityEngine;
using UnityEngine.UI;

public class ReticleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image reticleImage;

    [Header("Behavior")]
    [SerializeField] bool hideWhenCursorUnlocked = false; // set true if you only want it while mouse-look is active

    void Reset()
    {
        if (reticleImage == null) reticleImage = GetComponent<Image>();
    }

    void Update()
    {
        if (reticleImage == null) return;

        if (hideWhenCursorUnlocked)
        {
            bool locked = Cursor.lockState == CursorLockMode.Locked;
            reticleImage.enabled = locked;
        }
        else
        {
            reticleImage.enabled = true;
        }
    }
}