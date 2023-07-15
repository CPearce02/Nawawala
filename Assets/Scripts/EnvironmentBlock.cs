using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentBlock : MonoBehaviour
{
    [Header("My Objects")]
    [SerializeField] private List<GameObject> _objectsInThisBlock;

    
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        
    }
}
