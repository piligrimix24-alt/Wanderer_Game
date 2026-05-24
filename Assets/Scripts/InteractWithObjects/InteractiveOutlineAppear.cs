using UnityEngine;

public class InteractiveOutlineAppear : MonoBehaviour
{
    [SerializeField] private GameObject outlineObject;
    [Header("Only for Lock")]
    [SerializeField] private GameObject outlineLockNoKey;
    [SerializeField] private InteractionIn mainObject;
    private bool _isLock;

    private void Awake()
    {
        if (outlineObject != null)
            outlineObject.SetActive(false);
        if (outlineLockNoKey != null)
            outlineLockNoKey.SetActive(false);
    }
    private void Start()
    {
        if (mainObject != null)
            _isLock = mainObject.isLock;
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            if (_isLock)
            {
                if (Player.Instance.HasKey())
                {
                    outlineLockNoKey.SetActive(false);
                    outlineObject.SetActive(true);
                }
                else
                {
                    outlineObject.SetActive(false);
                    outlineLockNoKey.SetActive(true);
                }
            }
            else
            {
                if (outlineObject != null)
                    outlineObject.SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider is CapsuleCollider2D && collider.TryGetComponent<Player>(out _))
        {
            outlineObject.SetActive(false);
            if (_isLock && outlineLockNoKey != null)
            {
                outlineLockNoKey.SetActive(false);
            }
        }
    }
}
