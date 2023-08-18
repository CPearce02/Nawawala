using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;


public class TutorialManager : MonoBehaviour
{
    public enum TutorialType
    {
        Null, PlayerLearnToSing, PlayersLearnHowToJump, PlayerLearnWhatObjectsAreSingable, PlayersLearnHowToControlSingingPitch, PlayersLearnToDoubleJump, PlayersLearnToDash
    }

    [Header("Tutorial shit")]
    [SerializeField] private Transform objectToSpawn; 
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private float _timeToGetThere;
    private float _currentTime;
    [SerializeField] private float spawnPadding = 0.5f; 
    [SerializeField] private float _spaceBetween;
    private List<Transform> _currentTrans;
    [SerializeField] private List<Vector3> _orgPosList;
    [SerializeField] private List<Vector3> _targetPosList;
    private Vector2 spawnAreaMin;
    private Vector2 spawnAreaMax;
    private int width;
    private int height;

    [Header("Actual Tutorial Texture")]
    [SerializeField] private Texture2D _playerLearnToSingTex;
    [SerializeField] private Texture2D _playerLearnHowToJumpTex;
    [SerializeField] private Texture2D _playerLearnWhatObjectsAreSingableTex;
    [SerializeField] private Texture2D _playerLearnHowToControlSingingPitchTex;
    [SerializeField] private Texture2D _playerLearnToDoubleJumpTex;
    [SerializeField] private Texture2D _playerLearnToDashTex;
    Texture2D tex;

    private void Start()
    {
        float screenHeight = Camera.main.orthographicSize * 2f;
        float screenWidth = screenHeight * Camera.main.aspect;

        spawnAreaMin = new Vector2(-screenWidth / 2 + spawnPadding - 5, -screenHeight / 2 + spawnPadding);
        spawnAreaMax = new Vector2(screenWidth / 2 - spawnPadding - 5, screenHeight / 2 - spawnPadding);

        foreach (Transform child in transform)
        {
            if(child.TryGetComponent<TutorialNodeBehaviour>(out TutorialNodeBehaviour tutorialNodeBehaviour))
            {
                tutorialNodeBehaviour.SetUp(this);
            }
        }
    }

    private void OnEnable()
    {
        GameEvents.unlockTutorial += StartTutorial;
    }

    private void OnDisable()
    {
        GameEvents.unlockTutorial -= StartTutorial;
    }

    private Texture2D GetTargetTexture(TutorialType targetTut)
    {
        Texture2D targetTex = null;

        switch (targetTut)
        {
            case TutorialType.PlayerLearnToSing:
                targetTex = _playerLearnToSingTex;
                break;
            case TutorialType.PlayersLearnHowToJump:
                targetTex = _playerLearnHowToJumpTex;
                break;
            case TutorialType.PlayerLearnWhatObjectsAreSingable:
                targetTex = _playerLearnWhatObjectsAreSingableTex;
                break;
            case TutorialType.PlayersLearnHowToControlSingingPitch:
                targetTex = _playerLearnHowToControlSingingPitchTex;
                break;
            case TutorialType.PlayersLearnToDoubleJump:
                targetTex = _playerLearnToDoubleJumpTex;
                break;
            case TutorialType.PlayersLearnToDash:
                targetTex = _playerLearnToDashTex;
                break;
        }

        return targetTex;
    }

    public void StartTutorial(TutorialType targetTut, Transform trans)
    {
        tex = GetTargetTexture(targetTut);
        Color[] pix = tex.GetPixels();

        width = tex.width;
        height = tex.height;

        ConvertImgToObjects(pix, tex, trans);
    }

    private void ConvertImgToObjects(Color[] colors, Texture2D tex, Transform trans)
    {
        _currentTrans = new List<Transform>();
        _orgPosList = new List<Vector3>();
        _targetPosList = new List<Vector3>();
        Vector3 startingSpot = trans.position;
        float ogXPos = startingSpot.x;
        int y = colors.Length-1;
        for (int x = 0; x < tex.height; x++)
        {
            y -= (tex.width);
            for (int u = 0; u < tex.width; u++)
            {
                if(y+u > 0)
                {
                    if(colors[y+u] != new Color(colors[y+u].r,colors[y+u].b,colors[y+u].g,0f))
                    {
                        SpawnObject(startingSpot);
                    }
                }
                startingSpot = new Vector3(startingSpot.x + _spaceBetween, startingSpot.y);
            }
            startingSpot = new Vector3(ogXPos, startingSpot.y - _spaceBetween);
        }

        StartCoroutine(StartMovingObjects());
    }

    private void SpawnObject(Vector3 objectTargetPos)
    {
        Vector3 randomPosition = new Vector3(
            UnityEngine.Random.Range(spawnAreaMin.x + objectTargetPos.x, spawnAreaMax.x+ objectTargetPos.x),
            UnityEngine.Random.Range(spawnAreaMin.y+ objectTargetPos.y, spawnAreaMax.y+ objectTargetPos.y),
            0
        );

        Transform tempTrans = Instantiate(objectToSpawn, randomPosition, Quaternion.identity, transform);
        _currentTrans.Add(tempTrans);
        _orgPosList.Add(randomPosition);
        _targetPosList.Add(objectTargetPos);
    }

    private IEnumerator StartMovingObjects()
    {
        float timeSpinning = _timeToGetThere;
        _currentTime = 0;
        while (timeSpinning > 0)
        {
            NativeArray<float3> positionArray = new NativeArray<float3>(_currentTrans.Count, Allocator.TempJob);
            TransformAccessArray transformAccessArray = new TransformAccessArray(_currentTrans.Count);

            Vector3 tempVec3;
            for (int i = 0; i < _currentTrans.Count; i++)
            {
                transformAccessArray.Add(_currentTrans[i]);       
                tempVec3 = Vector3.Lerp(_orgPosList[i], _targetPosList[i], _curve.Evaluate(_currentTime/_timeToGetThere));
                positionArray[i] = tempVec3;
            }
            
            _currentTime += Time.deltaTime;


            MovingSoulsTJob movingSoulsJob = new MovingSoulsTJob{
                moveArray = positionArray
            };

            JobHandle jobHandle = movingSoulsJob.Schedule(transformAccessArray);
            jobHandle.Complete();

            positionArray.Dispose();
            transformAccessArray.Dispose();
            timeSpinning -= Time.deltaTime;

            yield return null;
        }

    } 

    public struct MovingSoulsTJob : IJobParallelForTransform
    {
        public NativeArray<float3> moveArray;

        void IJobParallelForTransform.Execute(int index, TransformAccess transformAcc)
        {
            transformAcc.position = moveArray[index];
        }
    }
}
