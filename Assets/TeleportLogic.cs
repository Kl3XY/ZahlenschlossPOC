using Unity.VisualScripting;
using UnityEngine;

public class TeleportLogic : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var rt = GetComponent<RectTransform>();
        rt.Rotate(0, 0, 25 * Time.deltaTime);
    }
}
