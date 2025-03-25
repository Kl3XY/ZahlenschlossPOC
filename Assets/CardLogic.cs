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

public enum modifiers
{
    none,
    teleportTo,
    teleportFrom
}

public class CardLogic : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Dependencies and stuff")]
    [SerializeField] public GameObject cardValueText;
    [SerializeField] public GameObject scoreNumCreateTextPrefab;
    [SerializeField] public Sprite positiveImage;
    [SerializeField] public Sprite negativeImage;
    [SerializeField] public GameObject cardShadow;
    [Space(20)]

    [Header("Customizeables")]
    public modifiers currentModifier = modifiers.none;
    public Color modifierColor = new Color(1, 1, 1);
    [Space(20)]

    [Header("Subtract, Multiply and Bitshift FX")]
    public Image TeleportFX;
    [Space(20)]
    public GameObject attachedGameObject;

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
        TeleportFX.enabled = false;

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

    public static T Choose<T>(params T[] options)
    {
        if (options == null || options.Length == 0)
            throw new ArgumentException("Array isn't long enough.");

        return options[UnityEngine.Random.Range(0, options.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        _originalPosition = cardShadow.GetComponent<RectTransform>().position;

        _cardValue = Convert.ToInt32(_cardValueText.text);

        TeleportFX.GetComponent<Image>().color = modifierColor;

        if (currentModifier != modifiers.none)
        {
            TeleportFX.enabled = true;
        } else
        {
            TeleportFX.enabled = false;
        }

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
            if (_isHoveringOverCard && GameLogic.AcceptPlayerInput == true)
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

    public void SetNumber(string num)
    {
        _cardValueText.text = num;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameLogic.AcceptPlayerInput == true)
        {
            _transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            bump();
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameLogic.AcceptPlayerInput == true && IsFlipping == false)
        {
            bump();

            IsFlipping = true;

            GameLogic.isHoveringOverCard = true;

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
        GameLogic.isHoveringOverCard = false;
    }
}
