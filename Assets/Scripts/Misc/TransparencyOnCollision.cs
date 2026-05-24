using System.Collections;
using UnityEngine;

public class TransparencyOnCollision : MonoBehaviour
{
    private const float NON_TRANSPARENT = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float transparencyAmount = 0.8f;
    [SerializeField] private float fadeTime = 0.2f;

    private SpriteRenderer _spriteRenderer;
    //==============================================================
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>() && (collider is CapsuleCollider2D))
        {
            StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, _spriteRenderer.color.a, transparencyAmount));
        }
    }
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Player>())
        {
            StartCoroutine(FadeRoutine(_spriteRenderer, fadeTime, transparencyAmount, NON_TRANSPARENT));
        }
    }
    //==============================================================
    private IEnumerator FadeRoutine(SpriteRenderer spriteRenderer, float fadeTime, float startTransparency, float targetTransparency)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startTransparency, targetTransparency, elapsedTime / fadeTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, newAlpha);
            yield return null;
        }
    }
}
