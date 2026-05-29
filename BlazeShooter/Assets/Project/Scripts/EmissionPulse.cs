using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Renderer))]
public class EmissionLoop : MonoBehaviour
{
    [Header("Материал")]
    [Tooltip("Если не указан, будет использован материал текущего Renderer")]
    public Material targetMaterial;

    [Header("Диапазон интенсивности свечения")]
    public float minIntensity = 2.3f;
    public float maxIntensity = 3.7f;

    [Header("Цвет свечения")]
    [ColorUsage(true, true)]
    public Color emissionColor = Color.white;

    [Header("Анимация")]
    public float duration = 2f;          // время одного перехода (от min к max или обратно)
    public Ease ease = Ease.InOutSine;

    private Renderer _renderer;
    private Material _materialInstance;
    private Tweener _tweener;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        if (targetMaterial == null && _renderer != null)
            targetMaterial = _renderer.material;   // создаст инстанс

        // Работаем с локальной копией материала
        _materialInstance = new Material(targetMaterial);
        if (_renderer != null)
            _renderer.material = _materialInstance;

        // Включаем эмиссию в шейдере
        _materialInstance.EnableKeyword("_EMISSION");

        // Запускаем бесконечный цикл между min и max
        StartLoop();
    }

    private void StartLoop()
    {
        KillTween();

        // Начальное значение — min (или любое в диапазоне)
        SetIntensity(minIntensity);

        _tweener = DOVirtual.Float(minIntensity, maxIntensity, duration, SetIntensity)
                            .SetEase(ease)
                            .SetLoops(-1, LoopType.Yoyo);   // бесконечно: туда-сюда
    }

    private void SetIntensity(float value)
    {
        if (_materialInstance == null) return;

        Color finalColor = emissionColor * value;
        _materialInstance.SetColor("_EmissionColor", finalColor);

        // Включаем/выключаем ключевое слово для оптимизации (если нужно)
        if (value <= 0f)
            _materialInstance.DisableKeyword("_EMISSION");
        else
            _materialInstance.EnableKeyword("_EMISSION");
    }

    private void KillTween()
    {
        if (_tweener != null && _tweener.IsActive())
            _tweener.Kill();
        _tweener = null;
    }

    private void OnDestroy()
    {
        KillTween();
        if (_materialInstance != null)
            Destroy(_materialInstance);
    }
}