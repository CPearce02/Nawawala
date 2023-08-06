using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerManager : MonoBehaviour
{
    private static PlayerManager Instance;
    [SerializeField] private Transform _player;
    public static Transform PlayerTrans{get; private set;} 

    [SerializeField] private Transform _followObject;
    private float _currentOffSet = 0.75f;
    public List<SoulManager> collectedSouls = new List<SoulManager>();
    public event EventHandler _addExtraPlayerJump;

    private void Awake() 
    {   
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        if(_followObject == null)
        {
            _followObject = GameObject.Find("FollowObj").transform;
        }
        
        if(_player == null)
        {
            _player = FindObjectOfType<PlayerController>().transform;
        }
        PlayerTrans = _player;
    }

    private void Start()
    {
        _player.GetComponent<PlayerController>().Init(this);
    }

    public void CallPlayerExtra()
    {
        _addExtraPlayerJump?.Invoke(this, null);
    }

    private void OnEnable()
    {
        GameEvents.onSoulCollect += AddSoul;
    }
    private void OnDisable()
    {
        GameEvents.onSoulCollect -= AddSoul;
    }


    private void AddSoul(SoulManager sm) 
    {
        sm.SetFollowAndOffset(_followObject.transform, _currentOffSet);
        collectedSouls.Add(sm);
        _followObject = collectedSouls[collectedSouls.Count - 1].transform;
    }

    public void DisperseSouls() 
    {
        StartCoroutine(DisperseSoulsCoroutine());
    }

    private IEnumerator DisperseSoulsCoroutine()
    {
        if (collectedSouls.Count <= 0) yield return null;
        foreach (SoulManager sm in collectedSouls)
        {
            sm.isFollowing = false;
            float randomAngle = UnityEngine.Random.Range(0f, 180f);
            Vector2 awayDirection = Quaternion.Euler(0f, 0f, randomAngle) * Vector2.up;
            Rigidbody2D rb = sm.GetComponent<Rigidbody2D>();
            rb.AddForce(awayDirection * 15f, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.5f);

            rb.velocity = Vector2.zero;
        }

        _followObject = GameObject.Find("FollowObj").transform;
        collectedSouls.Clear();

    }

    private void OnReset()
    {
        SceneManager.LoadScene(0);
    }
}
