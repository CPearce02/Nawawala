using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WindTunnelBehaviour : SingableObject
{
    private enum WindDirection
    {
        Left, Right, Up, Down
    }
    [SerializeField] private Collider2D _col2D;

    [Header("Initialisation")]
    [SerializeField] private bool _isMyPlantSingable = true;
    [SerializeField] private WindDirection _windDirection;
    [SerializeField] private PitchLevel[] _pitchTarget;
    [SerializeField] private PitchReceiver _pitchReceiver;


    [Header("Other")]
    [SerializeField] private GameObject _windSprite;
    //[SerializeField] private float _pushSpeed;  
    [SerializeField] private float _pushSpeedLimit;
    [SerializeField] private float _windDuration;    
    private bool isPushing;
    private float _xPushLimit;
    private float _yPushLimit;

    Coroutine PushingPlayerCo;    
    private Rigidbody2D _playerRb;
    private Vector2 _pushDir;
    private bool _isActive;

    private void Awake() 
    {
        if(_isMyPlantSingable)
        {
            SetUpPitchReciever();
        }
    }

    private void Start() 
    {
        switch (_windDirection)
        {
            case WindDirection.Left:
                _pushDir = new Vector2(-1, 0);
                _xPushLimit = -_pushSpeedLimit;
                break;
            case WindDirection.Right:
                _pushDir = new Vector2(1, 0);
                _xPushLimit = _pushSpeedLimit;
                break;
            case WindDirection.Up:
                _pushDir = new Vector2(0, 1);
                _yPushLimit = _pushSpeedLimit;
                break;
            case WindDirection.Down:
                _pushDir = new Vector2(0, -1);
                _yPushLimit = -_pushSpeedLimit;
                break;
        }

        _isActive = false;
        _col2D.enabled = false;
        _windSprite.SetActive(false);
    }

    public override void SetUpPitchReciever()
    {
        if(_pitchReceiver == null)
        {
            _pitchReceiver.GetComponentInChildren<PitchReceiver>();
        }
        _pitchReceiver.Init(PlayPitchBehaviour, _pitchTarget);
    }

    public override void PlayPitchBehaviour()
    {
        if(!_isActive)
        {
            StartWind();
        }
    }

    private void StartWind()
    {
        _isActive = true;
        _col2D.enabled = true;
        _windSprite.SetActive(true);
        StartCoroutine(TurnOffWindTunnel());
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerMovement) && !isPushing)
        {
            isPushing = true;
            _justEntered = true;
            //playerMovement.enabled = false;
            _playerRb = playerMovement.GetComponent<Rigidbody2D>();
            
            PushingPlayerCo = StartCoroutine(PushPlayer());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerMovement))
        {
            isPushing = false;
            if(PushingPlayerCo != null)
            {
                StopCoroutine(PushingPlayerCo);
            }
        }
    }

    private bool _justEntered;

    private IEnumerator PushPlayer()
    {
        //float justEnteredTimer = 0.2f;
        _playerRb.velocity = new Vector2(0,0);

        // while (isPushing && _justEntered)
        // {
        //     if(justEnteredTimer < 0)
        //     {
        //         _justEntered = false;
        //     }
        //     else
        //     {
        //         justEnteredTimer -= Time.deltaTime;
        //     }

        //     // if(_xPushLimit != 0)
        //     // {
        //     //     _playerRb.transform.position = new Vector3(_playerRb.transform.position.x, transform.position.y, 0);
        //     // }
        //     // else
        //     // {
        //     //     _playerRb.transform.position = new Vector3(transform.position.x, _playerRb.transform.position.y, 0);
        //     // }
        //     yield return null;
        // }
        while (isPushing)
        { 
            // if(_justEntered)
            // {
            //     if(justEnteredTimer < 0)
            //     {
            //         _justEntered = false;
            //     }
            //     else
            //     {
            //         justEnteredTimer -= Time.deltaTime;
            //     }
            //     _playerRb.AddForce(_pushDir*_pushSpeed/2);
            // }
            // else
            {
                //_playerRb.AddForce(_pushDir*_pushSpeed);
            }

            if(_windDirection == WindDirection.Up)
            {
                if(_playerRb.velocity.y > _yPushLimit)
                {
                    _playerRb.velocity = new Vector2(_playerRb.velocity.x, _yPushLimit);
                }
                _playerRb.velocity = new Vector2(_playerRb.velocity.x, _yPushLimit);
            }
            else if(_windDirection == WindDirection.Right)
            {
                if(_playerRb.velocity.x > _xPushLimit)
                {
                }
                    _playerRb.velocity = new Vector2(_xPushLimit, 0);
            }
            else if(_windDirection == WindDirection.Left)
            {
                if(_playerRb.velocity.x < _xPushLimit)
                {
                }
                    _playerRb.velocity = new Vector2(_xPushLimit, 0);
            }
            else if(_windDirection == WindDirection.Down)
            {
                if(_playerRb.velocity.y < _yPushLimit)
                {
                }
                    _playerRb.velocity = new Vector2(_playerRb.velocity.x, _yPushLimit);
            }
            else
            {
                Debug.Log("Push Limit wasn't set" + gameObject.name);
            }
            
            yield return null;
        }
    }

    IEnumerator TurnOffWindTunnel()
    {
        yield return new WaitForSeconds(_windDuration);
        _isActive = false;
        _col2D.enabled = false;
        _windSprite.SetActive(false);
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(WindTunnelBehaviour)), CanEditMultipleObjects]
    public class WindTunnelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            WindTunnelBehaviour windTunnelBehaviour = (WindTunnelBehaviour)target;
            if(GUILayout.Button("Facing Up", GUILayout.Height(20)))
            {
                FaceUp(windTunnelBehaviour.transform);
                windTunnelBehaviour._windDirection = WindDirection.Up;
            }
            if(GUILayout.Button("Facing Down", GUILayout.Height(20)))
            {
                FaceDown(windTunnelBehaviour.transform);
                windTunnelBehaviour._windDirection = WindDirection.Down;
            }
            if(GUILayout.Button("Facing Left", GUILayout.Height(20)))
            {
                FaceLeft(windTunnelBehaviour.transform);
                windTunnelBehaviour._windDirection = WindDirection.Left;
            }
            if(GUILayout.Button("Facing Right", GUILayout.Height(20)))
            {
                FaceRight(windTunnelBehaviour.transform);
                windTunnelBehaviour._windDirection = WindDirection.Right;
            }
        }

        private void FaceUp(Transform targetTran)
        {
            targetTran.eulerAngles = new Vector3(0,0,0);

        }

        private void FaceDown(Transform targetTran)
        {
            targetTran.eulerAngles = new Vector3(0, 0, 180);
        }

        private void FaceLeft(Transform targetTran)
        {
            targetTran.eulerAngles = new Vector3(0,0, 90);
        }

        private void FaceRight(Transform targetTran)
        {
            targetTran.eulerAngles = new Vector3(0, 0, -90);
        }
    }

    #endif
}
