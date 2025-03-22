using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

enum flipStates
{
    positive,
    negative
}

public class CardLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] public GameObject cardValueText;
    [SerializeField] public GameObject scoreNumCreateTextPrefab;
    [SerializeField] public Sprite positiveImage;
    [SerializeField] public Sprite negativeImage;
    [SerializeField] public GameObject cardShadow;

    private int _cardValue;

    public bool IsFlipping = false;
    private flipStates flipState = flipStates.positive;

    private bool _isHoveringOverCard = false;
    private RectTransform _transform;
    private Image _image;
    private CardScoreVariables _cardScoreVariables;
    private Vector3 _originalPosition;
    private TextMeshProUGUI _cardValueText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _image = GetComponent<Image>();
        _transform = GetComponent<RectTransform>();
        _cardValueText = cardValueText.GetComponent<TextMeshProUGUI>();
        _cardScoreVariables = cardValueText.GetComponent<CardScoreVariables>();

        _cardValue = Convert.ToInt32(_cardValueText.text);
        switch (_cardValue)
        {
            case 0:
                _image.sprite = negativeImage;
                _cardValueText.color = _cardScoreVariables.OneColor;
                break;
            case 1:
                _image.sprite = positiveImage;
                _cardValueText.color = _cardScoreVariables.ZeroColor;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        _originalPosition = cardShadow.GetComponent<RectTransform>().position;

        _cardValue = Convert.ToInt32(_cardValueText.text);

        if (IsFlipping)
        {
            switch(flipState)
            {
                case flipStates.positive:
                    _transform.localScale = Vector3.Lerp(_transform.localScale, new Vector3(0, 1, 1), Time.deltaTime * 20);

                    if (_transform.localScale.x <= 0.1f)
                    {
                        _transform.localScale = new Vector3(0, 1, 1);
                        flipState = flipStates.negative;

                        switch (_cardValue)
                        {
                            case 0:
                                _image.sprite = negativeImage;
                                _cardValueText.color = _cardScoreVariables.OneColor;
                                break;
                            case 1:
                                _image.sprite = positiveImage;
                                _cardValueText.color = _cardScoreVariables.ZeroColor;
                                break;
                        }
                    }
                    break;
                case flipStates.negative:
                    _transform.localScale = Vector3.Lerp(_transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 20);

                    if (_transform.localScale.x >= 0.9f)
                    {
                        _transform.localScale = new Vector3(1, 1, 1);
                        flipState = flipStates.positive;
                        IsFlipping = false;
                    }
                    break;
            }
        }

        if (IsFlipping == false)
        {
            if (_isHoveringOverCard && GameLogic.canInteract == true)
            {
                var newPos = new Vector3()
                {
                    x = _originalPosition.x,
                    y = _originalPosition.y + 5,
                    z = _originalPosition.z
                };
                _transform.position = Vector3.Lerp(_transform.position, newPos, Time.deltaTime * 10);
                _transform.localScale = Vector3.Lerp(_transform.localScale, new Vector3(1.2f, 1.2f, 1.2f), Time.deltaTime * 10);
            }
            else
            {
                _transform.position = Vector3.Lerp(_transform.position, _originalPosition, Time.deltaTime * 10);
                _transform.localScale = Vector3.Lerp(_transform.localScale, new Vector3(1.0f, 1.0f, 1.0f), Time.deltaTime * 10);
            }
        }
    }

    public void bump()
    {
        var newPos = new Vector3()
        {
            x = _originalPosition.x,
            y = _originalPosition.y + 45,
            z = _originalPosition.z
        };
        _transform.position = newPos;
    }

    public string GetNumber()
    {
        return _cardValueText.text;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameLogic.canInteract == true)
        {
            _transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            bump();

            GameLogic.FinalScore = 0;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameLogic.canInteract == true && IsFlipping == false)
        {
            bump();

            IsFlipping = true;

            GameLogic.FinalScore = 0;

            Vibration.Vibrate(15);

            if (_cardValueText.text == "0")
            {
                _cardValue = 1;
                _cardValueText.text = "1";
            }
            else
            {
                _cardValue = 0;
                _cardValueText.text = "0";
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isHoveringOverCard = false;
    }
}
