using UnityEngine;
using System;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    static public bool canInteract = true;
    public static int FinalScore = 0;

    private Vector3 mousePosition;
    [SerializeField] private GameObject scoreNum;
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject gameScoreText;
    [SerializeField] private GameObject scoreToAimFor;
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject background;
    public bool IsSubmitting = false;
    [SerializeField] private double delay = 0;
    private int pos = 0;
    public int GameScore = 0;
    public double Timer = 240;
    private Image _image;
    private UnityEngine.Color _color = new Color(1, 1, 1);
    private UnityEngine.Color _defaultColor = new Color(1, 1, 1);

    private DeckLogic _decklogic;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;

        _decklogic = deckPrefab.GetComponent<DeckLogic>();
        _image = background.GetComponent<Image>();
        _defaultColor = _image.color;

        var scoreText = scoreToAimFor.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.text = UnityEngine.Random.Range(0, 255).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        _image.color = Color.Lerp(_image.color, _defaultColor, Time.deltaTime * 10);

        var scoreText = scoreNum.GetComponent<TMPro.TextMeshProUGUI>();
        scoreText.text = FinalScore.ToString();
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
            canInteract = false;
            delay -= Time.fixedDeltaTime;

            if (delay <= 0)
            {
                var logic = _decklogic.GetCard(pos).GetComponentInChildren<CardLogic>();
                logic.bump();

                Vibration.Vibrate(5);

                pos++;
                
                if (logic.GetNumber() == "1")
                {
                    FinalScore += Convert.ToInt32(generateBinary(pos), 2);
                    scoreText.text = FinalScore.ToString();
                    scoreNum.GetComponent<MadeNumberLogic>().bump();
                    delay = 10 * Time.fixedDeltaTime;
                } else
                {
                    delay = 5 * Time.fixedDeltaTime;
                }
            }

            if (pos >= 8)
            {
                IsSubmitting = false;
                delay = 0;
                pos = 0;

                var Is = scoreNum.GetComponent<TMPro.TextMeshProUGUI>();

                var toBe = scoreToAimFor.GetComponent<TMPro.TextMeshProUGUI>();

                if (Is.text == toBe.text)
                {
                    _image.color = new Color(1f, 1f, 1f);

                    toBe.text = UnityEngine.Random.Range(0, 255).ToString();
                    FinalScore = 0;
                    scoreToAimFor.GetComponent<MadeNumberLogic>().bump();

                    GameScore += 100;
                    Timer += 15;

                    var button = deckPrefab.GetComponent<DeckLogic>();
                    foreach (Transform child in deckPrefab.transform)
                    {
                        var comp = child.GetComponent<CardInfo>();
                        comp.setCardValue("0");
                        comp.flipCard();

                    }
                }
            }
        }
    }

    private string generateBinary(int pos)
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
