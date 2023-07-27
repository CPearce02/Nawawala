using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulManager : MonoBehaviour
{
    private Transform _objToFollow;
    private float _offSet;
    Vector3 _positionToFollow;

    public bool isFollowing;

    private float _minSpeed = 13f;
    private float _maxSpeed = 20f;
    private float minDistance = 1f;
    private float maxDistance = 3f;

    Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isFollowing) return;

        _positionToFollow = _objToFollow.position + Vector3.right * _offSet;

        // Calculate the distance between the object and the player
        float distance = Vector2.Distance(transform.position, _positionToFollow);

        // Calculate the normalized speed based on the distance
        float normalizedSpeed = Mathf.InverseLerp(minDistance, maxDistance, distance);

        // Calculate the actual speed by lerping between the minimum and maximum speed
        float speed = Mathf.Lerp(_minSpeed, _maxSpeed, normalizedSpeed);

        transform.position = Vector2.MoveTowards(transform.position, _positionToFollow, speed * Time.deltaTime);
    }

    public void SetFollowAndOffset(Transform followObject, float offset) 
    {
        _objToFollow = followObject;
        _offSet = offset;
        isFollowing = true;
    }
}
