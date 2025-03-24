using UnityEngine;
using UnityEngine.UI;

public class CardOutlineLogic : MonoBehaviour
{
    [SerializeField] public GameObject card;
    [SerializeField] public Sprite positiveImage;
    [SerializeField] public Sprite negativeImage;
    [SerializeField] public Sprite positiveImageOutline;
    [SerializeField] public Sprite negativeImageOutline;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (card.GetComponent<Image>().sprite == positiveImage)
        {
            GetComponent<Image>().sprite = positiveImageOutline;
        } else
        {
            GetComponent<Image>().sprite = negativeImageOutline;
        }
    }
}
