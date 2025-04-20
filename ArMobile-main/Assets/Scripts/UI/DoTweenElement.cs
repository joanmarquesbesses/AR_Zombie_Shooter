using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public class DoTweenElement : MonoBehaviour
{
    public enum SpawnEffectType { None, Scale, Rotate, Vibrate, Bounce, Shake, Move, ColorChange, Return, ReturnScale, FadeIn, Pulse, Spin, Wave, Flip }
    public enum UpdateEffectType { None, Dance, Spin, Wave, RotateTwice }
    public enum EndEffectType { None, FadeOut, Shrink }

    [Header("Repeat Effects Settings")]
    public bool repeatSpawnEffectsOnEnable = true;
    public bool repeatEndEffectsOnDisable = true;

    [System.Serializable]
    public class TweenSettings<T>
    {
        public bool enabled = false;
        [ConditionalField(nameof(enabled))]
        public T effectType;

        [ConditionalField(nameof(enabled))]
        public float duration = 0.5f;
        [ConditionalField(nameof(enabled))]
        public float delay = 0f;
        [ConditionalField(nameof(enabled))]
        public Ease ease = Ease.OutBack;
        [ConditionalField(nameof(enabled))]
        public bool ignoreTimeScale = false;

        #region Spawn Effects
        [ConditionalField(nameof(effectType), false, SpawnEffectType.Scale)]
        public Vector3 targetScale = Vector3.one;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Rotate)]
        public Vector3 rotationAngle = new Vector3(0, 0, 360);

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Vibrate)]
        public float vibrationIntensity = 10f;
        [ConditionalField(nameof(effectType), false, SpawnEffectType.Vibrate)]
        public int vibrato = 10;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Bounce)]
        public float bounceHeight = 50f;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Shake)]
        public Vector3 shakeStrength = Vector3.one;
        [ConditionalField(nameof(effectType), false, SpawnEffectType.Shake)]
        public int shakeVibrato = 10;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Move)]
        public Vector3 targetLocation = Vector3.zero;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.ColorChange)]
        public Color targetColor = Color.white;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Return)]
        public Vector3 offset = Vector3.zero;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Pulse)]
        public float pulseScale = 1.2f;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Spin)]
        public float spinSpeed = 360f;

        [ConditionalField(nameof(effectType), false, SpawnEffectType.Wave)]
        public float waveHeight = 20f;
        #endregion

        #region Update Effects
        [ConditionalField(nameof(effectType), false, UpdateEffectType.Dance)]
        public float danceHeight = 20f;

        [ConditionalField(nameof(effectType), false, UpdateEffectType.Spin)]
        public float updateSpinSpeed = 360f;

        [ConditionalField(nameof(effectType), false, UpdateEffectType.Wave)]
        public float updateWaveHeight = 20f;

        [ConditionalField(nameof(effectType), false, UpdateEffectType.RotateTwice)]
        public float RotateByAmount = 20f;
        #endregion
    }

    public List<TweenSettings<SpawnEffectType>> spawnEffects = new List<TweenSettings<SpawnEffectType>>();

    public List<TweenSettings<UpdateEffectType>> updateEffects = new List<TweenSettings<UpdateEffectType>>();

    public List<TweenSettings<EndEffectType>> endEffects = new List<TweenSettings<EndEffectType>>();

    private RectTransform rectTransform;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Vector3 initialRotation;
    private Color initialColor = Color.white;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("DoTweenElement requires a RectTransform component.");
            enabled = false;
            return;
        }

        initialScale = rectTransform.localScale;
        initialPosition = rectTransform.localPosition;
        initialRotation = rectTransform.localEulerAngles;
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            initialColor = renderer.material.color;
        }

        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null && (spawnEffects.Exists(e => e.effectType == SpawnEffectType.FadeIn) ||
                                    endEffects.Exists(e => e.effectType == EndEffectType.FadeOut)))
        {
            gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void Start()
    {
        if (!repeatSpawnEffectsOnEnable)
        {
            ApplySpawnEffects();

            ApplyUpdateEffects();
        }
    }
    private void OnDestroy()
    {
        if (!repeatEndEffectsOnDisable)
        {
            ApplyEndEffects();
        }
    }

    private void OnEnable()
    {
        ResetState();

        if (repeatSpawnEffectsOnEnable)
        {
            rectTransform.DOKill();
            ApplySpawnEffects();

            ApplyUpdateEffects();
        }

        rectTransform.DOPlay();
    }

    private void OnDisable()
    {
        rectTransform.DOPause();

        if (repeatEndEffectsOnDisable)
        {
            ApplyEndEffects();
        }
    }

    private void ApplySpawnEffects()
    {
        foreach (var spawnEffect in spawnEffects)
        {
            if (spawnEffect.enabled)
                ApplyEffect(spawnEffect);
        }
    }
    private void ApplyUpdateEffects()
    {
        foreach (var updateEffect in updateEffects)
        {
            if (updateEffect.enabled)
                ApplyEffect(updateEffect);
        }
    }
    private void ApplyEndEffects()
    {
        foreach (var endEffect in endEffects)
        {
            if (endEffect.enabled)
                ApplyEffect(endEffect);
        }
    }

    private void ResetState()
    {
        rectTransform.localScale = initialScale;
        rectTransform.localPosition = initialPosition;
        rectTransform.localEulerAngles = initialRotation;

        var renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = initialColor;
        }

        var canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
        }
    }

    private void ApplyEffect<T>(TweenSettings<T> settings)
    {
        if (!settings.enabled) return;

        switch (settings.effectType)
        {
            #region Spawn Effects
            case SpawnEffectType.Scale:
                rectTransform.DOBlendableScaleBy(settings.targetScale, settings.duration)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Rotate:
                rectTransform.DOBlendableRotateBy(settings.rotationAngle, settings.duration, RotateMode.FastBeyond360)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Vibrate:
                rectTransform.DOShakePosition(settings.duration, settings.vibrationIntensity, settings.vibrato)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Bounce:
                rectTransform.DOBlendableLocalMoveBy(new Vector3(initialPosition.x, initialPosition.y + settings.bounceHeight, initialPosition.z), settings.duration / 2)
                             .SetEase(Ease.OutQuad)
                             .SetLoops(2, LoopType.Yoyo)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Shake:
                rectTransform.DOShakeScale(settings.duration, settings.shakeStrength, settings.shakeVibrato)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Move:
                rectTransform.DOBlendableLocalMoveBy(settings.targetLocation, settings.duration)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.ColorChange:
                var renderer = GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.DOBlendableColor(settings.targetColor, settings.duration)
                                     .SetEase(settings.ease)
                                     .SetDelay(settings.delay)
                                     .SetUpdate(settings.ignoreTimeScale);
                }
                break;

            case SpawnEffectType.Return:
                rectTransform.localPosition = initialPosition + settings.offset;
                rectTransform.DOBlendableLocalMoveBy(-settings.offset, settings.duration)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.ReturnScale:
                rectTransform.localScale = Vector3.zero;
                rectTransform.DOBlendableScaleBy(initialScale, settings.duration)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;
            case SpawnEffectType.FadeIn:
                var canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.DOFade(1, settings.duration)
                               .SetEase(settings.ease)
                               .SetDelay(settings.delay)
                               .SetUpdate(settings.ignoreTimeScale);
                }
                break;

            case SpawnEffectType.Pulse:
                rectTransform.DOBlendableScaleBy(initialScale * settings.pulseScale - initialScale, settings.duration / 2)
                             .SetEase(settings.ease)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Spin:
                rectTransform.DOBlendableRotateBy(new Vector3(0, 0, settings.spinSpeed), settings.duration, RotateMode.FastBeyond360)
                             .SetEase(settings.ease)
                             .SetLoops(-1, LoopType.Incremental)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Wave:
                rectTransform.DOBlendableLocalMoveBy(new Vector3(0, settings.waveHeight, 0), settings.duration / 2)
                             .SetEase(settings.ease)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case SpawnEffectType.Flip:
                rectTransform.DOBlendableScaleBy(new Vector3(-1, 1, 1), settings.duration / 2)
                             .SetEase(settings.ease)
                             .SetUpdate(settings.ignoreTimeScale)
                             .OnComplete(() => rectTransform.DOBlendableScaleBy(new Vector3(1, 1, 1), settings.duration / 2)
                             .SetEase(settings.ease)
                             .SetUpdate(settings.ignoreTimeScale));
                break;

            #endregion

            #region Update Effects
            case UpdateEffectType.Dance:
                rectTransform.DOBlendableLocalMoveBy(new Vector3(0, settings.danceHeight, 0), settings.duration / 2)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case UpdateEffectType.Spin:
                rectTransform.DOBlendableRotateBy(new Vector3(0, 0, settings.updateSpinSpeed), settings.duration, RotateMode.FastBeyond360)
                             .SetEase(settings.ease)
                             .SetLoops(-1, LoopType.Incremental)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case UpdateEffectType.Wave:
                rectTransform.DOBlendableLocalMoveBy(new Vector3(0, settings.updateWaveHeight, 0), settings.duration / 2)
                             .SetEase(settings.ease)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;

            case UpdateEffectType.RotateTwice:
                rectTransform.DOBlendableRotateBy(new Vector3(0, 0, settings.RotateByAmount), settings.duration / 2, RotateMode.FastBeyond360)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetLoops(-1, LoopType.Yoyo)
                             .SetUpdate(settings.ignoreTimeScale);
                break;
            #endregion

            #region End Effects
            case EndEffectType.FadeOut:
                var canvasGroupEnd = GetComponent<CanvasGroup>();
                if (canvasGroupEnd != null)
                {
                    canvasGroupEnd.DOFade(0, settings.duration)
                                  .SetEase(settings.ease)
                                  .SetDelay(settings.delay)
                                  .SetUpdate(settings.ignoreTimeScale);
                }
                break;

            case EndEffectType.Shrink:
                rectTransform.DOBlendableScaleBy(-initialScale, settings.duration)
                             .SetEase(settings.ease)
                             .SetDelay(settings.delay)
                             .SetUpdate(settings.ignoreTimeScale);
                break;
            #endregion

            case SpawnEffectType.None:
            case UpdateEffectType.None:
            case EndEffectType.None:
            default:
                break;
        }
    }
}
