using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTotemBehaviour : MonoBehaviour
{
    private bool _hasASoul;

    void Start()
    {
        _hasASoul = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!_hasASoul)
        {
            if(other.TryGetComponent<PlayerSoulHandler>(out PlayerSoulHandler playerSoulHandler))
            {
                if(playerSoulHandler._hasLostSouls)
                {
                    _hasASoul = true;
                    StartCoroutine(MoveSoulTowardsMe(playerSoulHandler.TouchedEndTotem()));
                }
            }
        }
    }

    private IEnumerator MoveSoulTowardsMe(LostSoulBehaviour lostSoulBehaviour)
    {   
        Debug.Log(lostSoulBehaviour.name);
        while (Vector3.Distance(lostSoulBehaviour.transform.position, transform.position) > 0.1f)
        {
            Debug.Log("workiung2");
            float distance = Vector2.Distance(lostSoulBehaviour.transform.position, transform.position);

            float normalizedSpeed = Mathf.InverseLerp(2.5f, 90f, distance);

            float extraSpeed = Mathf.Lerp(1, 3, normalizedSpeed);

            lostSoulBehaviour.transform.position = Vector3.MoveTowards(lostSoulBehaviour.transform.position, transform.position, Time.deltaTime*10f);

            yield return null;
        }

        SoulAttached();
    }

    private void SoulAttached()
    {
        Debug.Log("found bitch");
    }
}
