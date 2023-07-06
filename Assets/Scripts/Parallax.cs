using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    [Header("ParallaxEffect")]
    private float _spriteLength, _startPos, _yPos;
    [SerializeField] private GameObject _cam;
    [SerializeField] private float _parallaxEffect;


    // Start is called before the first frame update
    void Start()
    {
        _startPos = transform.position.x;
        _yPos = transform.position.y;
        _spriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float _temp = (_cam.transform.position.x * (1 - _parallaxEffect));
        float _dist = (_cam.transform.position.x * _parallaxEffect);
        float _ydist = (_cam.transform.position.y * _parallaxEffect);

        transform.position = new Vector3(_startPos + _dist, _yPos + _ydist, transform.position.z);
        if (_temp > _startPos + _spriteLength)
        {
           _startPos += _spriteLength;
        }
        else if (_temp < _startPos - _spriteLength)
        {
           _startPos  -= _spriteLength;
        }
    }
}
