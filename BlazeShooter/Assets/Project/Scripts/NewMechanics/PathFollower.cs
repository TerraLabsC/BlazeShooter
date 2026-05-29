using UnityEngine;
using System.Collections.Generic;

public class PathFollower : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public bool loop = true;

    [Header("Текущее состояние")]
    [SerializeField] private int currentWaypointIndex = 0;

    // Локальная ссылка на коллекцию точек из синглтона
    private List<Transform> waypoints;

    private void Start()
    {
        // Получаем коллекцию из синглтона
        if (WaypointManager.Instance != null)
        {
            waypoints = WaypointManager.Instance.Waypoints;
        }
        else
        {
            Debug.LogError("WaypointManager не найден на сцене!");
        }
    }

    private void Update()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Transform targetWP = waypoints[currentWaypointIndex];

        // Защита от отсутствующей точки (null в списке)
        if (targetWP == null)
        {
            Debug.LogWarning($"Путевая точка под индексом {currentWaypointIndex} равна null. Пропускаем.");
            MoveToNextWaypoint();
            return;
        }

        // Движение к точке
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetWP.position,
            moveSpeed * Time.deltaTime
        );

        // Поворот к целевому повороту точки
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetWP.rotation,
            rotationSpeed * Time.deltaTime
        );

        // Проверка достижения точки
        if (Vector3.Distance(transform.position, targetWP.position) < 0.01f)
        {
            MoveToNextWaypoint();
        }
    }

    private void MoveToNextWaypoint()
    {
        currentWaypointIndex++;
        if (currentWaypointIndex >= waypoints.Count)
        {
            if (loop)
                currentWaypointIndex = 0;
            else
                enabled = false;   // останавливаем скрипт
        }
    }

    // Для наглядности в редакторе используем ту же коллекцию из синглтона
    private void OnDrawGizmos()
    {
        if (WaypointManager.Instance == null)
            return;

        List<Transform> wps = WaypointManager.Instance.Waypoints;
        if (wps == null || wps.Count == 0)
            return;

        Gizmos.color = Color.green;
        foreach (Transform wp in wps)
        {
            if (wp == null) continue;

            Gizmos.DrawSphere(wp.position, 0.2f);

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(wp.position, wp.forward * 0.5f);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(wp.position, wp.up * 0.3f);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(wp.position, wp.right * 0.3f);

            Gizmos.color = Color.green;
        }

        Gizmos.color = Color.yellow;
        for (int i = 0; i < wps.Count; i++)
        {
            if (wps[i] == null) continue;
            int next = (i + 1) % wps.Count;
            if (wps[next] == null) continue;
            Gizmos.DrawLine(wps[i].position, wps[next].position);
        }
    }
}