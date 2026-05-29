using UnityEngine;

/// <summary>
/// Поворачивает UI-элемент лицом к камере.
/// Применяется к объектам на World Space Canvas (например, полоски здоровья, иконки над персонажами).
/// </summary>
public class BillboardUI : MonoBehaviour
{
    [Tooltip("Камера, к которой нужно поворачиваться. Если не задана, используется Camera.main.")]
    public Camera targetCamera;

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        // Способ 1: полный билборд — объект смотрит туда же, куда и камера, сохраняя свой "верх" по вертикали мира.
        transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                         targetCamera.transform.rotation * Vector3.up);

        // Альтернатива (проще, но может вращать UI "вверх ногами" при определённых углах):
        // transform.LookAt(targetCamera.transform);
    }
}