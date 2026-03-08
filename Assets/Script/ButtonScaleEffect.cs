using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Button))]
public class ButtonScaleEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Header("缩放设置")]
    [Range(0.5f, 1f)]
    public float pressedScale = 0.9f;
    [Range(0.01f, 0.5f)]
    public float animationDuration = 0.1f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(pressedScale, animationDuration / 2f));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(1f, animationDuration / 2f));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    private System.Collections.IEnumerator ScaleTo(float targetScaleMultiplier, float duration)
    {
        Vector3 targetScale = originalScale * targetScaleMultiplier;
        Vector3 startScale = transform.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = elapsedTime / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        transform.localScale = targetScale;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        transform.localScale = originalScale;
    }
}
