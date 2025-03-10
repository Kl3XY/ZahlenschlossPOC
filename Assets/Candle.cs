using UnityEngine;
using UnityEngine.UI;

public class Candle : MonoBehaviour
{
    private Image _image;
    [SerializeField] private double delay = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.fixedDeltaTime;

        if (delay <= 0)
        {
            delay = 1;

            _image.color = new Color()
            {
                r = _image.color.r,
                g = _image.color.g,
                b = _image.color.b,
                a = Mathf.Min(Mathf.Max(_image.color.a - Random.Range(-0.5f, 0.5f), 0.2f), 0.7f)
            };
        }
    }
}
