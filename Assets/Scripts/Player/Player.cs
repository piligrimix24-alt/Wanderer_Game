using System;
using System.Collections;
using UnityEngine;

[SelectionBase]
public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    public static Player Instance { get; private set; }
    private Rigidbody2D _rb;
    private KnockBack _knockBack;
    private PlayerAudio _playerAudio;

    public event EventHandler OnPlayerTakeHit;
    public event EventHandler OnPlayerDeath;
    public event EventHandler OnPlayerRespawn;

    [SerializeField] private float movingSpeed;
    private readonly float _baseSpeed = 5f;
    private readonly float _shiftSpeed = 10f;
    //private readonly float _dashSpeed = 15f;
    private readonly float _minSpeed = 0.1f;
    private int _directionRunning = 1;
    [Header("Player Health")]
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private int _currentHealth;
    private float _damageRecoveryTime = 1f;
    [SerializeField] private bool canRespawn;
    private Vector3 respawnPosition;
    private int _keysOnDeath;
    [Header("Player States")]
    public bool isEPressed = false;
    public bool isInteractingInOut = false;
    public bool isCloseToInteract = false;
    public bool allowedToMove = true;
    public bool isNearBreakable = false;
    private bool _visibleForEnemy = true;
    private bool _isRunning = false;
    private bool _isShiftRunning = false;
    private bool _isAttacking = false;
    private bool _canTakeDamage = true;
    private bool _isAlive = true;

    [SerializeField] private int _keysCount = 0;
    private float _attackAnimationDuration = 0.1f;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip keyPickupSound;
    [SerializeField] private AudioClip keyUseSound;
    [SerializeField] private AudioClip checkpointSound;

    private float _lastHealTime = -10f;
    private float _lastKeyPickupTime = -10f;
    private float _healCooldown = 1.5f;
    private float _keyPickupCooldown = 1.5f;
    private float _lastDeathTime = -10f;
    private float _deathCooldown = 2f;

    Vector2 inputVector;
    //=================================================================================================================
    private void Awake()
    {
        Instance = this;
        _rb = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();
    }
    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
        GameInput.Instance.OnPlayerShift += GameInput_OnPlayerShift;
        GameInput.Instance.OnPlayerShiftReleased += GameInput_OnPlayerShiftReleased;
        GameInput.Instance.OnPlayerInteractionE += GameInput_OnPlayerInteractionE;
        GameInput.Instance.OnPlayerInteractionEStopped += GameInput_OnPlayerInteractionEStopped;
        movingSpeed = _baseSpeed;
        _currentHealth = maxHealth;
        respawnPosition = transform.position;
    }
    private void Update()
    {
        inputVector = GameInput.Instance.GetMovementVector();
        HandleStateDuringInteraction();
    }
    private void FixedUpdate()
    {
        if (allowedToMove && !_knockBack.isGettingKnockedBack)
        {
            HandleMovement();
        }
    }
    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
        GameInput.Instance.OnPlayerShift -= GameInput_OnPlayerShift;
        GameInput.Instance.OnPlayerShiftReleased -= GameInput_OnPlayerShiftReleased;
        GameInput.Instance.OnPlayerInteractionE -= GameInput_OnPlayerInteractionE;
        GameInput.Instance.OnPlayerInteractionEStopped -= GameInput_OnPlayerInteractionEStopped;
    }
    //=================================================================================================================
    public Vector3 GetPlayerPosition()
    {
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        return playerScreenPosition;
    }
    public int DirectionRunning() => _directionRunning;
    public bool isMoving() => _directionRunning != 0;
    public bool IsRunning() => _isRunning;
    public bool IsShiftRunning() => _isShiftRunning;
    public bool IsAttacking() => _isAttacking;
    public bool IsAlive() => _isAlive;
    public bool IsVisibleForEnemy() => _visibleForEnemy;
    public int GetPlayerCurrentHealth() => _currentHealth;
    public int GetPlayerMaxHealth() => maxHealth;
    public void TakeDamage(Transform damageSourse, int damage)
    {
        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            _knockBack.GetKnockedBack(damageSourse);
            StartCoroutine(DamageRecoveryRoutine());
            Debug.Log(_currentHealth);
            OnPlayerTakeHit?.Invoke(this, EventArgs.Empty);
        }
        DetectDeath();
    }
    public void RecoverHP()
    {
        if (Time.time < _lastHealTime + _healCooldown) return;
        if (_currentHealth != maxHealth)
        {
            _lastHealTime = Time.time;
            _currentHealth += 1;
            AudioManager.Instance.PlaySFX(healSound, 0.1f);
        }
    }
    public void AddKey()
    {
        if (Time.time < _lastKeyPickupTime + _keyPickupCooldown) return;
        _lastKeyPickupTime = Time.time;
        _keysCount++;
        Debug.Log($"Key collected! Total keys: {_keysCount}");
        AudioManager.Instance.PlaySFX(keyPickupSound);
    }
    public void UseKey()
    {
        if (_keysCount > 0)
        {
            _keysCount--;
            Debug.Log($"Key used! Total keys: {_keysCount}");
            AudioManager.Instance.PlaySFX(keyUseSound);
        }
    }
    public bool HasKey() => _keysCount > 0;
    public int GetPlayerKeys() => _keysCount;
    public void ResetAttack()
    {
        _isAttacking = false;
        allowedToMove = true;
    }
    public void UpdateRespawnPoint(Vector3 checkpointPosition)
    {
        respawnPosition = checkpointPosition;
        AudioManager.Instance.PlaySFX(checkpointSound, 0.5f);
    }
    public void DisablePLayerMovement()
    {
        allowedToMove = false;
    }
    //=================================================================================================================
    private void HandleStateDuringInteraction()
    {
        if (isInteractingInOut)
        {
            allowedToMove = false;
            _canTakeDamage = false;
            _visibleForEnemy = false;
        }
        if (isInteractingInOut == false)
        {
            allowedToMove = true;
            _canTakeDamage = true;
            _visibleForEnemy = true;
        }
    }
    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(_damageRecoveryTime);
        _canTakeDamage = true;
    }
    private void DetectDeath()
    {
        if (Time.time < _lastDeathTime + _deathCooldown) return;
        if (_currentHealth == 0 && _isAlive)
        {
            _lastDeathTime = Time.time;
            _isAlive = false;
            _knockBack.StopKnockBackMovement();
            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
            GameInput.Instance.DisableMovement();
            if (canRespawn)
            {
                StartCoroutine(Respawn());
                _canTakeDamage = false;
            }
        }
    }
    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.8f);
        transform.position = respawnPosition;
        OnPlayerRespawn?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(0.5f);
        _currentHealth = maxHealth / 2;
        _isAlive = true;
        GameInput.Instance.EnableMovement();
        yield return new WaitForSeconds(1.5f);
        _canTakeDamage = true;
    }
    private void HandleMovement()
    {
        _rb.MovePosition(_rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > _minSpeed || Mathf.Abs(inputVector.y) > _minSpeed)
        {
            _isRunning = true;
            if (inputVector.x > 0)
            {
                _directionRunning = 1;
            }
            else
            {
                _directionRunning = -1;
            }

        }
        else
        {
            _isRunning = false;
            _directionRunning = 0;
        }
    }
    //============================================================================
    private void GameInput_OnPlayerShift(object sender, System.EventArgs e)
    {
        if (_isRunning)
        {
            movingSpeed = _shiftSpeed;
            _isShiftRunning = true;
        }
        else
        {
            _isShiftRunning = false;
        }
    }
    private void GameInput_OnPlayerShiftReleased(object sender, System.EventArgs e)
    {
        movingSpeed = _baseSpeed;
        _isShiftRunning = false;
    }
    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        ActiveWeapon.Instance.GetActiveWeapon().Attack();
        _isAttacking = true;
        allowedToMove = false;
        Invoke(nameof(ResetAttack), _attackAnimationDuration);
    }
    private void GameInput_OnPlayerInteractionE(object sender, EventArgs e)
    {
        isEPressed = true;
    }
    private void GameInput_OnPlayerInteractionEStopped(object sender, EventArgs e)
    {
        isEPressed = false;
    }
}