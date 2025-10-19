using UnityEngine;
using System.Linq;

public static class ColorUtility3D
{
    public static Color CombineColors(params Color[] colors)
    {
        if (colors == null || colors.Length == 0)
            return Color.black;

        Color result = Color.black;
        foreach (var c in colors)
            result += c;

        result.r = Mathf.Clamp01(result.r);
        result.g = Mathf.Clamp01(result.g);
        result.b = Mathf.Clamp01(result.b);

        // If all 3 channels maxed, we get white (used as alpha color)
        return result;
    }

    public static Color ApplyAlphaChannel(Color baseColor, float alpha)
    {
        baseColor.a = Mathf.Clamp01(alpha);
        return baseColor;
    }
}
