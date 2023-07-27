using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LostSoulBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collider2D _detectPlayerCol;
    [SerializeField] private Collider2D _pickUpCol;
    private SoulManager _sm;


    [Header("Points")]
    [SerializeField] private Vector3[] _lostSoulHoverPositions; 
    [SerializeField] private Vector3[] _lostSoulChasePositions; 
    [SerializeField]private int hoverPosCounter;
    [SerializeField]private int chasePosCounter;


    [Header("Misc")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] bool _isMoving;
    [SerializeField] bool _canPickUp;
    [SerializeField] bool _stopInividualBehaviour;

    void Start()
    {
        _detectPlayerCol.enabled = true;
        _pickUpCol.enabled = false;
        _canPickUp = false;
        hoverPosCounter = 0;
        chasePosCounter = 0;
        _sm = GetComponent<SoulManager>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            if(chasePosCounter < _lostSoulChasePositions.Length)
            {
                if(!_isMoving)
                {
                    _isMoving = true;
                    StartCoroutine(MoveToNextChasePoint());
                    _detectPlayerCol.enabled = false;
                }
            }
            else if(_canPickUp)
            {
                //CODE HERE FOR WHAT HAPPENS WHEN PLAYER PICKS UP SOUL
                if (!_sm.isFollowing) 
                {
                    GameEvents.onSoulCollect.Invoke(_sm);
                }
                // if(other.TryGetComponent<PlayerSoulHandler>(out PlayerSoulHandler playerSoulHandler))
                // {
                //     playerSoulHandler.AddMeToSouls(this);
                //     StopThisSoul();
                // }
                Debug.Log("Picked up this soul: " + gameObject.name);
            }
            else
            {
                _detectPlayerCol.enabled = false;
                _pickUpCol.enabled = true;
                _canPickUp = true;
            }
        }
    }

    IEnumerator MoveToNextChasePoint()
    {
        Vector3 NextPos = _lostSoulChasePositions[chasePosCounter];
        
        //Moves the lost soul to the next saved position 
        while (transform.position != NextPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, NextPos, _moveSpeed*Time.deltaTime);
            yield return null;
        }

        //Checks if the lost soul new position is a hovering position and if it is waits for the player to be in range again
        if(_lostSoulHoverPositions[hoverPosCounter] == _lostSoulChasePositions[chasePosCounter])
        {
            hoverPosCounter++;
            chasePosCounter++;
            LostSoulIsHovering();
        }
        else
        {
            chasePosCounter++;
            StartCoroutine(MoveToNextChasePoint());
        }
    }

    private void LostSoulIsHovering()
    {
        _detectPlayerCol.enabled = true;
        _isMoving = false;
    }

    private void StopThisSoul()
    {
        _stopInividualBehaviour = true;
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(LostSoulBehaviour)), CanEditMultipleObjects]
    public class LostSoulEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LostSoulBehaviour lostSoulBehaviour = (LostSoulBehaviour)target;
            if(GUILayout.Button("Add Chase Point", GUILayout.Height(20)))
            {
                lostSoulBehaviour._lostSoulChasePositions = AddAPoint(lostSoulBehaviour, lostSoulBehaviour._lostSoulChasePositions);
            }

            if(GUILayout.Button("Add Hover Point", GUILayout.Height(20)))
            {
                lostSoulBehaviour._lostSoulChasePositions = AddAPoint(lostSoulBehaviour, lostSoulBehaviour._lostSoulChasePositions);
                lostSoulBehaviour._lostSoulHoverPositions = AddAPoint(lostSoulBehaviour, lostSoulBehaviour._lostSoulHoverPositions);
            }
            
            base.OnInspectorGUI();
        }

        private Vector3[] AddAPoint(LostSoulBehaviour lostSoulBehaviour, Vector3[] targetArray)
        {
            lostSoulBehaviour.transform.position = new Vector3(lostSoulBehaviour.transform.position.x, lostSoulBehaviour.transform.position.y, 0);  
            Vector3[] tempPositions = new Vector3[targetArray.Length+1];

            targetArray.CopyTo(tempPositions, 0);
            targetArray = tempPositions;
            targetArray[targetArray.Length-1] = lostSoulBehaviour.transform.position;
            return targetArray;
        }
    }
    #endif
}




