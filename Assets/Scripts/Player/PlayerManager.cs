using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]private Transform _followObject;
    private float _currentOffSet = 0.75f;
    public List<SoulManager> collectedSouls = new List<SoulManager>();

    private void OnEnable()
    {
        GameEvents.onSoulCollect += AddSoul;
    }
    private void OnDisable()
    {
        GameEvents.onSoulCollect -= AddSoul;
    }

    // Start is called before the first frame update
    void Start()
    {
        _followObject = GameObject.Find("FollowObj").transform;
    }

    // Update is called once per frame
    void Update()
    {
            
    }

    private void AddSoul(SoulManager sm) 
    {
        sm.SetFollowAndOffset(_followObject.transform, _currentOffSet);
        collectedSouls.Add(sm);
        _followObject = collectedSouls[collectedSouls.Count - 1].transform;
    }

    public void DisperseSouls() 
    {
        StartCoroutine(DisperseSoulsCoroutine());
    }

    private IEnumerator DisperseSoulsCoroutine()
    {
        if (collectedSouls.Count <= 0) yield return null;
        foreach (SoulManager sm in collectedSouls)
        {
            sm.isFollowing = false;
            float randomAngle = Random.Range(0f, 180f);
            Vector2 awayDirection = Quaternion.Euler(0f, 0f, randomAngle) * Vector2.up;
            Rigidbody2D rb = sm.GetComponent<Rigidbody2D>();
            rb.AddForce(awayDirection * 15f, ForceMode2D.Impulse);

            yield return new WaitForSeconds(0.5f);

            rb.velocity = Vector2.zero;
        }

        _followObject = GameObject.Find("FollowObj").transform;
        collectedSouls.Clear();

    }

    private void OnReset()
    {
        SceneManager.LoadScene(0);
    }
}
