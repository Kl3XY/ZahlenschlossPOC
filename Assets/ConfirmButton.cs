using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfirmButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public GameObject cardDeck;
    [SerializeField] public GameObject gameLogic;

    private Vector3 _originalPosition;

    public void OnPointerClick(PointerEventData eventData)
    {
        var button = gameLogic.GetComponent<GameLogic>();
        button.IsSubmitting = true;
        button.Timer -= 5;
        GameLogic.FinalScore = 0;
    }
    void Start()
    {
        _originalPosition = this.transform.position;
    }

    void Update()
    {
        if (GameLogic.canInteract)
        {
            this.transform.position = _originalPosition;
        } else
        {
            this.transform.position = new Vector3()
            {
                x = _originalPosition.x,
                y = _originalPosition.y + 1000,
                z = _originalPosition.z,
            };
        }
    }
}
