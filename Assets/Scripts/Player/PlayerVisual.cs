using System;
using System.Collections;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;
    [SerializeField] protected GameObject HidingDarkness;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Transform _transform;
    private FlashBlink _flashBlink;
    private Vector3 _positionBeforeInteraction;
    public AudioClip footsteps;
    public AudioClip whoosh;
    public AudioClip lockerOpen;
    public AudioClip lockerClose;
    private bool _hasPlayedHidingSound;
    private bool _hasPlayedAttackSound;

    private const string IS_RUNNING = "IsRunning";
    private const string IS_SHIFT_RUNNING = "IsShiftRunning";
    private const string IS_ATTACKING = "IsAttacking";
    private const string IS_DEAD = "IsDead";
    private const string IS_HIDING = "IsHiding";
    //=================================================================================================================
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flashBlink = GetComponent<FlashBlink>();
        HidingDarkness.SetActive(false);
    }
    private void Start()
    {
        Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
        Player.Instance.OnPlayerRespawn += Player_OnPlayerRespawn;
        StartCoroutine(PlaySteps());
        StartCoroutine(PlayRunning());
        StartCoroutine(PlayAttacking());
    }
    private void Update()
    {
        _animator.SetBool(IS_RUNNING, Player.Instance.IsRunning());
        _animator.SetBool(IS_SHIFT_RUNNING, Player.Instance.IsShiftRunning());
        _animator.SetBool(IS_ATTACKING, Player.Instance.IsAttacking());
        if (Player.Instance.IsAlive())
        {
            AdjustPlayerFacingDirection();
        }

        if (!Player.Instance.IsVisibleForEnemy())
        {
            _animator.SetBool(IS_HIDING, true);
            HidingDarkness.SetActive(true);
            if (!_hasPlayedHidingSound)
            {
                _hasPlayedHidingSound = true;
                AudioManager.Instance.PlaySFX(lockerOpen);
            }
        }
        else
        {
            _animator.SetBool(IS_HIDING, false);
            HidingDarkness.SetActive(false);
            if (_hasPlayedHidingSound)
            {
                _hasPlayedHidingSound = false;
                AudioManager.Instance.PlaySFX(lockerClose);
            }
        }

        if (Player.Instance.IsAttacking() && !_hasPlayedAttackSound)
        {
            _hasPlayedAttackSound = true;
            AudioManager.Instance.PlaySFX(whoosh);
        }
        else if (!Player.Instance.IsAttacking())
        {
            _hasPlayedAttackSound = false;
        }
    }
    private void OnDestroy()
    {
        Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
        Player.Instance.OnPlayerRespawn -= Player_OnPlayerRespawn;
    }
    //=================================================================================================================
    private void AdjustPlayerFacingDirection()
    {
        int directionRunning = Player.Instance.DirectionRunning();
        if (directionRunning < 0) { _spriteRenderer.flipX = true; }
        else if (directionRunning > 0) { _spriteRenderer.flipX = false; }
        else
        {
            Vector3 mousePosition = GameInput.Instance.GetMousePosition();
            Vector3 playerPosition = Player.Instance.GetPlayerPosition();
            if (mousePosition.x < playerPosition.x) { _spriteRenderer.flipX = true; }
            else { _spriteRenderer.flipX = false; }
        }
    }
    private void Player_OnPlayerDeath(object sender, EventArgs e)
    {
        _animator.SetTrigger(IS_DEAD);
        _flashBlink.StopBlinking();
        //HidingDarkness.SetActive(true);
    }
    private void Player_OnPlayerRespawn(object sender, EventArgs e)
    {
        _flashBlink.StartBlinking();
        //HidingDarkness.SetActive(false);
    }
    private IEnumerator PlaySteps()
    {
        while (true)
        {
            if (Player.Instance.IsRunning() && !Player.Instance.IsShiftRunning())
            {
                AudioManager.Instance.PlaySFX(footsteps);
            }
            yield return new WaitForSeconds(0.3f);  //Можно настроить
        }
    }
    private IEnumerator PlayRunning()
    {
        while (true)
        {
            if (Player.Instance.IsShiftRunning())
            {
                AudioManager.Instance.PlaySFX(footsteps);
            }
            yield return new WaitForSeconds(0.25f);  //Можно настроить
        }
    }
    private IEnumerator PlayAttacking()
    {
        if (Player.Instance.IsAttacking())
        {
            AudioManager.Instance.PlaySFX(whoosh);
        }
        yield return new WaitForSeconds(0.5f);  //Можно настроить
    }
}
