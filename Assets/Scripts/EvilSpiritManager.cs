using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvilSpiritManager : MonoBehaviour
{
    [SerializeField] private GameObject _evilSpirit; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent<PlayerManager>(out PlayerManager pm)) 
        {
            if (_evilSpirit.activeInHierarchy) { _evilSpirit.SetActive(false); } else {_evilSpirit.SetActive(true); }
        }   
    }
}
