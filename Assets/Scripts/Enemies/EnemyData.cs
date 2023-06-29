
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy")]
public class EnemyData : ScriptableObject
{
    [Header("General")]
    public string enemyName;
    public Sprite enemySprite;
    [Header("Movement")]
    public float moveSpeed;
    public float retreatTime;
    public float triggerRange;
    [Header("Attacking")]
    public float attackRange;
    public int damageAmount;
    public float attackCooldown;
    [Header("Health")]
    public int healthAmount;
}
