using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public int Score { get; private set; }

    public int RootRemain { get; private set; }
    public UnityAction OnInitialzed;

    [SerializeField]
    TMP_Text ScoreText;

    [SerializeField]
    TMP_Text RootRemainText;

    bool[] _hasAwarded; 

    private void Start()
    {
        
        Initialize();
    }

    private void Update()
    {
        ScoreText.text = "ÓªÑøÖ²:" + Score.ToString() ;
        RootRemainText.text = "Ê£Óà¸ù: " + RootRemain.ToString();
    }
    public void Initialize()
    {
        Score = 0;
        BuildManager.Instance.GenerateTileMap(50, 50);
        RootRemain = 10;
        _hasAwarded =  new bool[] { false, false,false,false};
        OnInitialzed.Invoke();
    }

    public void AddScore(int score)
    {
        Score += score;
        if (Score >4 && _hasAwarded[Score/5 - 1] == false)
        {
            RootRemain += 10;
            _hasAwarded[Score / 5 - 1] = true;
        }
    }

    public void Remove1Root()
    {
        if (RootRemain > 0)
            RootRemain--;
    }

    public void Add1Root()
    {
            RootRemain++;
    }






}
