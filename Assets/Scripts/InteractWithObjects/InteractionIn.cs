using UnityEngine;

public class InteractionIn : MonoBehaviour
{
    [SerializeField] private GameObject outlineObject;
    [SerializeField] private Animator _animator;
    [Header("Chest")]
    [SerializeField] public bool isChest;
    [SerializeField] private bool chestWithKey;
    [SerializeField] private bool chestWithHeart;
    private float _initialTransparencyAmount;
    [Header("Statue")]
    [SerializeField] public bool isStatue;
    [SerializeField] private Canvas inventoryCanvas;
    [SerializeField] private AudioClip edningPre;
    [SerializeField] private AudioClip edning;
    [Header("Lock")]
    [SerializeField] public bool isLock;

    private bool _isPlayerCloseToInteract = false;

    private const string CHEST_OPEN = "ChestOpen";
    private const string LOCK_OPEN = "LockOpen";

    private float _lastInteractionTime = -10f;
    private float _interactionCooldown = 0.5f;

    //private SpriteRenderer _spriteRenderer;
    //==============================================================
    private void Awake()
    {
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        GameInput.Instance.OnPlayerInteractionE += GameInput_OnPlayerInteractionE;
        EndCutscene();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            _isPlayerCloseToInteract = true;
            Player.Instance.isCloseToInteract = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            _isPlayerCloseToInteract = false;
            Player.Instance.isCloseToInteract = false;
        }
    }
    //==============================================================
    public void DestroySelf()
    {
        Destroy(gameObject);
        NavMesgSurfaceManagment.Instance.RebakeNavMeshSurface();
    }
    public void OnPlayerTouchAnimationEnd()
    {
        GameInput.Instance.EnableMovement();
        SceneTransition.SwitchToScene("Menu");
    }
    public void PlayEndingSound()
    {
        AudioManager.Instance.PlaySFX(edning, 0.5f);
    }
    //==============================================================
    private void GameInput_OnPlayerInteractionE(object sender, System.EventArgs e)
    {
        if (_isPlayerCloseToInteract)
        {
            StartInteraction();
        }
    }
    private void StartInteraction()
    {
        if (Time.time < _lastInteractionTime + _interactionCooldown) return;
        _lastInteractionTime = Time.time;

        Debug.Log("InteractionIn performed");
        outlineObject.SetActive(false);
        if (isChest)
        {
            HandleChest();
        }
        if (isLock)
        {
            HandleLock();
        }
        if (isStatue)
        {
            StartEndingCutscene();
        }
    }
    private void StartEndingCutscene()
    {
        //GameDisplay.Instance.DisableInventoryDisplay();
        AudioManager.Instance.PlaySFX(edningPre, 0.3f);
        StartCutscene();
        GameInput.Instance.DisableMovement();
        if (_animator != null)
            _animator.SetTrigger("PlayerTouch");
    }
    private void HandleChest()
    {
        if (_animator != null)
            _animator.SetTrigger(CHEST_OPEN);
        if (chestWithKey)
        {
            Player.Instance.AddKey();
            Debug.Log("Player Got Key");
        }
        if (chestWithHeart)
        {
            Player.Instance.RecoverHP();
            Debug.Log("Player Healed");
        }
    }
    private void HandleLock()
    {
        if (Player.Instance.HasKey())
        {
            Player.Instance.UseKey();
            //DestroySelf();
            if (_animator != null)
                _animator.SetTrigger(LOCK_OPEN);
            Debug.Log("Lock opened!");
        }
        else
        {
            Debug.Log("Need a key to open this lock!");
        }
    }
    private void StartCutscene()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.enabled = false;
    }

    private void EndCutscene()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.enabled = true;
    }
}

