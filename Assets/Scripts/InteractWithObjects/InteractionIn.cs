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
    [Header("Lock")]
    [SerializeField] public bool isLock;

    private bool _isPlayerCloseToInteract = false;

    private const string CHEST_OPEN = "ChestOpen";
    private const string LOCK_OPEN = "LockOpen";

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
}

