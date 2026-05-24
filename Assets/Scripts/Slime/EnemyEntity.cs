using System;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAI))]

public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;
    private int _currentHealth;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private EnemyAI _enemyAI;

    private bool _canDamagePlayer = true;
    private float _damageCooldown = 0.5f;
    private float _lastDamageTime = 0f;
    //=======================================================================
    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }
    private void Start()
    {
        _currentHealth = enemySO.enemyHealth;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out Player player))
        {
            if (Time.time >= _lastDamageTime + _damageCooldown)
            {
                _lastDamageTime = Time.time;

                if (enemySO.isInstantKill)
                {
                    // Блоб убивает мгновенно
                    player.TakeDamage(transform, 9999);
                }
                else
                {
                    // Обычный урон
                    player.TakeDamage(transform, enemySO.enemyDamageAmount);
                }
            }
        }
    }
    //=======================================================================
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        OnTakeHit?.Invoke(this, EventArgs.Empty);
        DetectDeath();
    }
    public void PolygonColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }
    public void PolygonColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }
    //=================================================================================================================
    private void DetectDeath()
    {
        if (_currentHealth <= 0)
        {
            OnDeath?.Invoke(this, EventArgs.Empty);
            _boxCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;
            _enemyAI.SetDeathState();
        }
    }
}
