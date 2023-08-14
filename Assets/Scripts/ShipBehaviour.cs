using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipBehaviour : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve;
    [SerializeField] private Vector3 _orgPos;
    [SerializeField] private Vector3 _targetPos;
    [SerializeField] private float _timeToGetThere;
    [SerializeField] private bool _canMove;
    private float _currentTime;
    //private Transform _playerTrans;


    private void OnEnable() 
    {
        //_playerTrans = PlayerManager.PlayerTrans;
        GameEvents.gameStartSetUp += GameHasStarted;
    }

    private void OnDisable() 
    {
        GameEvents.gameStartSetUp -= GameHasStarted;
    }


    private void GameHasStarted()
    {
        _canMove = true;
    }
    
    void Update()
    {
        if(_canMove)
        {
            if(transform.position != _targetPos)
            {
                transform.position = Vector3.Lerp(_orgPos, _targetPos, _curve.Evaluate(_currentTime/_timeToGetThere));
                _currentTime += Time.deltaTime;
            }
            else
            {
                PlayerManager.PlayerTrans.SetParent(null);
                _canMove = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
            
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if(other.transform.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }
}
