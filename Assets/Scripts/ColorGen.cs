using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ColorGen : MonoBehaviour
{
    /*
     * Die Farben die vorher generiert worden.
     */
    public List<Color> colorList = new List<Color>();

    public Color GenerateUniqueColor()
    {
        int maxAttempts = 25; // Maximale Versuche, um eine passende Farbe zu finden
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // HSV-Werte zufällig generieren
            float h = UnityEngine.Random.value; // Farbton (Hue)
            float s = UnityEngine.Random.Range(1.0f, 1f); // Sättigung (kräftige Farben)
            float v = UnityEngine.Random.Range(1.0f, 1f); // Helligkeit

            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = 0.45f;

            // Prüfen, ob die Farbe einzigartig genug ist
            if (IsColorUnique(h))
            {
                return newColor;
            }
            Debug.Log(attempt);
        }
        Debug.LogWarning("Maximale Versuche erreicht! Rückgabe einer möglichen Farbe.");
        return Color.HSVToRGB(UnityEngine.Random.value, 1f, 1f); // Notfallfarbe
    }

    public void clearColorList()
    {
        colorList.Clear();
    }


    bool IsColorUnique(float newHue)
    {
        foreach (Color color in colorList)
        {
            Color.RGBToHSV(color, out float existingHue, out _, out _);

            if (Mathf.Abs(existingHue - newHue) < 0.1f)
            {
                return false;
            }
        }
        return true;
    }
}
