using UnityEngine;
using UnityEngine.UI;

public class GameDisplay : MonoBehaviour
{
    private int _health;
    private int _maxHealth;
    private int _currentKeys;
    private bool _isPlayerCloseToInteract;
    private bool _isPlayerNearBreakable;

    [SerializeField] private Player player;
    [SerializeField] Animator animator;
    [Header("Hearts")]
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private Image[] hearts;
    [Header("Keys")]
    [SerializeField] private Sprite key;
    [SerializeField] private Image[] keys;

    private const string E = "E";
    private const string MOUSE = "Mouse";

    private void Update()
    {
        _maxHealth = player.GetPlayerMaxHealth();
        _health = player.GetPlayerCurrentHealth();
        _currentKeys = player.GetPlayerKeys();
        _isPlayerCloseToInteract = player.isCloseToInteract;
        _isPlayerNearBreakable = player.isNearBreakable;
        VisualiseCurrentHealth();
        VisualiseCurrentKeys();
        VisualiseHints();
    }
    //==========================
    private void VisualiseCurrentHealth()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < _health)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
            if (i < _maxHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }

    }
    private void VisualiseCurrentKeys()
    {
        for (int j = 0; j < keys.Length; j++)
        {
            if (j < _currentKeys)
            {
                keys[j].enabled = true;
            }
            else
            {
                keys[j].enabled = false;
            }
        }
    }
    private void VisualiseHints()
    {
        animator.SetBool(E, _isPlayerCloseToInteract);
        animator.SetBool(MOUSE, _isPlayerNearBreakable);
    }
}
