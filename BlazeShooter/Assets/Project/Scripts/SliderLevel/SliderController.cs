using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderController : MonoBehaviour
{
    public static SliderController Instance { get; private set; }

    [SerializeField] private Slider slider;
    [SerializeField] private Transform[] dummyParents;

    [SerializeField] private TextMeshProUGUI remainingText;   // "X осталось из Y"
    [SerializeField] private TextMeshProUGUI percentText;     // "X%"

    private int totalObjects;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        if (dummyParents == null || dummyParents.Length == 0)
        {
            Debug.LogError("SliderController: Не назначены пустышки (dummyParents)!");
            return;
        }

        totalObjects = 0;
        foreach (Transform dummy in dummyParents)
        {
            if (dummy != null)
                totalObjects += dummy.childCount;
        }

        UpdateSliderValue();
    }

    public void UpdateSliderValue()
    {
        if (slider == null || dummyParents == null)
            return;

        int currentObjects = 0;
        foreach (Transform dummy in dummyParents)
        {
            if (dummy != null)
                currentObjects += dummy.childCount;
        }

        // Слайдер: 1 = всё живо, 0 = всё уничтожено
        float ratio = (totalObjects > 0) ? (float)currentObjects / totalObjects : 0f;
        slider.value = ratio;

        // Обновление текста (числа убывают от максимума к 0)
        if (remainingText != null || percentText != null)
        {
            float percentRemaining = ratio * 100f;   // 100% → 0%

            if (remainingText != null)
                remainingText.text = $"{currentObjects}/{totalObjects}";

            if (percentText != null)
                percentText.text = $"{percentRemaining:F0}%";
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}