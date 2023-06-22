using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]private Transform _followObject;
    private float _currentOffSet = 0.75f;
    private float _offsetIncrement = 1.5f;
    private List<SoulManager> collectedSouls = new List<SoulManager>();

    private void OnEnable()
    {
        GameEvents.onSoulCollect += AddSoul;
    }
    private void OnDisable()
    {
        GameEvents.onSoulCollect -= AddSoul;
    }

    // Start is called before the first frame update
    void Start()
    {
        _followObject = GameObject.Find("FollowObj").transform;
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    private void AddSoul(SoulManager sm) 
    {
        sm.SetFollowAndOffset(_followObject.transform, _currentOffSet);
        //_currentOffSet += _offsetIncrement;
        collectedSouls.Add(sm);
        _followObject = collectedSouls[collectedSouls.Count - 1].transform;
    }

    private void OnReset()
    {
        SceneManager.LoadScene(0);
    }
}
