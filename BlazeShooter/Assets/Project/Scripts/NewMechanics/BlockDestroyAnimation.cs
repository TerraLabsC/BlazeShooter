using UnityEngine;
using DG.Tweening;

public class BlockDestroyAnimation : MonoBehaviour
{
    [SerializeField] private float destroyDuration = 0.3f; // длительность анимации
    private bool isDestroying = false;

    /// <summary>
    /// Запускает анимацию уничтожения (уменьшение до нуля и удаление)
    /// </summary>
    public void PlayDestroyAnimation()
    {
        if (isDestroying)
            return;

        isDestroying = true;

        // Отключаем коллайдер, чтобы в блок нельзя было попасть повторно
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // Анимация масштаба: уменьшаем до нуля и по окончании удаляем объект
        transform.DOScale(Vector3.zero, destroyDuration)
            .SetEase(Ease.InBack)   // можно выбрать другую функцию плавности, например Ease.OutQuad
            .OnComplete(() => Destroy(gameObject));
    }
}