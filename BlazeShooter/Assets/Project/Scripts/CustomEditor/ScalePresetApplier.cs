using UnityEngine;

/// <summary>
/// Хранит пресеты масштаба и применяет их по вызову из редактора.
/// </summary>
public class ScalePresetApplier : MonoBehaviour
{
    [Header("Масштаб для кнопки 1 (бывший Numpad1)")]
    public Vector3 scalePreset1 = Vector3.zero;

    [Header("Масштаб для кнопки 2 (бывший Numpad2)")]
    public Vector3 scalePreset2 = new Vector3(0.05740175f, 0.2430197f, 0.05740175f);

    // Методы вызываются кастомным редактором при нажатии кнопок
    public void ApplyPreset1()
    {
        transform.localScale = scalePreset1;
    }

    public void ApplyPreset2()
    {
        transform.localScale = scalePreset2;
    }

    // Вызывается в редакторе при каждом изменении значений в инспекторе
    private void OnValidate()
    {
        // Запрещаем отрицательный масштаб (при желании можно убрать или изменить)
        scalePreset1 = ClampNegative(scalePreset1);
        //scalePreset2 = ClampNegative(scalePreset2);
    }

    private Vector3 ClampNegative(Vector3 v)
    {
        return new Vector3(Mathf.Max(0, v.x), Mathf.Max(0, v.y), Mathf.Max(0, v.z));
    }
}