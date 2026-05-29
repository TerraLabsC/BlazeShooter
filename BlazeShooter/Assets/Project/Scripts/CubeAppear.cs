using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ScalePresetApplier))]
public class CubeAppear : MonoBehaviour
{
    [SerializeField] private float animationDuration = 0.5f; // длительность самого скейла
    [SerializeField] private float maxDelay = 1f;            // максимальна€ случайна€ задержка (0..maxDelay)
    [SerializeField] private Ease ease = Ease.OutBack;       // тип плавности

    private void Awake()
    {
        // «апоминаем оригинальный размер куба
        Vector3 originalScale = GetComponent<ScalePresetApplier>().scalePreset2;

        // «апускаем твин: от нулевого размера до оригинального
        // From(Vector3.zero) мгновенно ставит scale в 0 и анимирует до originalScale
        // SetDelay задаЄт случайную паузу перед началом анимации
        transform.DOScale(originalScale, animationDuration)
            .From(Vector3.zero)
            .SetDelay(Random.Range(0f, maxDelay))
            .SetEase(ease);
    }
}