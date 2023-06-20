using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Scene IDs")]
    public const int MAINMENUSCENE = 0;
    public const int STORYSCENE = 1;
    public const int CHALLENGESCENE = 2;
    public const int ARCADESCENE = 4;
    public const int GAMESCENE = 3;


    [Header("Scene Control Variables")]
    [SerializeField] private int _lastSceneID;
    [SerializeField] private int _currentSceneID;
    //public static bool isGameFrozen;

    [Header("Saving Data Variables")]
    const string SAVEDATAPATH = "gameData";


    public int CurrentSceneID{get{return _currentSceneID;} private set{_currentSceneID = value;}}
    public int LastSceneID{get{return _lastSceneID;} private set{_lastSceneID = value;}}

    private void Awake() 
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _currentSceneID = SceneManager.GetActiveScene().buildIndex;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() 
    {
        GameEvents.finishStartGame += UnFreezeGame;
        GameEvents.finsihEndGame += FreezeGame;
    }

    private void OnDisable() 
    {
        GameEvents.finishStartGame -= UnFreezeGame;
        GameEvents.finsihEndGame -= FreezeGame;
    }

    public void LoadThisScene(int levelID)
    {
        Debug.Log("Load Scene");
        StartCoroutine(LoadSceneAsync(levelID));
    }

    IEnumerator LoadSceneAsync(int levelID)
    {
        _lastSceneID = SceneManager.GetActiveScene().buildIndex;
        AsyncOperation operation = SceneManager.LoadSceneAsync(levelID);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            yield return null;
        }
        
        _currentSceneID = SceneManager.GetActiveScene().buildIndex;
    }

    public IEnumerator StartGame()
    {
        //Makes sure that the Scene is the GAME Scene so that the events that are called are actually received
        while (_currentSceneID != GAMESCENE)
        {
            yield return new WaitForSecondsRealtime(0);
        }
        GameEvents.gameStartSetUp?.Invoke();;
    }

    /*
    #region SAVE AND LOAD SYSTEM METHODS
    //Main Save and Load Methods
    private void SaveGameData()
    {
        DateTime dateQuit = DateTime.Now;
        SaveData.current.timeOfSave = dateQuit;
        SaveData.current.totalEnergy = GameGlobalData.PlayerEnergy;
        SaveData.current.currentEnergyTimer = EnergyManager.eMInstance.timeTillNextEnergy;
        
        //Saved Challenge Level Datas
        //SaveData.current.savedEasyLevels = CheckSaveThisLevelData(SaveData.current.savedEasyLevels, ChallengeLevelManager.Instance.easyChallengeLevelDatas);
        //SaveData.current.savedNormalLevels = CheckSaveThisLevelData(SaveData.current.savedNormalLevels, ChallengeLevelManager.Instance.normalChallengeLevelDatas);
        //SaveData.current.savedHardLevels = CheckSaveThisLevelData(SaveData.current.savedHardLevels, ChallengeLevelManager.Instance.hardChallengeLevelDatas);
        //SaveData.current.savedExtremeLevels = CheckSaveThisLevelData(SaveData.current.savedExtremeLevels, ChallengeLevelManager.Instance.extremeChallengeLevelDatas);

        SaveData.current.savedMechanicsDictionary = FoundMechanics.mechanicsDictionary;
        //Saves all Data to a Folder
        SaveManager.Save(SAVEDATAPATH, SaveData.current);
    }

    private void LoadGameData()
    {
        SaveData.current = (SaveData)SaveManager.Load(Application.persistentDataPath + "/saves/" + SAVEDATAPATH +".save");
        UpdateValues();
        //GameGlobalData.PlayerEnergy = 25;
    }

    //HELPER METHODS
    //This method is called to unload Load all the data that we saved from last play session
    private void UpdateValues()
    {
        GameGlobalData.PlayerEnergy = SaveData.current.totalEnergy;

        TimeSpan timespan = DateTime.Now - SaveData.current.timeOfSave;
        float timePasedInSeconds = (float)timespan.TotalSeconds;
        if(EnergyManager.eMInstance != null)
        {
            EnergyManager.eMInstance.GameLoadedSetUp(timePasedInSeconds);
        }

        //Load Challenge Level Datas and checks if the game has been saved before
        if(SaveData.current.savedEasyLevels != null)
        {
            LoadOutChallengeLevelData(ChallengeLevelManager.Instance.easyChallengeLevelDatas, SaveData.current.savedEasyLevels);
        }
        if(SaveData.current.savedNormalLevels != null)
        {
            LoadOutChallengeLevelData(ChallengeLevelManager.Instance.normalChallengeLevelDatas, SaveData.current.savedNormalLevels);
        }
        if(SaveData.current.savedHardLevels != null)
        {
            LoadOutChallengeLevelData(ChallengeLevelManager.Instance.hardChallengeLevelDatas, SaveData.current.savedHardLevels);
        }
        if(SaveData.current.savedExtremeLevels != null)
        {
            LoadOutChallengeLevelData(ChallengeLevelManager.Instance.extremeChallengeLevelDatas, SaveData.current.savedExtremeLevels);
        }

        if(SaveData.current.savedMechanicsDictionary.Count == 0)
        {
            FillUpMechanicsList();
        }
        FoundMechanics.mechanicsDictionary = SaveData.current.savedMechanicsDictionary;
    }

    //Checks if Challenge Level Data has been saved before and if it hasn't, creates room for it
    private SaveChallengeLevelData CheckSaveThisLevelData(SaveChallengeLevelData saveLevelData, List<ChallengeLevelData> ChallengeLevelData)
    {
        if(saveLevelData != null)
        {
            SaveAllChallengeLevelData(saveLevelData, ChallengeLevelData);
        }
        else
        {
            //Initialising Challenge Level Data that is needed to be saved
            saveLevelData = new SaveChallengeLevelData();
            saveLevelData.unlocked = new List<bool>();
            saveLevelData.hasCompletion = new List<bool>();
            saveLevelData.totalStarsEarnt = new List<int>();
            saveLevelData = FillSaveDataChallengeLevelData(saveLevelData, ChallengeLevelData);
            SaveAllChallengeLevelData(saveLevelData, ChallengeLevelData);
        }
        return saveLevelData;
    }

    //Assigns the new data that the player has changed through their ending playthrough into SaveLevelData
    private void SaveAllChallengeLevelData(SaveChallengeLevelData targetSaveLevelData, List<ChallengeLevelData> targetChallengeLevelData)
    {
        //During Developments checks for increased levels in a specific difficulty
        if(targetSaveLevelData.unlocked.Count != targetChallengeLevelData.Count)
        {
            targetSaveLevelData = FillSaveDataChallengeLevelData(targetSaveLevelData, targetChallengeLevelData);
        }

        for (int i = 0; i < targetChallengeLevelData.Count; i++)
        {
            targetSaveLevelData.unlocked[i] = targetChallengeLevelData[i].unlocked;
            targetSaveLevelData.hasCompletion[i] = targetChallengeLevelData[i].hasCompletion;
            targetSaveLevelData.totalStarsEarnt[i] = targetChallengeLevelData[i].totalStarsEarnt;
        }
    }

    //Creates entries when Save Level Data has not been Initialised properly so that we can assign values
    private SaveChallengeLevelData FillSaveDataChallengeLevelData(SaveChallengeLevelData targetSaveLevelData, List<ChallengeLevelData> targetChallengeLevelData)
    {
        targetSaveLevelData.unlocked.Clear();
        targetSaveLevelData.hasCompletion.Clear();
        targetSaveLevelData.totalStarsEarnt.Clear();
        for (int i = 0; i < targetChallengeLevelData.Count; i++)
        {
            targetSaveLevelData.unlocked.Add(targetChallengeLevelData[i].unlocked);
            targetSaveLevelData.hasCompletion.Add(targetChallengeLevelData[i].hasCompletion);
            targetSaveLevelData.totalStarsEarnt.Add(targetChallengeLevelData[i].totalStarsEarnt);
        }  
        return targetSaveLevelData;
    }

    //Loads specific Challenge Level Data variables from <Script>(SaveLevelData) into ChallengeLevelData 
    private void LoadOutChallengeLevelData(List<ChallengeLevelData> targetChallengeLevelData, SaveChallengeLevelData targetSaveLevelData)
    {
        for (int i = 0; i < targetChallengeLevelData.Count; i++)
        {
            targetChallengeLevelData[i].unlocked = targetSaveLevelData.unlocked[i];
            targetChallengeLevelData[i].hasCompletion = targetSaveLevelData.hasCompletion[i];
            targetChallengeLevelData[i].totalStarsEarnt = targetSaveLevelData.totalStarsEarnt[i];
        }
    }

    private void FillUpMechanicsList()
    {
        FoundMechanics.mechanicsDictionary = new Dictionary<FoundMechanics.mechanicsList, bool>();
        foreach (FoundMechanics.mechanicsList mechanicType in Enum.GetValues(typeof(FoundMechanics.mechanicsList)))
        {
            FoundMechanics.mechanicsDictionary.Add(mechanicType, false);
        }
    }

    public void UpdateChallengeLevelSave(Difficulty targetDifficulty)
    {
        switch (targetDifficulty)
        {
            case Difficulty.Easy:
                CheckSaveThisLevelData(SaveData.current.savedEasyLevels, ChallengeLevelManager.Instance.easyChallengeLevelDatas);           
                break;
            case Difficulty.Normal:
                CheckSaveThisLevelData(SaveData.current.savedNormalLevels, ChallengeLevelManager.Instance.normalChallengeLevelDatas);
                break;
            case Difficulty.Hard:
                CheckSaveThisLevelData(SaveData.current.savedHardLevels, ChallengeLevelManager.Instance.hardChallengeLevelDatas);
                break;
            case Difficulty.Extreme:
                CheckSaveThisLevelData(SaveData.current.savedExtremeLevels, ChallengeLevelManager.Instance.extremeChallengeLevelDatas);
                break;
        }
    }
    #endregion

    private void OnApplicationFocus(bool focusStatus) 
    {
        if(!focusStatus)
        {
            //Debug.Log("Saved");
            //SaveGameData();
        }
        else
        {
            //Debug.Log("Loaded");
            //LoadGameData();
        }
    }
    
    */
    public void UnFreezeGame()
    {
        Debug.Log("Unfreeze");
        Time.timeScale = 1;
        GameEvents.gameFreezeState?.Invoke(true);
        //isGameFrozen = false;
    }

    public void FreezeGame(bool x)
    {
        Debug.Log("freeze");
        Time.timeScale = 0;
        GameEvents.gameFreezeState?.Invoke(false);
        //isGameFrozen = true;
    }
}
