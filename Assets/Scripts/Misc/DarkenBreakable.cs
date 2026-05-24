using System.Collections;
using UnityEngine;

public class DarkenBreakable : MonoBehaviour
{
    private const float NON_TRANSPARENT = 1f;

    [Range(0f, 1f)]
    [SerializeField] private float darkenAmount = 0.85f;
    [SerializeField] private float darkenTime = 0.2f;

    private SpriteRenderer _spriteRenderer;
    private Color initialColorState;
    private float initialDarkness;
    //==============================================================
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        initialColorState = _spriteRenderer.color;
        initialDarkness = initialColorState.a;
        Debug.Log(initialColorState);
    }
    private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            StartCoroutine(DarkenRoutine(_spriteRenderer, darkenTime, initialDarkness, darkenAmount));
            Debug.Log("Enter obj");
        }
    }
    private void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            StartCoroutine(DarkenRoutine(_spriteRenderer, darkenTime, darkenAmount, initialDarkness));
            _spriteRenderer.color = initialColorState;
            Debug.Log("Left obj");
        }
    }
    //==============================================================
    private IEnumerator DarkenRoutine(SpriteRenderer spriteRenderer, float fadeTime, float startDarken, float targetDarken)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startDarken, targetDarken, elapsedTime / fadeTime);
            spriteRenderer.color = new Color(darkenAmount, darkenAmount, darkenAmount, NON_TRANSPARENT);
            yield return null;
        }
    }
}
