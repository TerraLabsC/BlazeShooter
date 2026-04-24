using UnityEngine;

/// <summary>
/// Вешаем на каждый дочерний объект, который может быть уничтожен.
/// </summary>
public class DestroyNotifier : MonoBehaviour
{
    private void OnDestroy()
    {
        // Безопасно обращаемся к синглтону и обновляем слайдер
        if (SliderController.Instance != null)
        {
            SliderController.Instance.UpdateSliderValue();
        }
    }
}