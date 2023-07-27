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
        SpinAroundPlayer, ChaseEachOther
    }

    private List<LostSoulBehaviour> _lostSouls;
    private bool _hasLostSouls;
    private bool _notDoingRandomBehaviour;
    private int _maxNumOfSpecialBehaviour;

    [Header("Random Special Behaviours")]
    [SerializeField] private float _randomBehaviourMinTime;
    [SerializeField] private float _randomBehaviourMaxTime;
    private float _doRandomBehaviourTimer;

    [Header("SpinAroundPlayerBehaviour")]
    [SerializeField] private float calling;


    private void Awake() 
    {
        _lostSouls = new List<LostSoulBehaviour>();
    }

    void Start()
    {
        _maxNumOfSpecialBehaviour = System.Enum.GetValues(typeof(SpecialSoulBehaviours)).Length;
    }

    void Update()
    {
        if(_hasLostSouls)
        {
            if(_notDoingRandomBehaviour)
            {
                if(_doRandomBehaviourTimer <= 0)
                {
                    //ChooseRandomBehaviour();
                    ChooseBehaviour(SpecialSoulBehaviours.SpinAroundPlayer);
                    _doRandomBehaviourTimer = UnityEngine.Random.Range(_randomBehaviourMinTime, _randomBehaviourMaxTime);
                }
                else
                {
                    _doRandomBehaviourTimer -= Time.deltaTime;
                }
            }
        }
    }

    private void ChooseRandomBehaviour()
    {
        int ranNum =  UnityEngine.Random.Range(0, _maxNumOfSpecialBehaviour);
        ChooseBehaviour((SpecialSoulBehaviours)ranNum);
    }

    private void ChooseBehaviour(SpecialSoulBehaviours specialSoulBehaviours)
    {
        switch (specialSoulBehaviours)
        {
            case SpecialSoulBehaviours.SpinAroundPlayer:
                StartCoroutine(SpinningLostSoulBehaviour());
                break;
            case SpecialSoulBehaviours.ChaseEachOther:
                StartCoroutine(ChaseEachOtherBehaviour());
                break;
        }
    }

    private IEnumerator SpinningLostSoulBehaviour()
    {
        float timeSpinning = 10f;
        while (timeSpinning > 0)
        {
            NativeArray<float3> positionArray = new NativeArray<float3>(_lostSouls.Count, Allocator.TempJob);
            TransformAccessArray transformAccessArray = new TransformAccessArray(_lostSouls.Count);

            for (int i = 0; i < _lostSouls.Count; i++)
            {
                transformAccessArray.Add(_lostSouls[i].transform);
            }


            MovingSoulsTJob movingSoulsJob = new MovingSoulsTJob{
                //moveYArray = positionArray
                deltaTime = Time.deltaTime
            };

            JobHandle jobHandle = movingSoulsJob.Schedule(transformAccessArray);
            jobHandle.Complete();

            positionArray.Dispose();
            transformAccessArray.Dispose();
            timeSpinning -= Time.deltaTime;

            yield return null;
        }

        _notDoingRandomBehaviour = false;
    } 

    private IEnumerator ChaseEachOtherBehaviour()
    {
        yield return null;
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
    }

    public struct MovingSoulsTJob : IJobParallelForTransform
    {
        //public NativeArray<float3> moveYArray;
        public float deltaTime;
        void IJobParallelForTransform.Execute(int index, TransformAccess transformAcc)
        {
            transformAcc.position += new Vector3(0, deltaTime, 0);
        }
    }
}
