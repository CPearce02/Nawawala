using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialNodeBehaviour : MonoBehaviour
{
    private TutorialManager _tutorialManager;
    [SerializeField] private TutorialManager.TutorialType _targetTutorial;
    [SerializeField] private Transform _startSpot;

    public void SetUp(TutorialManager tutorialManager)
    {
        _tutorialManager = tutorialManager;
    } 

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            _tutorialManager.StartTutorial(_targetTutorial, _startSpot);
            Destroy(gameObject);
        }
    }
}
