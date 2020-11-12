using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Rope : MonoBehaviour
{
    [SerializeField] private Image image;

    public UnityAction OnAnimationFinished;

    public bool IsFinished { get; private set; }

    /// <summary>
    /// Animates rope image, to move from 
    /// </summary>
    /// <param name="timeToAnimate"></param>
    /// <param name="timeToWait"></param>
    public Rope Animate(Vector2 initialPosition, Vector2 endPosition, float timeToAnimate, Ease easeType)
    {
        var rectTransform = image.rectTransform;

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 0);
        rectTransform.anchoredPosition = initialPosition;

        var direction = (endPosition - initialPosition).normalized;
        rectTransform.up = direction;

        rectTransform.DOSizeDelta(
            new Vector2(rectTransform.sizeDelta.x, Vector2.Distance(initialPosition, endPosition)),
            timeToAnimate).OnComplete(() =>
        {
            IsFinished = true;
            OnAnimationFinished?.Invoke();

        }).SetEase(easeType);

        return this;
    }
}