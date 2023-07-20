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
        StartCoroutine(TurnOffWindTunnel());
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.TryGetComponent<PlayerMovement>(out PlayerMovement playerMovement) && !isPushing)
        {
            isPushing = true;
            playerMovement.enabled = false;
            _playerRb = playerMovement.GetComponent<Rigidbody2D>();
            PushPlayer();
            // PushingPlayerCo =
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
            playerMovement.enabled = true;
        }
    }


    private void PushPlayer()
    {
        _playerRb.AddForce(_pushDir*_pushSpeed, ForceMode2D.Impulse);
    }


    IEnumerator TurnOffWindTunnel()
    {
        yield return new WaitForSeconds(_windDuration);
        _isActive = false;
        _col2D.enabled = false;
    }

    // private void FixedUpdate()
    // {
    //     if (isPulling)
    //     {
    //         Transform currentEndPos = positions[currentIndex];

    //         Vector2 direction = (currentEndPos.position - transform.position).normalized;

    //         Rigidbody2D playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    //         playerRigidbody.velocity = direction * _pullSpeed;

    //         float distance = Vector2.Distance(transform.position, currentEndPos.position);
    //         if (distance < 0.1f)
    //         {
    //             currentIndex++;
    //             if (currentIndex >= positions.Length)
    //             {
    //                 currentIndex = 0;  
    //             }
    //         }
    //     }
    // }
}
