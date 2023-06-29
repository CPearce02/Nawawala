using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraEffectsSystem : MonoBehaviour
{
    public static CameraEffectsSystem Instance {get; private set;}
    private CinemachineVirtualCamera _cinemachineVirtualCamera;
    
    [Header("Components")]
    public Camera mainCam;
    public CinemachineBrain cinemachineBrain;


    [Header("Normal Variables")]
    //private bool _effectBeingPlayed;
    const float CAMERAOGORTHO = 15;
    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
    private CinemachineTransposer _cinemachineTransposer;


    [Header("ZoomInAndOut")]
    private bool _isZoomingOutAndIn;
    private bool _hasZoomedOut;
    private bool _staleZoom;
    private float _staleZoomTimer;
    private float _zoomOutDistance;
    private float _durationOfZoomingIn;
    private float _durationOfZoomingOut;


    [Header("ZoomInOnAngles")]
    private bool _zoomInOnAngles;
    private bool _zoomInOnAnglesIsOn;
    private bool _zoomInOnAnglesMovingIn;
    private int _zoomInOnAnglesCounter;

    private float _maxZoomInOnAnglesTime;
    private float _zoomInOnAnglesTimer;
    private float _zoomInDistance;
    private float _zoomIntoAngleDuration;
    private float _angleIntensity;

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
        _cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineTransposer = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        //_effectBeingPlayed = false;
        _zoomInOnAnglesCounter = 0;
    }

    public void ShakeCamera(float intensity, float shakeDuration)
    {
        StartCoroutine(StartNormalShakeEffect(intensity, shakeDuration));
    }

    // public void ShakeChangeCamera(float InitialIntensity, float ModifyIntensityBy ,float shakeDuration)
    // {
    //     StartCoroutine(StartChangingShakeEffect(InitialIntensity, ModifyIntensityBy, shakeDuration));
    // }

    IEnumerator StartNormalShakeEffect(float intensity, float shakeDuration)
    {
        float shakeTimer = shakeDuration;
        float shakeTimerTotal = shakeDuration;

        _cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain += intensity;
       
        while (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            if(shakeTimer <= 0f)
            {
                _cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain -= intensity;
                Mathf.Lerp(intensity, 0f, (1-(shakeTimer / shakeTimerTotal)));
            }
            yield return null;
        }
    }

    // IEnumerator StartChangingShakeEffect(float intensity, float otherIntensity, float shakeDuration)
    // {
    //     float shakeTimer = shakeDuration;
    //     float shakeTimerTotal = shakeDuration;

    //     _cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    //     _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain += intensity;
       
    //     float addedIntensity = 0;
    //     float startTime = Time.time;
    //     float lastLerpValue = 0;

    //     while (shakeTimer > 0)
    //     {
    //         shakeTimer -= Time.deltaTime;
            
    //         float timeElapsed = Time.time - startTime;
    //         float t = Mathf.Clamp01(timeElapsed / shakeDuration);
    //         float lerpedValue = Mathf.Lerp(addedIntensity, otherIntensity, t);

    //         //LastLerpValue needed otherwise the AmplitudeGain increase by each time the lerp value is calculated
    //         _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain += lerpedValue - lastLerpValue;

    //         lastLerpValue = lerpedValue;

    //         if(shakeTimer <= 0f)
    //         {
    //             _cinemachineBasicMultiChannelPerlin = _cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

    //             _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain -= intensity + otherIntensity;
    //             Mathf.Lerp(intensity + otherIntensity, 0f, (1-(shakeTimer / shakeTimerTotal)));
    //         }
    //         yield return null;
    //     }
    // }

    // public void ZoomOutAndIn(float zoomOutDistance, float zoomOutDuration, float zoomInDuration, bool stayInAir, float staleZoomDuration)
    // {
    //     _zoomOutDistance = CAMERAOGORTHO + zoomOutDistance;
    //     _durationOfZoomingOut = zoomOutDuration;
    //     _durationOfZoomingIn = zoomInDuration;

    //     if(stayInAir)
    //     {
    //         _staleZoom = stayInAir;
    //         _staleZoomTimer = staleZoomDuration;
    //     }

    //     _hasZoomedOut = false;
    //     _isZoomingOutAndIn = true;
    //     //_effectBeingPlayed = true;
    // }

    public void ChangeZoom(float targetZoom)
    {
        
    }

    // #region ZOOM IN ON ANGLES METHODS
    // public void ZoomInOnAngles(float _maxLeftAngle, float _maxRightAngle, float zoomInDistance, float moveInSpeed, float timeTilBackToNormal)
    // {
    //     if(_zoomInOnAnglesCounter <= 2)
    //     {
    //         _zoomInDistance = zoomInDistance;
    //         _zoomIntoAngleDuration = moveInSpeed;

    //         //Works out the angles the Effects needs to do
    //         if(_zoomInOnAnglesCounter <= 0)
    //         {
    //             _angleIntensity = Random.Range(_maxLeftAngle, _maxRightAngle+1);
    //             _zoomInOnAnglesMovingIn = true;
    //             _zoomInOnAnglesIsOn = true;
    //             _maxZoomInOnAnglesTime = timeTilBackToNormal;
    //         }
    //         else if(_angleIntensity >= 0)
    //         {
    //             _angleIntensity += _maxLeftAngle;
    //         }
    //         else if(_angleIntensity < 0)
    //         {
    //             _angleIntensity += _maxRightAngle;
    //         }

    //         _zoomInOnAnglesTimer = 0;
    //         _zoomInOnAngles = true;
    //         _zoomInOnAnglesCounter++;
    //     }
    //     else
    //     {
    //         _zoomInOnAnglesCounter = 0;
    //         _zoomInOnAnglesIsOn = false;
    //     }
    // }

    // public void CancelZoomInOnAngles()
    // {
    //     _zoomInOnAnglesIsOn = false;
    //     _zoomInOnAnglesCounter = 0;
    // }
    // #endregion
    
    
    private void Update() 
    {
        if(_isZoomingOutAndIn)
        {
            if(!_hasZoomedOut)
            {
                if(_zoomOutDistance - _cinemachineVirtualCamera.m_Lens.OrthographicSize > 0.1f)
                {
                    _cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_cinemachineVirtualCamera.m_Lens.OrthographicSize, _zoomOutDistance, _durationOfZoomingOut*Time.deltaTime);
                }
                else
                {
                    _cinemachineVirtualCamera.m_Lens.OrthographicSize = _zoomOutDistance;
                    _hasZoomedOut = true;
                }
            }
            else if(_hasZoomedOut)
            {
                if(!_staleZoom)
                {
                    if(CAMERAOGORTHO - _cinemachineVirtualCamera.m_Lens.OrthographicSize < 0.1f)
                    {
                        _cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_cinemachineVirtualCamera.m_Lens.OrthographicSize, CAMERAOGORTHO, _durationOfZoomingIn*Time.deltaTime);
                    }
                    else
                    {
                        _cinemachineVirtualCamera.m_Lens.OrthographicSize = CAMERAOGORTHO;
                        _isZoomingOutAndIn = false;
                        //_effectBeingPlayed = false;
                    }
                }
                else if(_staleZoomTimer >= 0)
                {
                    _staleZoomTimer -= Time.deltaTime;
                }
                else if(_staleZoomTimer < 0)
                {
                    _staleZoom = false;
                }
            }
        }

        if(_zoomInOnAngles)
        {
            if(_zoomInOnAnglesIsOn)
            {
                //Handles zooming in
                if(_zoomInOnAnglesMovingIn)
                {
                    if(_cinemachineVirtualCamera.m_Lens.OrthographicSize - _zoomInDistance > 0.1f)
                    {
                        _cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_cinemachineVirtualCamera.m_Lens.OrthographicSize, _zoomInDistance, _zoomIntoAngleDuration*Time.deltaTime);
                    }
                    else
                    {
                        _zoomInOnAnglesMovingIn = false;
                    }
                }

                //Handles Rotation
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, _angleIntensity), 100f);

                //Checks to see 
                if(_zoomInOnAnglesTimer > _maxZoomInOnAnglesTime)
                {
                    //Debug.Log("Timer End");
                    _zoomInOnAnglesIsOn = false;
                    _zoomInOnAnglesCounter = 0;
                }
                _zoomInOnAnglesTimer += Time.deltaTime;
            }
            else
            {
                if(CAMERAOGORTHO - _cinemachineVirtualCamera.m_Lens.OrthographicSize > 0.05f)
                {
                    //Debug.Log("Ending");
                    _cinemachineVirtualCamera.m_Lens.OrthographicSize = Mathf.MoveTowards(_cinemachineVirtualCamera.m_Lens.OrthographicSize, CAMERAOGORTHO, _zoomIntoAngleDuration*Time.deltaTime);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), 50f);
                }
                else
                {
                    _zoomInOnAngles = false;
                }
            }
        }
    }

    public void ChangeCameraDampingState(bool state)
    {
        float dampingValue = 0;
        
        if(state)
        {
            dampingValue = 0.3f;
            
        }
        _cinemachineTransposer.m_XDamping = dampingValue;
        _cinemachineTransposer.m_YDamping = dampingValue;
    }

    public void ChangeCameraUpdateType(CinemachineBrain.UpdateMethod updateMethod)
    {
        cinemachineBrain.m_UpdateMethod = updateMethod;
    }
}
