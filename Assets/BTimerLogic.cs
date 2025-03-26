using System;
using UnityEngine;

public class BTimerLogic : MonoBehaviour
{
    [SerializeField] GameObject Game;

    private GameLogic _gameLogic;
    private RectTransform _transfrom;

    private Vector3 _startPosition;
    private Vector3 _endPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameLogic = Game.GetComponent<GameLogic>();
        _transfrom = GetComponent<RectTransform>();
        _startPosition = _transfrom.localPosition;
        _startPosition = new Vector3(
            _transfrom.localPosition.x - 277 * 2,
            _transfrom.localPosition.y,
            _transfrom.localPosition.z
            );
    }

    // Update is called once per frame
    void Update()
    {
        var max = Math.Max(2, _gameLogic.DefaultBonusTimer - (_gameLogic.ChainCount - 1));
        var pDiff = _gameLogic.BonusTimer / max;
        _transfrom.localPosition = Vector3.Lerp(_startPosition, _endPosition, (float)pDiff);


    }
}
