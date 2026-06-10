using DG.Tweening;
using UnityEngine;
using System.Collections;

public class WorldSpawnButton : MonoBehaviour
{
    [Tooltip("Имя префаба из списка BaseObjects.playerPrefabs")]
    public string prefabName;

    [Tooltip("Ссылка на ряд, к которому принадлежит этот объект")]
    public CarRowManager rowManager;

    private Transform objectToScale;
    [SerializeField] private float scaleDownDuration = 0.5f;
    private bool isActivated = false;

    [Header("UI-индикаторы")]
    [SerializeField] private GameObject cannotInteractImage;
    [SerializeField] private GameObject canInteractImage;

    [Header("Анимации иконок")]
    [SerializeField] private float iconFadeDuration = 0.5f;     // длительность появления/исчезновения
    [SerializeField] private float cannotShowDuration = 1f;     // сколько видна иконка «нельзя»
    [SerializeField] private float swayAngle = 8f;              // амплитуда покачивания
    [SerializeField] private float swayDuration = 1.2f;         // цикл покачивания

    private float nextIconCheckTime;
    private bool isIconShowing = false;

    private Tween iconScaleTween;
    private Tween iconSwayTween;
    private Sequence cannotSequence;   // DOTween Sequence для анимации «нельзя»

    private void Start()
    {
        objectToScale = transform;
        rowManager = GetComponentInParent<CarRowManager>();

        // Инициализация иконок (скрыты, scale=0)
        if (cannotInteractImage != null)
        {
            cannotInteractImage.SetActive(false);
            cannotInteractImage.transform.localScale = Vector3.zero;
        }
        if (canInteractImage != null)
        {
            canInteractImage.SetActive(false);
            canInteractImage.transform.localScale = Vector3.zero;
        }

        nextIconCheckTime = Time.time + 15f;
    }

    private void Update()
    {
        bool isAvailable = !isActivated && (rowManager == null || rowManager.IsFirstInQueue(gameObject));

        if (Time.time >= nextIconCheckTime)
        {
            if (isAvailable)
            {
                bool shouldShow = Random.value <= 0.05f;
                if (shouldShow && !isIconShowing)
                    ShowCanInteractIcon();
                else if (!shouldShow && isIconShowing)
                    HideCanInteractIcon();
            }
            else
            {
                if (isIconShowing)
                    HideCanInteractIcon();
            }
            nextIconCheckTime = Time.time + 15f;
        }
        else
        {
            if (!isAvailable && isIconShowing)
                HideCanInteractIcon();
        }
    }

    private void ShowCanInteractIcon()
    {
        if (canInteractImage == null) return;

        KillIconTweens();

        canInteractImage.SetActive(true);
        canInteractImage.transform.localScale = Vector3.zero;
        canInteractImage.transform.localRotation = Quaternion.identity;

        iconScaleTween = canInteractImage.transform.DOScale(1f, iconFadeDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                iconSwayTween = canInteractImage.transform
                    .DOLocalRotate(new Vector3(0f, 0f, swayAngle), swayDuration / 2f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        isIconShowing = true;
    }

    private void HideCanInteractIcon()
    {
        if (canInteractImage == null || !isIconShowing) return;

        KillIconTweens();

        iconScaleTween = canInteractImage.transform.DOScale(0f, iconFadeDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                canInteractImage.SetActive(false);
                canInteractImage.transform.localRotation = Quaternion.identity;
            });

        isIconShowing = false;
    }

    private void KillIconTweens()
    {
        if (iconScaleTween != null && iconScaleTween.IsActive())
            iconScaleTween.Kill();
        if (iconSwayTween != null && iconSwayTween.IsActive())
            iconSwayTween.Kill();
    }

    private void OnMouseDown()
    {
        if (isActivated) return;

        if (rowManager != null && !rowManager.IsFirstInQueue(gameObject))
        {
            ShowCannotInteractImage();
            Debug.Log("Этот объект нельзя нажать, он не первый в очереди.");
            return;
        }

        if (BaseObjects.Instance == null)
        {
            Debug.LogError("BaseObjects.Instance не найден на сцене!");
            return;
        }

        bool found = BaseObjects.Instance.playerPrefabs.Exists(p => p != null && p.name == prefabName);
        if (!found)
        {
            Debug.LogWarning($"Префаб с именем '{prefabName}' отсутствует в BaseObjects.playerPrefabs!");
            return;
        }

        isActivated = true;

        if (rowManager != null)
            rowManager.LockMovement();

        if (isIconShowing)
            HideCanInteractIcon();

        BaseObjects.Instance.SpawnPlayerClickObject(prefabName);
        DestroyAnimation();
    }

    /// <summary>
    /// Плавное появление, пауза и исчезновение иконки «нельзя».
    /// </summary>
    private void ShowCannotInteractImage()
    {
        if (cannotInteractImage == null) return;

        // Прерываем предыдущую анимацию, если была
        if (cannotSequence != null && cannotSequence.IsActive())
            cannotSequence.Kill();

        cannotInteractImage.SetActive(true);
        cannotInteractImage.transform.localScale = Vector3.zero;

        cannotSequence = DOTween.Sequence();
        cannotSequence.Append(cannotInteractImage.transform.DOScale(1f, iconFadeDuration).SetEase(Ease.OutBack));
        cannotSequence.AppendInterval(cannotShowDuration);
        cannotSequence.Append(cannotInteractImage.transform.DOScale(0f, iconFadeDuration).SetEase(Ease.InBack));
        cannotSequence.OnComplete(() =>
        {
            cannotInteractImage.SetActive(false);
            cannotSequence = null;
        });
    }

    public void DestroyAnimation()
    {
        objectToScale.DOScale(Vector3.zero, scaleDownDuration)
                     .SetEase(Ease.InBack)
                     .OnComplete(() =>
                     {
                         if (rowManager != null)
                             rowManager.RemoveFirstFromList();

                         if (rowManager != null)
                             rowManager.UnlockMovement();

                         Destroy(objectToScale.gameObject);
                     });
    }

    private void OnDestroy()
    {
        KillIconTweens();
        if (cannotSequence != null && cannotSequence.IsActive())
            cannotSequence.Kill();
    }
}