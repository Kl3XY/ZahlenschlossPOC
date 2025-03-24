using System;
using TMPro;
using Unity.VisualScripting;
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
        _cardValueText.text = str;
    }

    public void setTeleportStatus(modifiers modifier, GameObject obj, Color col)
    {
        var script = card.GetComponent<CardLogic>().GetComponent<CardLogic>();

        script.currentModifier = modifier;
        script.attachedGameObject = obj;
        script.modifierColor = col;
    }

    public Tuple<modifiers, GameObject> getTeleportStatus()
    {
        var script = card.GetComponent<CardLogic>().GetComponent<CardLogic>();
        return new Tuple<modifiers, GameObject>(script.currentModifier, script.attachedGameObject);
    }

    public void flipCard()
    {
        var logic = card.GetComponent<CardLogic>();
        logic.IsFlipping = true;
    }
}
