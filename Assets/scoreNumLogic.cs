using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class scoreNumLogic : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] double SurviveTime = 20;
    private float ySpd = 2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        SurviveTime -= Time.fixedDeltaTime;

        transform.position = new Vector3(transform.position.x, transform.position.y+ySpd, transform.position.z);

        if (SurviveTime < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
