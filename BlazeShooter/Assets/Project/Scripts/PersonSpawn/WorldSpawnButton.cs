using DG.Tweening;
using UnityEngine;

public class WorldSpawnButton : MonoBehaviour
{
    [Tooltip("Имя префаба из списка BaseObjects.playerPrefabs")]
    public string prefabName;

    [Tooltip("Ссылка на ряд, к которому принадлежит этот объект")]
    public CarRowManager rowManager;

    private Transform objectToScale;
    [SerializeField] private float scaleDownDuration = 0.5f;
    private bool isActivated = false;   // защита от повторных кликов

    private void Start()
    {
        objectToScale = transform;

        rowManager = GetComponentInParent<CarRowManager>();
    }

    private void OnMouseDown()
    {
        // Если объект уже активирован или не первый – выход
        if (isActivated) return;

        if (rowManager != null && !rowManager.IsFirstInQueue(gameObject))
        {
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

        // Блокируем повторные нажатия
        isActivated = true;

        // Останавливаем движение всех машин в ряду
        if (rowManager != null)
            rowManager.LockMovement();

        // Спавним игрока (метод SpawnPlayerClickObject должен существовать в BaseObjects)
        BaseObjects.Instance.SpawnPlayerClickObject(prefabName);

        // Запускаем анимацию исчезновения
        DestroyAnimation();
    }

    public void DestroyAnimation()
    {
        objectToScale.DOScale(Vector3.zero, scaleDownDuration)
                     .SetEase(Ease.InBack)
                     .OnComplete(() =>
                     {
                         // Убираем первый объект из списка (теперь вторая машина станет первой)
                         if (rowManager != null)
                             rowManager.RemoveFirstFromList();

                         // Возобновляем движение машин
                         if (rowManager != null)
                             rowManager.UnlockMovement();

                         // Уничтожаем объект
                         Destroy(objectToScale.gameObject);
                     });
    }
}