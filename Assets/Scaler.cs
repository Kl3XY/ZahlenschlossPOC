using System;
using UnityEngine;

public class Scaler : MonoBehaviour
{
    [SerializeField] private Vector2 referenceResolution;
    private RectTransform _transform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _transform = GetComponent<RectTransform>();

        float getYDiff = referenceResolution.y - Math.Abs(_transform.localPosition.y);
        float getYPercent = getYDiff / referenceResolution.y;

        float newYPos = Screen.height * getYPercent;
        _transform.position = new Vector3() { 
            x = _transform.position.x, 
            y = newYPos, z = 0 };

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
