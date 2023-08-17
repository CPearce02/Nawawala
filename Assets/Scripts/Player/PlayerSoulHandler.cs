using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class PlayerSoulHandler : MonoBehaviour
{
    private enum SpecialSoulBehaviours
    {
        SpinAroundPlayer, ChaseEachOther, DrawShapes, BlockPlayer
    }

    private bool _hasLostSouls;
    private bool _notDoingRandomBehaviour;
    private int _maxNumOfSpecialBehaviour;

    [Header("Lost Soul Control Variables")]
    [SerializeField] private float _normalFollowSpeed;
    [SerializeField] private float _normalSpinSpeed;
    [SerializeField] private float _spaceBetweenEachLostSoul;
    [SerializeField] private List<LostSoulBehaviour> _lostSouls;


    [Header("Random Special Behaviours")]
    [SerializeField] private float _randomBehaviourMinTime;
    [SerializeField] private float _randomBehaviourMaxTime;
    private float _doRandomBehaviourTimer;


    // [Header("SpinAroundPlayerBehaviour")]
    // [SerializeField] private float calling;
    [Header("Misc")]
    [SerializeField] private List<Transform> _currentFollowingSouls;
    private Transform _playerTrans;
    float step;

    private void Awake() 
    {
        _lostSouls = new List<LostSoulBehaviour>();
        _currentFollowingSouls = new List<Transform>();
    }

    void Start()
    {
        _maxNumOfSpecialBehaviour = System.Enum.GetValues(typeof(SpecialSoulBehaviours)).Length;
        _playerTrans = PlayerManager.PlayerTrans;
        step = _normalSpinSpeed * Time.deltaTime;
    }

    void Update()
    {
        if(_hasLostSouls)
        {
            //Controls the lost souls normally folling 
            if(_currentFollowingSouls.Count > 0)
            {
                Transform lastTrans = _playerTrans;
                List<Vector3> chickenBalls = new List<Vector3>();

                foreach (Transform soul in _currentFollowingSouls)
                {
                    if(Vector3.Distance(soul.transform.position, lastTrans.position) > _spaceBetweenEachLostSoul)
                    {
                        // Calculate the distance between the object and the player
                        float distance = Vector2.Distance(soul.transform.position, lastTrans.position);

                        // Calculate the normalized speed based on the distance
                        float normalizedSpeed = Mathf.InverseLerp(2.5f, 90f, distance);

                        // Calculate the actual speed by lerping between the minimum and maximum speed
                        float extraSpeed = Mathf.Lerp(1, 3, normalizedSpeed);

                        Vector3 comeOnMan = Vector3.MoveTowards(soul.transform.position, lastTrans.position, Time.deltaTime*_normalFollowSpeed*extraSpeed);
                        chickenBalls.Add(comeOnMan);
                    }
                    else
                    {
                        //Idle movement maybe
                        chickenBalls.Add(soul.transform.position);
                    }
                    lastTrans = soul.transform;
                }

                MoveSouls(_currentFollowingSouls, chickenBalls);
            }

            //Controls when lost souls go do random behaviours
            if(_notDoingRandomBehaviour)
            {
                if(_doRandomBehaviourTimer <= 0)
                {
                    //ChooseRandomBehaviour();
                    ChooseBehaviour(SpecialSoulBehaviours.ChaseEachOther);
                    _doRandomBehaviourTimer = UnityEngine.Random.Range(_randomBehaviourMinTime, _randomBehaviourMaxTime);
                    _notDoingRandomBehaviour = false;
                }
                else
                {
                    _doRandomBehaviourTimer -= Time.deltaTime;
                }
            }

            //Control Spinning the Lost Souls
            for (int i = 0; i < _currentFollowingSouls.Count; i++)
            {
                _currentFollowingSouls[i].rotation = Quaternion.RotateTowards(Quaternion.identity, Quaternion.Euler(0,0,180), step); 
            }

            step -= _normalSpinSpeed * Time.deltaTime;
        }
    }

    private void ChooseRandomBehaviour()
    {
        int ranNum =  UnityEngine.Random.Range(0, _maxNumOfSpecialBehaviour);
        ChooseBehaviour((SpecialSoulBehaviours)ranNum);
    }

    private void ChooseBehaviour(SpecialSoulBehaviours specialSoulBehaviours)
    {
        Debug.Log("Beahviour getting picked");
        switch (specialSoulBehaviours)
        {
            case SpecialSoulBehaviours.BlockPlayer:
                StartCoroutine(SpinInSpotBehaviour());
                break;
            case SpecialSoulBehaviours.ChaseEachOther:
                if(_currentFollowingSouls.Count >= 2)
                {
                    StartCoroutine(ChaseEachOtherBehaviour());
                }
                break;
            case SpecialSoulBehaviours.DrawShapes:
                StartCoroutine(ChaseEachOtherBehaviour());
                break;
        }
    }

    private IEnumerator SpinInSpotBehaviour()
    {
        float timeSpinning = 10f;

        List<Transform> balls = new List<Transform>();
        List<Quaternion> chickenBalls = new List<Quaternion>();

        foreach (var item in _currentFollowingSouls)
        {
            balls.Add(item);
        }

        float bsstep = 60 * Time.deltaTime;
        
        //Quaternion targetRotation;
        while (timeSpinning > 0)
        {
            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].rotation = Quaternion.RotateTowards(Quaternion.identity, Quaternion.Euler(0,0,-180), bsstep); 
                //chickenBalls.Add(bs);
            }

            //RotateSouls(balls, chickenBalls);

            bsstep -= 60 * Time.deltaTime;

            timeSpinning -= Time.deltaTime;

            yield return null;
        }

        _notDoingRandomBehaviour = true;
    } 

    private IEnumerator ChaseEachOtherBehaviour()
    {
        float timeChasing = 4f;
        List<Transform> balls = new List<Transform>();
        List<Vector3> chickenBalls = new List<Vector3>();
        
        int targetRan = 0;

        for (int i = 0; i < 2; i++)
        {
            targetRan = UnityEngine.Random.Range(0, _currentFollowingSouls.Count-1);
            balls.Add(_currentFollowingSouls[targetRan]);
            _currentFollowingSouls.RemoveAt(targetRan);
        }

        Vector3 chasePoint;
        bool chaserCanGo = false;

        float ranX = UnityEngine.Random.Range(-10, 10);
        do
        {
            ranX = UnityEngine.Random.Range(-10, 10);
        } while (ranX == 0);

        float ranY = UnityEngine.Random.Range(-10, 10);
        do
        {
            ranY = UnityEngine.Random.Range(-10, 10);
        } while (ranY == 0);
        
        chasePoint = new Vector3(ranX + balls[0].position.x, ranY + balls[0].position.y, 0);

        while (timeChasing > 0)
        {
            if(!chaserCanGo)
            {
                if(Vector3.Distance(balls[0].position, chasePoint) > 0.05f)
                {
                    Vector3 comeOnMan = Vector3.MoveTowards(balls[0].position, chasePoint, Time.deltaTime*40);
                    chickenBalls.Add(comeOnMan);
                    chickenBalls.Add(balls[1].position);
                    MoveSouls(balls, chickenBalls);
                }
                else
                {
                    chaserCanGo = true;
                }
            }
            else
            {
                if(Vector3.Distance(balls[1].position, chasePoint) > 1.5f)
                {
                    Vector3 comeOnMan = Vector3.MoveTowards(balls[1].position, chasePoint, Time.deltaTime*40);
                    chickenBalls.Add(balls[0].position);
                    chickenBalls.Add(comeOnMan);
                    MoveSouls(balls, chickenBalls);
                }
                else
                {
                    ranX = UnityEngine.Random.Range(-10, 10);
                    do
                    {
                        ranX = UnityEngine.Random.Range(-10, 10);
                    } while (ranX == 0);

                    ranY = UnityEngine.Random.Range(-10, 10);
                    do
                    {
                        ranY = UnityEngine.Random.Range(-10, 10);
                    } while (ranY == 0);
                    
                    chasePoint = new Vector3(ranX + balls[0].position.x, ranY + balls[0].position.y, 0);
                    chaserCanGo = false;
                }
            }
            
            timeChasing -= Time.deltaTime;

            chickenBalls.Clear();

            yield return null;
        }

        foreach (var item in balls)
        {
            _currentFollowingSouls.Add(item);  
        }
        _notDoingRandomBehaviour = true;
    } 

    public void AddMeToSouls(LostSoulBehaviour lostSoulBehaviour)
    {
        if(_lostSouls.Count == 0)
        {
            _doRandomBehaviourTimer = UnityEngine.Random.Range(_randomBehaviourMinTime, _randomBehaviourMaxTime);
            _notDoingRandomBehaviour = true;
            _hasLostSouls = true;
        }
        _lostSouls.Add(lostSoulBehaviour);
        _currentFollowingSouls.Add(lostSoulBehaviour.transform);
    }

    private void MoveSouls(List<Transform> transToMove, List<Vector3> posToMoveTo)
    {
        NativeArray<float3> positionArray = new NativeArray<float3>(transToMove.Count, Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray(transToMove.Count);

        //Debug.Log(transToMove.Count);
        for (int i = 0; i < transToMove.Count; i++)
        {
            transformAccessArray.Add(transToMove[i].transform);
            positionArray[i] = posToMoveTo[i];
        }


        MovingSoulsTJob movingSoulsJob = new MovingSoulsTJob{
            moveArray = positionArray
            //deltaTime = Time.deltaTime
        };

        JobHandle jobHandle = movingSoulsJob.Schedule(transformAccessArray);
        jobHandle.Complete();

        positionArray.Dispose();
        transformAccessArray.Dispose();
    }

    private void RotateSouls(List<Transform> transToMove, List<Quaternion> rotateChange)
    {
        //NativeArray<float3> positionArray = new NativeArray<float3>(transToMove.Count, Allocator.TempJob);
        TransformAccessArray transformAccessArray = new TransformAccessArray(transToMove.Count);

        //Debug.Log(transToMove.Count);
        for (int i = 0; i < transToMove.Count; i++)
        {
            transformAccessArray.Add(transToMove[i].transform);
            //positionArray[i] = posToMoveTo[i];
        }


        RotatingSoulsTJob movingSoulsJob = new RotatingSoulsTJob{
            //rotateArray = rotateChange
            //deltaTime = Time.deltaTime
        };

        JobHandle jobHandle = movingSoulsJob.Schedule(transformAccessArray);
        jobHandle.Complete();

        //positionArray.Dispose();
        transformAccessArray.Dispose();
    }

    public struct MovingSoulsTJob : IJobParallelForTransform
    {
        public NativeArray<float3> moveArray;
        void IJobParallelForTransform.Execute(int index, TransformAccess transformAcc)
        {
            transformAcc.position = moveArray[index];
        }
    }

    public struct RotatingSoulsTJob : IJobParallelForTransform
    {
        public List<TransformAccess> rotateArray;
        void IJobParallelForTransform.Execute(int index, TransformAccess transformAcc)
        {
            transformAcc.rotation = rotateArray[index].rotation;
        }
    }
}
