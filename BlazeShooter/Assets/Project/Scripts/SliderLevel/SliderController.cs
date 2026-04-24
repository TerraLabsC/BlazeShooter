using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public static SliderController Instance { get; private set; }

    [SerializeField] private Transform targetParent; // Родитель, чьих потомков считаем
    [SerializeField] private Slider slider;          // Сам слайдер

    private int totalChildren;

    private void Awake()
    {
        // Реализуем статичный instance (синглтон)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (targetParent == null)
        {
            Debug.LogError("SliderController: Не назначен Target Parent!");
            return;
        }

        if (slider == null)
            slider = GetComponent<Slider>();

        totalChildren = targetParent.childCount;
        UpdateSliderValue();
    }

    /// <summary>
    /// Вызывается при уничтожении любого дочернего объекта.
    /// </summary>
    public void UpdateSliderValue()
    {
        if (slider == null || targetParent == null)
            return;

        // Текущее количество живых детей
        int currentChildren = targetParent.childCount;

        // Процентное соотношение (от 0 до 1)
        float ratio = (totalChildren > 0) ? (float)currentChildren / totalChildren : 0f;
        slider.value = ratio; // Слайдер плавно уходит от 1 к 0
    }

    private void OnDestroy()
    {
        // Очищаем instance, чтобы не осталось висячих ссылок
        if (Instance == this)
            Instance = null;
    }
}