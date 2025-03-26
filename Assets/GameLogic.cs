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
    static public bool AcceptPlayerInput = true;
    static public bool isHoveringOverCard = false;
    static public bool hasConfirmed = false;

    [SerializeField] private GameObject scoreNum;
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject gameScoreText;
    [SerializeField] private GameObject scoreToAimFor;
    [SerializeField] private GameObject _chainTextd;
    [SerializeField] private GameObject _roundTextd;
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject soundWin;
    [SerializeField] private GameObject soundLose;
    [SerializeField] private double delay = 0;

    private int teleportAmount = 0;
    public bool ActualSubmit = false;
    public bool IsSubmitting = false;
    public int DefaultBonusTimer = 5;
    private double swapTeleportDelay = 0.5;
    private uint pos = 0;
    private float soundPitch = 0.5f;

    private Image _image;
    private Color _color = new Color(1, 1, 1);
    private Color _defaultColor = new Color(1, 1, 1);

    /*
     * Score Related things
     */
    private int _finalScore = 0;
    private int _chainCount = 1;
    private int _rounds = 1;
    private double _timer = 30;
    private double _bonusTimer = 5;
    private int _gameScore = 0;

    /*
     * Get Setter
     */
    public int FinalScore
    {
        get
        {
            return _finalScore;
        }
        set
        {
            _finalScore = value;
            _scoreText.text = _finalScore.ToString();
        }
    }

    /*
     * Die länge der kette von dem Spieler.
     */
    public int ChainCount
    {
        get
        {
            return _chainCount;
        }
        set
        {
            _chainCount = value;
            _chainText.text = $"x {value}";
        }
    }

    /*
     * Die Anzahl der Runden
     */
    public int Rounds
    {
        get
        {
            return _rounds;
        }
        set
        {
            _rounds = value;
            _roundText.text = $"Round {value}";
        }
    }
    public double Timer
    {
        get
        {
            return _timer;
        }
        set
        {
            value = Math.Max(value, 0);

            TimeSpan time = TimeSpan.FromSeconds(value);
            _timerText.text = time.ToString(@"hh\:mm\:ss");
            _timer = value;
        }
    }
    public int GameScore
    {
        get
        {
            return _gameScore;
        }
        set
        {
            _gameScore = value;
            _gameScoreText.text = _gameScore.ToString().PadLeft(7, '0');
        }
    }

    public double BonusTimer
    {
        get
        {
            return _bonusTimer;
        }
        set
        {
            _bonusTimer = value;
        }
    }


    /*
     * Texts
     */
    private TMPro.TextMeshProUGUI _scoreText;
    private TMPro.TextMeshProUGUI _chainText;
    private TMPro.TextMeshProUGUI _roundText;
    private TMPro.TextMeshProUGUI _timerText;
    private TMPro.TextMeshProUGUI _gameScoreText;
    private TMPro.TextMeshProUGUI _currentEnteredScore;
    private TMPro.TextMeshProUGUI _shouldBeEnteredScore;
    private AudioSource _audioWin;
    private AudioSource _audioLose;

    /*
     * Logics
     */
    private MadeNumberLogic _madeNumberLogic;
    private MadeNumberLogic _currentMadeNumberLogic;

    private ColorGen _colorGen = new ColorGen();

    private DeckLogic _decklogic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;

        _decklogic = deckPrefab.GetComponent<DeckLogic>();
        _image = background.GetComponent<Image>();
        _defaultColor = _image.color;

        _scoreText = scoreNum.GetComponent<TMPro.TextMeshProUGUI>();
        _chainText = _chainTextd.GetComponent<TMPro.TextMeshProUGUI>();
        _roundText = _roundTextd.GetComponent<TMPro.TextMeshProUGUI>();
        _timerText = timerText.GetComponent<TMPro.TextMeshProUGUI>();
        _gameScoreText = gameScoreText.GetComponent<TMPro.TextMeshProUGUI>();
        _currentEnteredScore = scoreNum.GetComponent<TMPro.TextMeshProUGUI>();
        _shouldBeEnteredScore = scoreToAimFor.GetComponent<TMPro.TextMeshProUGUI>();
        _madeNumberLogic = scoreToAimFor.GetComponent<MadeNumberLogic>();
        _currentMadeNumberLogic = scoreNum.GetComponent<MadeNumberLogic>();

        _audioLose = soundLose.GetComponent<AudioSource>();
        _audioWin = soundWin.GetComponent<AudioSource>();

        _currentEnteredScore.text = "0";

        _shouldBeEnteredScore.text = UnityEngine.Random.Range(0, 20).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        _image.color = Color.Lerp(_image.color, _defaultColor, Time.deltaTime * 10);

        if ((isHoveringOverCard && AcceptPlayerInput) || hasConfirmed)
        {
            FinalScore = 0;
            hasConfirmed = false;
        }

        AcceptPlayerInput = true;

        if (IsSubmitting)
        {
            swapTeleportDelay -= Time.fixedDeltaTime;
            if (swapTeleportDelay < 0 || teleportAmount == 0)
            {
                AcceptPlayerInput = false;
                delay -= Time.fixedDeltaTime;

                if (delay <= 0)
                {
                    var logic = _decklogic.GetCard((int)pos).GetComponentInChildren<CardLogic>();
                    soundPitch += 0.1f;

#if !UNITY_EDITOR
    Handheld.Vibrate();
#endif

                    pos++;

                    if (logic.GetNumber() == "1")
                    {
                        FinalScore += Convert.ToInt32(generateBinary(pos), 2);
                        _scoreText.text = FinalScore.ToString();
                        _currentMadeNumberLogic.bump();
                        delay = 10 * Time.fixedDeltaTime;
                        logic.bump(true, soundPitch+0.2f);
                    }
                    else
                    {
                        delay = 5 * Time.fixedDeltaTime;
                        logic.bump(true, soundPitch);
                    }

                    if (pos >= 8)
                    {
                        refreshDelays();
                        if (_currentEnteredScore.text == _shouldBeEnteredScore.text)
                        {
                            _audioWin.Play();
                            finishRound();
                        }
                        else
                        {
                            _audioLose.Play();
                            ChainCount = 1;
                            SwapTeleports();
                            _image.color = new Color(1f, 0f, 0f);
                        }
                        soundPitch = 0.5f;
                    }
                }
            }
        } else
        {
            Timer -= Time.fixedDeltaTime;
            BonusTimer -= Time.fixedDeltaTime;
        }
    }

    private void refreshDelays() 
    {
        IsSubmitting = false;
        delay = 0;
        pos = 0;
        swapTeleportDelay = 0.5;
    }

    private void finishRound()
    {
        _image.color = new Color(1f, 1f, 1f);

        var difficulty = Rounds + ChainCount;
        Debug.Log(difficulty);

        _shouldBeEnteredScore.text = NumberGen.Generate(difficulty).ToString();
        FinalScore = 0;
        _madeNumberLogic.bump();

        ChainCount++;

        GameScore += 100 + 45 * ChainCount;
        Timer += Math.Max(15, 25 - 5 * Rounds);
        if (BonusTimer > 0)
        {
            GameScore += UnityEngine.Random.Range(45, 250) * ChainCount;
        }
        BonusTimer = Math.Max(2, DefaultBonusTimer-(ChainCount-1));

        Rounds += 1;

        _colorGen.clearColorList();

        resetTeleportStatusOnAllCards(modifiers.none, "0");

        if (Rounds % 10 == 0)
        {
            testScript.generateNewBG();
        }

        if (Rounds > 10)
        {
            var a = Math.Clamp(Math.Floor((double)Rounds / 10), 0, 4);
            teleportAmount = 0;

            while (a > 0)
            {
                teleportAmount += 1;
                RandomlySelectTwoTeleporters();
                a--;
            }
        }
    }

    public GameObject RandomlySelectTeleportableCard()
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

    private void resetTeleportStatusOnAllCards(modifiers mod, string val)
    {
        foreach (Transform child in deckPrefab.transform)
        {
            var comp = child.GetComponent<CardInfo>();
            comp.setCardValue(val);
            comp.flipCard();
            comp.setTeleportStatus(mod, null, new Color());

        }
    }

    public void RandomlySelectTwoTeleporters()
    {
        Color randomColor = _colorGen.GenerateUniqueColor();

        var telTo = RandomlySelectTeleportableCard();

        telTo.GetComponent<CardInfo>().setTeleportStatus(modifiers.teleportTo, null, randomColor);

        var telFrom = RandomlySelectTeleportableCard();      

        telTo.GetComponent<CardInfo>().setTeleportStatus(modifiers.teleportTo, telFrom.gameObject, randomColor);
        telFrom.GetComponent<CardInfo>().setTeleportStatus(modifiers.teleportFrom, telTo.gameObject, randomColor);

    }

    public void SwapTeleports()
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
