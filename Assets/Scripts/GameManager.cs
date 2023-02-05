using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Cinemachine;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    public int Score { get; private set; }

    public int RootRemain { get; private set; }
    public UnityAction OnInitialzed;

    [SerializeField]
    TMP_Text ScoreText;

    [SerializeField]
    TMP_Text RootRemainText;

    [SerializeField]
    TMP_Text RootText;

    bool[] _hasAwarded;

    [SerializeField]
    int[] _levelUPExperience;

    int _currentLevel;

    [SerializeField]
    int _rootAwardPerLevel;

    [SerializeField]
    GameObject _inGameUI;

    [SerializeField]
    GameObject _mainMenuUI;

    [SerializeField]
    GameObject _WinUI;


    [SerializeField]
    CinemachineVirtualCamera _mainMenuCam;

    [SerializeField]
    CinemachineVirtualCamera _gameCam;

    [SerializeField]
    Animator _flowerAnimator;

    [SerializeField]
    AudioClip _levelUpAudio;

    AudioSource _audioSource;
    private void Start()
    {
        _currentLevel = 5;
        _inGameUI.SetActive(false);
        _mainMenuUI.SetActive(true);
        _flowerAnimator.enabled = false;
        _WinUI.SetActive(false);
        _audioSource = GetComponent<AudioSource>();
       // Initialize();
    }

    private void Update()
    {
        ScoreText.text = "营养植:" + Score.ToString();
        RootText.text = "下一阶段: " + _levelUPExperience[_currentLevel - 1].ToString();
        RootRemainText.text = "剩余根: " + RootRemain.ToString();

        if (Score >= _levelUPExperience[3])
        {
            _WinUI.SetActive(true);
            
        }
            
    }
    public void Initialize()
    {
        Score = 0;

        RootRemain = 10;
        _hasAwarded =  new bool[] { false, false,false,false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        OnInitialzed.Invoke();
        _currentLevel = 1;

        BuildManager.Instance.GenerateTileMap(50, 60);

        _flowerAnimator.enabled = true;

        _WinUI.SetActive(false);
    }

    public void AddScore(int score)
    {
        Score += score;

        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        if (Score - _levelUPExperience[_currentLevel-1] >= 0 && !_hasAwarded[_currentLevel-1])
        {
            _hasAwarded[_currentLevel-1] = true;
            RootRemain += _rootAwardPerLevel;
            _currentLevel++;

            _audioSource.PlayOneShot(_levelUpAudio);
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

    public int GetLevel()
    {
        return _currentLevel;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameStart()
    {
        Initialize();
        _gameCam.Priority = 100;
        _mainMenuUI.SetActive(false);
        _inGameUI.SetActive(true);
    }




}
