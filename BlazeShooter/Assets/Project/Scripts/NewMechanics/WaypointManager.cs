using UnityEngine;
using System.Collections.Generic;

public class WaypointManager : MonoBehaviour
{
    [Header("Singleton")]
    public static WaypointManager Instance { get; private set; }

    [Header("Коллекция точек пути")]
    [Tooltip("Список Transform-точек. Позиция и поворот каждой задают целевую позу.")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    /// <summary>
    /// Публичный доступ к коллекции (только для чтения извне).
    /// </summary>
    public List<Transform> Waypoints => waypoints;

    private void Awake()
    {
        // Реализация классического синглтона
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);   // опционально, чтобы объект жил между сценами
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Визуализация точек в редакторе
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        // Отрисовка сфер и осей
        Gizmos.color = Color.green;
        foreach (Transform wp in waypoints)
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

        // Соединительные линии
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
            int next = (i + 1) % waypoints.Count;
            if (waypoints[next] == null) continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
        }
    }
}