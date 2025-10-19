using UnityEngine;
using System.Linq;

public static class ColorCombiner
{
    public static Color Combine(params Color[] colors)
    {
        if (colors == null || colors.Length == 0)
            return Color.clear;

        float r = 0, g = 0, b = 0;
        foreach (var c in colors)
        {
            r += c.r;
            g += c.g;
            b += c.b;
        }

        float max = Mathf.Max(r, g, b);
        if (max > 1f)
        {
            r /= max;
            g /= max;
            b /= max;
        }

        // If all channels are near full, return alpha/white (structure erasure)
        if (r > 0.9f && g > 0.9f && b > 0.9f)
            return new Color(1, 1, 1, 0.25f);

        return new Color(r, g, b, 1f);
    }
}
