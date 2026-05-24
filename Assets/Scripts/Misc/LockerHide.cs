using System.Collections;
using UnityEngine;

public class InteractionOnOneClick : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float transparencyAmount = 0.1f;
    [SerializeField] private float fadeTime = 0.05f;
    private float _initialTransparencyAmount;

    private bool _isPlayerCloseToHide = false;
    private bool _isHiding = false;
    private bool _isInteracting = false;
    private bool _isFading = false;

    private SpriteRenderer _spriteRenderer;
    //==============================================================
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _initialTransparencyAmount = _spriteRenderer.color.a;
    }
    private void Update()
    {
        _isInteracting = _isPlayerCloseToHide && Player.Instance.isEPressed;
        if (_isInteracting && !_isHiding)
        {
            _isHiding = true;
            Player.Instance.isInteractingInOut = true;
            if (!_isFading)
                StartCoroutine(FadeToTarget(transparencyAmount));
        }
        else if (!_isInteracting && _isHiding)
        {
            _isHiding = false;
            Player.Instance.isInteractingInOut = false;
            if (!_isFading)
                StartCoroutine(FadeToTarget(_initialTransparencyAmount));
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            _isPlayerCloseToHide = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            _isPlayerCloseToHide = false;
        }
    }
    //private void OnTriggerStay2D(Collider2D collider)
    //{

    //    if (collider.gameObject.GetComponent<Player>() && collider is CapsuleCollider2D)
    //    {
    //        Debug.Log("Enter Obj");
    //        if (Player.Instance.isInteractingE)
    //        {
    //            StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, _spriteRenderer.color.a, transparencyAmount));
    //            Debug.Log("Interacting with Obj");
    //        }
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collider)
    //{
    //    if (collider.gameObject.GetComponent<Player>() && collider is CapsuleCollider2D)
    //    {
    //        if (_isInteractingNow && Player.Instance.isInteractingE == false)
    //        {
    //            _isInteractingNow = false;
    //            StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, transparencyAmount, _initialTransparencyAmount));
    //            Debug.Log("Interaction stopped");
    //        }
    //    }
    //}
    //==============================================================
    private IEnumerator FadeToTarget(float targetAlpha)
    {
        _isFading = true;
        float startAlpha = _spriteRenderer.color.a;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeTime);
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, newAlpha);
            yield return null;
        }

        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, targetAlpha);
        _isFading = false;
    }
}
