using UnityEngine;

public class DebugCrosshair : MonoBehaviour
{
    [SerializeField] bool hideWhenCursorUnlocked = false;
    [SerializeField, Range(4, 32)] int halfSize = 8;
    [SerializeField] Color color = Color.white;
    Texture2D _tex;

    void OnEnable()
    {
        _tex = Texture2D.whiteTexture;
    }

    void OnGUI()
    {
        if (hideWhenCursorUnlocked && Cursor.lockState != CursorLockMode.Locked) return;

        var w = Screen.width;
        var h = Screen.height;
        int x = w / 2;
        int y = h / 2;

        var prevColor = GUI.color;
        GUI.color = color;

        // vertical
        GUI.DrawTexture(new Rect(x - 1, y - halfSize, 2, halfSize * 2), _tex);
        // horizontal
        GUI.DrawTexture(new Rect(x - halfSize, y - 1, halfSize * 2, 2), _tex);

        GUI.color = prevColor;
    }
}