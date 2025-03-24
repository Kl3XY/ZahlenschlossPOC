using UnityEngine;
using System;
using UnityEngine.UI;
using JetBrains.Annotations;
using UnityEditor;
using Unity.VisualScripting;
using static Unity.Burst.Intrinsics.X86.Avx;
using System.Collections.Generic;

public class GameLogic : MonoBehaviour
{
    static public bool canInteract = true;
    public static int FinalScore = 0;

    private Vector3 mousePosition;
    [SerializeField] private GameObject scoreNum;
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject gameScoreText;
    [SerializeField] private GameObject scoreToAimFor;
    [SerializeField] private GameObject _chainTextd;
    [SerializeField] private GameObject _roundTextd;
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject background;
    [SerializeField] private int chainCount = 1;
    private int amountCompleted = 1;
    private int teleportAmount = 0;
    public bool ActualSubmit = false;
    public bool IsSubmitting = false;
    [SerializeField] private double delay = 0;
    private double swapTeleportDelay = 0.5;
    private uint pos = 0;
    public int GameScore = 0;
    public double Timer = 240;
    private Image _image;
    private UnityEngine.Color _color = new Color(1, 1, 1);
    private UnityEngine.Color _defaultColor = new Color(1, 1, 1);
    private List<Color> colorList = new List<Color>();

    private DeckLogic _decklogic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;

        _decklogic = deckPrefab.GetComponent<DeckLogic>();
        _image = background.GetComponent<Image>();
        _defaultColor = _image.color;

        var scoreText = scoreToAimFor.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.text = UnityEngine.Random.Range(0, 20).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        _image.color = Color.Lerp(_image.color, _defaultColor, Time.deltaTime * 10);

        var scoreText = scoreNum.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.text = FinalScore.ToString();

        var chainText = _chainTextd.GetComponent<TMPro.TextMeshProUGUI>();
        chainText.text = $"Chain {chainCount}";

        var roundText = _roundTextd.GetComponent<TMPro.TextMeshProUGUI>();
        roundText.text = $"Round {amountCompleted}";
        //scoreText.color = Color.Lerp(scoreText.color, new Color(1f - _color.r, 1f - _color.g, 1f - _color.b), Time.deltaTime * 10);

        Timer -= Time.fixedDeltaTime;
        Timer = Math.Max(Timer, 0);

        TimeSpan time = TimeSpan.FromSeconds(Timer);
        var t_Text = timerText.GetComponent<TMPro.TextMeshProUGUI>();
        t_Text.text = time.ToString(@"hh\:mm\:ss");

        var s_Text = gameScoreText.GetComponent<TMPro.TextMeshProUGUI>();
        s_Text.text = GameScore.ToString().PadLeft(7, '0');

        canInteract = true;

        if (IsSubmitting)
        {
            swapTeleportDelay -= Time.fixedDeltaTime;
            if (swapTeleportDelay < 0 || teleportAmount == 0)
            {
                canInteract = false;
                delay -= Time.fixedDeltaTime;

                if (delay <= 0)
                {
                    var logic = _decklogic.GetCard((int)pos).GetComponentInChildren<CardLogic>();
                    logic.bump();

                    Vibration.Vibrate(5);

                    pos++;

                    if (logic.GetNumber() == "1")
                    {
                        FinalScore += Convert.ToInt32(generateBinary(pos), 2);
                        scoreText.text = FinalScore.ToString();
                        scoreNum.GetComponent<MadeNumberLogic>().bump();
                        delay = 10 * Time.fixedDeltaTime;
                    }
                    else
                    {
                        delay = 5 * Time.fixedDeltaTime;
                    }
                    if (pos >= 8)
                    {
                        IsSubmitting = false;
                        delay = 0;
                        pos = 0;
                        swapTeleportDelay = 0.5;

                        var Is = scoreNum.GetComponent<TMPro.TextMeshProUGUI>();
                        var toBe = scoreToAimFor.GetComponent<TMPro.TextMeshProUGUI>();

                        if (Is.text == toBe.text)
                        {
                            _image.color = new Color(1f, 1f, 1f);

                            toBe.text = Generate(chainCount).ToString();
                            FinalScore = 0;
                            scoreToAimFor.GetComponent<MadeNumberLogic>().bump();

                            chainCount++;

                            GameScore += 100 + 45 * chainCount;
                            Timer += Math.Max(15, 25 - 5 * amountCompleted);

                            amountCompleted += 1;

                            colorList.Clear();

                            var button = deckPrefab.GetComponent<DeckLogic>();
                            foreach (Transform child in deckPrefab.transform)
                            {
                                var comp = child.GetComponent<CardInfo>();
                                comp.setCardValue("0");
                                comp.flipCard();
                                comp.setTeleportStatus(modifiers.none, null, new Color());

                            }

                            if (amountCompleted > 10)
                            {
                                var a = Math.Clamp(Math.Floor((double)amountCompleted / 10), 0, 4);
                                teleportAmount = 0;

                                while (a > 0)
                                {
                                    teleportAmount += 1;
                                    RandomlySelectTwoTeleporters();
                                    a--;
                                }
                            }
                        }
                        else
                        {
                            chainCount = 1;
                            swapTeleports();
                            _image.color = new Color(1f, 0f, 0f);
                        }
                    }
                }
            }
        }
    }

    public GameObject randomlySelectTeleportableCard()
    {
        var childCount = deckPrefab.transform.childCount;
        var selectChildTeleportTo = UnityEngine.Random.Range(0, childCount);
        var telTo = deckPrefab.transform.GetChild(selectChildTeleportTo);
        while (telTo.GetComponent<CardInfo>().getTeleportStatus().Item1 != modifiers.none)
        {
            selectChildTeleportTo = UnityEngine.Random.Range(0, childCount);
            telTo = deckPrefab.transform.GetChild(selectChildTeleportTo);
        }
        telTo = deckPrefab.transform.GetChild(selectChildTeleportTo);

        return telTo.gameObject;
    }

    public void RandomlySelectTwoTeleporters()
    {
        Color randomColor = GenerateUniqueColor();

        colorList.Add(randomColor);

        var telTo = randomlySelectTeleportableCard();

        telTo.GetComponent<CardInfo>().setTeleportStatus(modifiers.teleportTo, null, randomColor);

        var telFrom = randomlySelectTeleportableCard();      

        telTo.GetComponent<CardInfo>().setTeleportStatus(modifiers.teleportTo, telFrom.gameObject, randomColor);
        telFrom.GetComponent<CardInfo>().setTeleportStatus(modifiers.teleportFrom, telTo.gameObject, randomColor);

    }

    Color GenerateUniqueColor()
    {
        int maxAttempts = 100; // Maximale Versuche, um eine passende Farbe zu finden
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            // HSV-Werte zufällig generieren
            float h = UnityEngine.Random.value; // Farbton (Hue)
            float s = UnityEngine.Random.Range(1.0f, 1f); // Sättigung (kräftige Farben)
            float v = UnityEngine.Random.Range(1.0f, 1f); // Helligkeit

            Color newColor = Color.HSVToRGB(h, s, v);
            newColor.a = 0.45f;

            // Prüfen, ob die Farbe einzigartig genug ist
            if (IsColorUnique(h))
            {
                return newColor;
            }
        }

        Debug.LogWarning("Maximale Versuche erreicht! Rückgabe einer möglichen Farbe.");
        return Color.HSVToRGB(UnityEngine.Random.value, 1f, 1f); // Notfallfarbe
    }

    bool IsColorUnique(float newHue)
    {
        foreach (Color color in colorList)
        {
            Color.RGBToHSV(color, out float existingHue, out _, out _);

            if (Mathf.Abs(existingHue - newHue) < 0.1f)
            {
                return false;
            }
        }
        return true;
    }

    public void swapTeleports()
    {
        foreach (Transform child in deckPrefab.transform)
        {
            var _f = child.GetComponent<CardInfo>().getTeleportStatus();
            if (_f.Item1 == modifiers.teleportFrom)
            {
                var _f2 = child.GetComponent<CardInfo>().getCardValue();
                var _f3 = _f.Item2.GetComponent<CardInfo>().getCardValue();

                _f.Item2.GetComponent<CardInfo>().setCardValue(_f2);
                _f.Item2.GetComponent<CardInfo>().flipCard();

                child.GetComponent<CardInfo>().setCardValue(_f3);
                child.GetComponent<CardInfo>().flipCard();
            }

        }
    }

    public static int Generate(int inp)
    {
        int numBits = (inp * 8) / (inp + 5);
        int leftBit = (inp * 8) / (inp + 2);

        int outValue = 0;

        for (int i = 0; i < numBits; i++)
        {
            int randomBit = UnityEngine.Random.Range(0, numBits+1);
            outValue |= 1 << randomBit;
        }

        return outValue;
    }

    private string generateBinary(uint pos)
    {
        var res = "";
        for (var i = 0; i <= 8; i++)
        {
            if (i == pos)
            {
                res += "1";
            }
            else
            {
                res += "0";
            }
        }
        return res;
    }
}
