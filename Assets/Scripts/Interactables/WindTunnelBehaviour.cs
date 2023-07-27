using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float _pushSpeed;  
    [SerializeField] private float _windDuration;    
    private bool isPushing;

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
                break;
            case WindDirection.Right:
                _pushDir = new Vector2(1, 0);
                break;
            case WindDirection.Up:
                _pushDir = new Vector2(0, 1);
                break;
            case WindDirection.Down:
                _pushDir = new Vector2(0, -1);
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
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement) && !isPushing)
        {
            isPushing = true;
            //playerMovement.enabled = false;
            _playerRb = playerMovement.GetComponent<Rigidbody2D>();
            
            PushingPlayerCo = StartCoroutine(PushPlayer());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement))
        {
            isPushing = false;
            if(PushingPlayerCo != null)
            {
                StopCoroutine(PushingPlayerCo);
            }
            //playerMovement.enabled = true;
        }
    }

    private IEnumerator PushPlayer()
    {
        while (isPushing)
        { 
            _playerRb.AddForce(_pushDir*_pushSpeed, ForceMode2D.Force);
            if(_playerRb.velocity.y > 20)
            {
                _playerRb.velocity = new Vector2(_playerRb.velocity.x, 20);
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
}
