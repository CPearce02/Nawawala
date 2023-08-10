using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _newGameButton;
    [SerializeField] private Button _chaptersPanelButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _exitGameButton;


    [Header("Scene IDS")]
    [SerializeField] private int _mainMenuID;
    [SerializeField] private int _gameSceneID;

    //[Header("Animations")]
    //[SerializeField] private Animator _mainMenuAnim;
    //[SerializeField] private AnimationClip _OutOfScreenAnimClip;

    
    private void Start()
    {
        _newGameButton.onClick.AddListener(NewGameFunction);
        _chaptersPanelButton.onClick.AddListener(ChaptersFunction);
        _settingsButton.onClick.AddListener(SettingsFunction);
        _exitGameButton.onClick.AddListener(ExitFunction);
    }
    
    private void NewGameFunction()
    {
        //SceneManager.LoadScene(_gameSceneID);
        gameObject.SetActive(false);
        GameEvents.gameStartSetUp?.Invoke();
        // GameManager.Instance.LoadThisScene(_gameSceneID);
    }

    private void ChaptersFunction()
    {
        
    }

    private void SettingsFunction()
    {
        
    }

    private void ExitFunction()
    {
        Application.Quit();
    }
}

