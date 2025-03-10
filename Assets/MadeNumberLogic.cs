using TMPro;
using UnityEngine;

public class MadeNumberLogic : MonoBehaviour
{
    private Vector3 _originalPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _originalPosition = transform.position;
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _originalPosition, Time.deltaTime * 10);
    }

    public void bump()
    {
        var newPos = new Vector3()
        {
            x = _originalPosition.x,
            y = _originalPosition.y + 35,
            z = _originalPosition.z
        };
        transform.position = newPos;
    }
}
