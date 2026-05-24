using System.Collections;
using UnityEngine;

public class InteractionInOut : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float transparencyAmount = 0.1f;
    [SerializeField] private float fadeTime = 0.05f;
    [SerializeField] private GameObject outlineObject;
    [Header("Locker Settings")]
    [SerializeField] private bool isLocker;
    [SerializeField] private float _moveToLockerSpeed = 5f;
    private float _initialTransparencyAmount;

    private bool _isPlayerCloseToInteract = false;
    private bool _isInteractionActive = false;
    private bool _isFading = false;
    private Vector3 _playerBasePosition;

    private SpriteRenderer _spriteRenderer;
    //==============================================================
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (outlineObject != null)
            outlineObject.SetActive(false);
    }
    private void Start()
    {
        _initialTransparencyAmount = _spriteRenderer.color.a;
        GameInput.Instance.OnPlayerInteractionE += GameInput_OnPlayerInteractionE;
    }
    private void OnDestroy()
    {
        GameInput.Instance.OnPlayerInteractionE -= GameInput_OnPlayerInteractionE;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            _isPlayerCloseToInteract = true;
            Player.Instance.isCloseToInteract = true;
            if (!_isInteractionActive && outlineObject != null)
                outlineObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            _isPlayerCloseToInteract = false;
            Player.Instance.isCloseToInteract = false;
            if (outlineObject != null)
                outlineObject.SetActive(false);
        }
    }
    //==============================================================
    private void GameInput_OnPlayerInteractionE(object sender, System.EventArgs e)
    {
        if (_isPlayerCloseToInteract && !_isInteractionActive)
        {
            StartInteraction();
        }
        else if (_isInteractionActive)
        {
            EndInteraction();
        }
    }
    private void StartInteraction()
    {
        Debug.Log("Interaction InOut started");
        _isInteractionActive = true;
        Player.Instance.isInteractingInOut = true;
        Player.Instance.allowedToMove = false;
        HideInLocker();
        if (outlineObject != null)
            outlineObject.SetActive(false);

        if (!_isFading)
            StartCoroutine(FadeToTarget(transparencyAmount));
    }
    private void EndInteraction()
    {
        Debug.Log("Interaction InOut ended");
        _isInteractionActive = false;
        Player.Instance.isInteractingInOut = false;
        GetOutOfLocker();
        if (_isPlayerCloseToInteract && outlineObject != null)
            outlineObject.SetActive(true);

        if (!_isFading)
            StartCoroutine(FadeToTarget(_initialTransparencyAmount));
    }
    private void HideInLocker()
    {
        if (isLocker)
        {
            _playerBasePosition = Player.Instance.transform.position;
            StartCoroutine(MovePlayerToObject(Player.Instance.transform.position, transform.position));
        }
    }
    private void GetOutOfLocker()
    {
        if (isLocker)
        {
            StartCoroutine(MovePlayerToObject(Player.Instance.transform.position, _playerBasePosition));
        }
    }
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
    private IEnumerator MovePlayerToObject(Vector3 startPosition, Vector3 targetPosition)
    {
        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * _moveToLockerSpeed;
            Player.Instance.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsed);
            yield return null;
        }

        Player.Instance.transform.position = targetPosition;
    }

}