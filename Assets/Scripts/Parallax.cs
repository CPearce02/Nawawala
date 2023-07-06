using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    [Header("ParallaxEffect")]
    private float _spriteSize, _startXPos, _startYPos;
    [SerializeField] private Transform _camTransform;
    [SerializeField] private float _parallaxSpeed;


    // Start is called before the first frame update
    void Start()
    {
        _camTransform = Camera.main.transform;
        _startXPos = transform.position.x;
        _startYPos = transform.position.y;
        _spriteSize = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float _relativeDist = _camTransform.position.x * _parallaxSpeed;
        float _relativeDistY = _camTransform.position.y * _parallaxSpeed;
        transform.position = new Vector3(_startXPos + _relativeDist, _startYPos + _relativeDistY, transform.position.z);

        float _relativeCameraDist = _camTransform.position.x * (1 - _parallaxSpeed);
        if(_relativeCameraDist > _startXPos + _spriteSize) 
        {
            _startXPos += _spriteSize;
        }
        else if(_relativeCameraDist < _startXPos - _spriteSize) 
        {
            _startXPos -= _spriteSize;
        }
    }
}
