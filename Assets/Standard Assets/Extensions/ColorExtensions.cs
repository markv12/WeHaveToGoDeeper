using UnityEngine;
public static class ColorExtensions : object
{
    public static readonly Color WHITE_CLEAR = new Color(1, 1, 1, 0);

    public static Color SetR(this Color color, float value)
    {
        color = new Color(value, color.g, color.b, color.a);
        return color;
    }

    public static Color SetG(this Color color, float value)
    {
        color = new Color(color.r, value, color.b, color.a);
        return color;
    }

    public static Color SetB(this Color color, float value)
    {
        color = new Color(color.r, color.g, value, color.a);
        return color;
    }

    public static Color SetA(this Color color, float value)
    {
        color = new Color(color.r, color.g, color.b, value);
        return color;
    }
}