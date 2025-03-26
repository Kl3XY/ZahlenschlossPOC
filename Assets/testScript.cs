using UnityEngine;

public class testScript : MonoBehaviour
{
    [SerializeField] Material material;
    static private ColorGen _colGen = new();

    static Color color1 = Color.red;
    static Color color2 = Color.blue;

    private void Start()
    {
        _colGen.colorDiff = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        var cCol1 = material.GetColor("_Color1");
        var cCol2 = material.GetColor("_Color2");

        material.SetColor("_Color1", Color.Lerp(cCol1, color1, 0.2f));
        material.SetColor("_Color2", Color.Lerp(cCol2, color2, 0.2f));
    }

    static public void generateNewBG()
    {
        color1 = _colGen.GenerateUniqueColor();
        color2 = _colGen.GenerateUniqueColor();
        _colGen.clearColorList();
    }
}
