using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindTunnelBehaviour : MonoBehaviour
{
    [SerializeField] private Transform[] positions;  
    [SerializeField] private float _pullSpeed;      
    private bool isPulling = false;    
    private int currentIndex = 0;      

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPulling = true;
            currentIndex = 0;  
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPulling = false;
        }
    }

    private void FixedUpdate()
    {
        if (isPulling)
        {
            Transform currentEndPos = positions[currentIndex];

            Vector2 direction = (currentEndPos.position - transform.position).normalized;

            Rigidbody2D playerRigidbody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
            playerRigidbody.velocity = direction * _pullSpeed;

            float distance = Vector2.Distance(transform.position, currentEndPos.position);
            if (distance < 0.1f)
            {
                currentIndex++;
                if (currentIndex >= positions.Length)
                {
                    currentIndex = 0;  
                }
            }
        }
    }
}
