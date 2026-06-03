using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BaseObjects : MonoBehaviour
{
    public static BaseObjects Instance { get; private set; }

    [Header("UI")]
    public GameObject buttonPrefab;
    public Transform contentPanel;

    [Header("Player Prefabs")]
    public List<GameObject> playerPrefabs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Создаёт кнопку, окрашенную в цвет игрока, и запоминает префаб + оставшиеся патроны.
    /// </summary>
    public void RegisterPlayer(string prefabName, Color playerColor, int remainingAmmo)
    {
        if (buttonPrefab == null || contentPanel == null)
        {
            Debug.LogError("Не назначены buttonPrefab или contentPanel в BaseObjects!");
            return;
        }

        // Создаём корневой объект кнопки
        GameObject newButtonRoot = Instantiate(buttonPrefab, contentPanel);

        // Дочерний объект с индексом 0 — реальная кнопка
        if (newButtonRoot.transform.childCount == 0)
        {
            Debug.LogError("В префабе кнопки нет дочерних объектов!");
            Destroy(newButtonRoot);
            return;
        }

        Transform buttonTransform = newButtonRoot.transform.GetChild(0);

        // Окрашиваем кнопку
        Image buttonImage = buttonTransform.GetComponent<Image>();
        if (buttonImage != null)
            buttonImage.color = playerColor;

        // Навешиваем обработчик клика
        Button button = buttonTransform.GetComponent<Button>();
        if (button != null)
        {
            // Захватываем значения в локальные переменные для замыкания
            string capturedPrefabName = prefabName;
            int capturedAmmo = remainingAmmo;
            button.onClick.AddListener(() => SpawnPlayer(capturedPrefabName, capturedAmmo, newButtonRoot));
        }
        else
        {
            Debug.LogError("На дочернем объекте (индекс 0) нет компонента Button!");
        }
    }

    /// <summary>
    /// Спавнит игрока с заданным количеством патронов и удаляет кнопку.
    /// </summary>
    private void SpawnPlayer(string prefabName, int remainingAmmo, GameObject buttonToDestroy)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("prefabName пуст! Спавн невозможен.");
            Destroy(buttonToDestroy);
            return;
        }

        // Ищем префаб в списке
        GameObject prefabToSpawn = playerPrefabs.Find(p => p != null && p.name == prefabName);
        if (prefabToSpawn == null)
        {
            Debug.LogError($"Префаб с именем '{prefabName}' не найден в списке playerPrefabs!");
            Destroy(buttonToDestroy);
            return;
        }

        // Точка спавна
        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        if (WaypointManager.Instance != null && WaypointManager.Instance.Waypoints.Count > 0)
        {
            Transform firstWP = WaypointManager.Instance.Waypoints[0];
            if (firstWP != null)
            {
                spawnPos = firstWP.position;
                spawnRot = firstWP.rotation;
            }
        }

        GameObject newPlayer = Instantiate(prefabToSpawn, spawnPos, spawnRot);

        // Применяем сохранённые патроны и запускаем анимацию появления
        PlayerShoot playerShoot = newPlayer.GetComponent<PlayerShoot>();
        if (playerShoot != null)
        {
            playerShoot.SetInitialAmmo(remainingAmmo);
            playerShoot.PlaySpawnAnimation();
        }
        else
        {
            Debug.LogWarning("На префабе игрока нет компонента PlayerShoot!");
        }

        buttonToDestroy.GetComponentInChildren<Button>().interactable = false;

        buttonToDestroy.GetComponentInParent<ButtonScale>().AnimationUIZero();
    }

    public void SpawnPlayerClickObject(string prefabName)
    {
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError("prefabName пуст! Спавн невозможен.");
            return;
        }

        GameObject prefabToSpawn = playerPrefabs.Find(p => p != null && p.name == prefabName);
        if (prefabToSpawn == null)
        {
            Debug.LogError($"Префаб с именем '{prefabName}' не найден в списке playerPrefabs!");
            return;
        }

        Vector3 spawnPos = Vector3.zero;
        Quaternion spawnRot = Quaternion.identity;
        if (WaypointManager.Instance != null && WaypointManager.Instance.Waypoints.Count > 0)
        {
            Transform firstWP = WaypointManager.Instance.Waypoints[0];
            if (firstWP != null)
            {
                spawnPos = firstWP.position;
                spawnRot = firstWP.rotation;
            }
        }

        GameObject newPlayer = Instantiate(prefabToSpawn, spawnPos, spawnRot);

        // Просто запускаем анимацию появления (патроны выставятся максимальными в Start)
        PlayerShoot playerShoot = newPlayer.GetComponent<PlayerShoot>();
        if (playerShoot != null)
        {
            playerShoot.PlaySpawnAnimation();
        }
        else
        {
            Debug.LogWarning("На префабе игрока нет компонента PlayerShoot!");
        }
    }
}