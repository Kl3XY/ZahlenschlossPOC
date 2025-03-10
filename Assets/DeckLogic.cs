using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class DeckLogic : MonoBehaviour
{
    [SerializeField] public GameObject value;

    private string getUserBinary()
    {
        var res = "";
        foreach (Transform child in transform)
        {
            var comp = child.GetComponent<CardInfo>();
            res += comp.getCardValue();
        }
        return res;
    }

    public GameObject GetCard(int index)
    {
        return transform.GetChild(index).gameObject;
    }
}
