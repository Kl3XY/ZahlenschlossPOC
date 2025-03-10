using TMPro;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _cardValueText;
    [SerializeField] private GameObject card;
    
    public string getCardValue()
    {
        return _cardValueText.text;
    }

    public void setCardValue(string str)
    {
        _cardValueText.text = "0";
    }

    public void flipCard()
    {
        var logic = card.GetComponent<CardLogic>();
        logic.IsFlipping = true;
    }
}
