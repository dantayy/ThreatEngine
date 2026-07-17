using System;
using DG.Tweening;
using UnityEngine;

public class RectTransformAnimationScript : MonoBehaviour
{
    public enum AnimateDirection
    {
        Left,
        Right,
        Up,
        Down,
    }

    [Header("Initialization")]
    [SerializeField]
    private GameObject window;

    [Header("Animation")]
    [SerializeField]
    private AnimateDirection animateInDirection = AnimateDirection.Left;

    [SerializeField]
    private AnimateDirection animateOutDirection = AnimateDirection.Left;

    [SerializeField]
    private Vector2 distanceToAnimate = new Vector2(100f, 100f);

    [SerializeField]
    private Ease ease = Ease.InOutSine;

    [SerializeField]
    private bool animateOnStart = true;

    [SerializeField]
    [Range(0.05f, 5f)]
    private float animationDuration = 0.5f;

    private RectTransform windowRectTransform;
    private CanvasGroup windowCanvasGroup;
    private bool isVisible;
    private Vector2 originalAnchoredPosition;
    private float originalCanvasAlpha = 1f;
    private Tween currentTween;

    public event Action OnOpenWindow;
    public event Action OnCloseWindow;

    void OnValidate()
    {
        if (window == null)
        {
            window = gameObject;
        }

        distanceToAnimate.x = Mathf.Max(0f, distanceToAnimate.x);
        distanceToAnimate.y = Mathf.Max(0f, distanceToAnimate.y);
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        if (window == null)
        {
            window = gameObject;
        }

        windowRectTransform = window.GetComponent<RectTransform>();
        windowCanvasGroup = window.GetComponent<CanvasGroup>();

        if (windowRectTransform != null)
        {
            originalAnchoredPosition = windowRectTransform.anchoredPosition;
        }

        if (windowCanvasGroup != null)
        {
            originalCanvasAlpha = windowCanvasGroup.alpha;
            windowCanvasGroup.alpha = 0;
        }

        isVisible = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (animateOnStart)
        {
            OpenWindow();
        }
    }

    /// Debug - Toggle the animation in Inspector by right-clicking this script attached to the GameObject.
    [ContextMenu("Toggle Open/Close")]
    public void ToggleOpenClose()
    {
        if (isVisible)
            CloseWindow();
        else
            OpenWindow();
    }

    public void OpenWindow()
    {
        if (isVisible)
            return;

        isVisible = true;
        OnOpenWindow?.Invoke();
        StopCurrentTween();

        window.SetActive(true);
        SetStartStateForOpen();
        currentTween = CreateOpenSequence();
    }

    public void CloseWindow()
    {
        if (!isVisible)
            return;

        isVisible = false;
        OnCloseWindow?.Invoke();
        StopCurrentTween();

        currentTween = CreateCloseSequence();
    }

    private void StopCurrentTween()
    {
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
            currentTween = null;
        }
    }

    private void SetStartStateForOpen()
    {
        var startPosition = GetPositionAtOffset(animateInDirection);
        windowRectTransform.anchoredPosition = startPosition;

        if (windowCanvasGroup != null)
        {
            windowCanvasGroup.alpha = 0f;
        }
    }

    /// Setup for DOTween Sequence for opening animation
    private Tween CreateOpenSequence()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(
            windowRectTransform
                .DOAnchorPos(originalAnchoredPosition, animationDuration)
                .SetEase(ease)
        );

        if (windowCanvasGroup != null)
        {
            sequence.Join(
                windowCanvasGroup.DOFade(originalCanvasAlpha, animationDuration).SetEase(ease)
            );
        }
        else
        {
            Debug.LogWarning(
                $"You don't have a CanvasGroup element attached to this GameObject! Can't tween opacity without it."
            );
        }

        sequence.OnComplete(() => currentTween = null);
        sequence.OnKill(() => currentTween = null);
        return sequence;
    }

    /// Setup for DOTween Sequence for closing animation
    private Tween CreateCloseSequence()
    {
        var outTarget = GetPositionAtOffset(animateOutDirection, true);
        var sequence = DOTween.Sequence();

        sequence.Append(
            windowRectTransform.DOAnchorPos(outTarget, animationDuration).SetEase(ease)
        );

        if (windowCanvasGroup != null)
        {
            sequence.Join(windowCanvasGroup.DOFade(0f, animationDuration).SetEase(ease));
        }

        sequence.OnComplete(() =>
        {
            currentTween = null;
        });
        sequence.OnKill(() => currentTween = null);
        return sequence;
    }

    private Vector3 GetPositionAtOffset(AnimateDirection direction, bool close = false)
    {
        Vector2 offset = direction switch
        {
            AnimateDirection.Left => new Vector2(-distanceToAnimate.x, 0f),
            AnimateDirection.Right => new Vector2(distanceToAnimate.x, 0f),
            AnimateDirection.Up => new Vector2(0f, distanceToAnimate.y),
            AnimateDirection.Down => new Vector2(0f, -distanceToAnimate.y),
            _ => Vector2.zero,
        };

        return originalAnchoredPosition + offset;
    }
}
