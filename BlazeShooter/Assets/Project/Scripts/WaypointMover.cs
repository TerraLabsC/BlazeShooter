using UnityEngine;
using System.Collections.Generic;

public class WaypointMover : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private List<Transform> waypoints = new List<Transform>();
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 2f; // скорость поворота

    [Header("Поведение")]
    [SerializeField] private bool loop = true;        // зациклить маршрут
    [SerializeField] private bool reverseAtEnd = false; // идти обратно по точкам (вместо зацикливания)

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private bool isWaiting = false; // если нужна задержка на точках

    private void Start()
    {
        if (waypoints.Count == 0)
        {
            Debug.LogWarning("WaypointMover: список точек пуст!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (waypoints.Count == 0 || isWaiting) return;

        Transform targetWP = waypoints[currentWaypointIndex];
        Vector3 targetPosition = targetWP.position;
        Quaternion targetRotation = targetWP.rotation;

        // Перемещаем объект к точке
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Плавно поворачиваем к целевому вращению
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Проверяем, достигли ли точки
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            OnReachedWaypoint();
        }
    }

    private void OnReachedWaypoint()
    {
        // Можно добавить задержку, если нужно
        // StartCoroutine(WaitAtWaypoint());

        // Переход к следующей точке
        if (loop)
        {
            if (reverseAtEnd)
            {
                // Движение туда-обратно
                if (movingForward)
                {
                    if (currentWaypointIndex == waypoints.Count - 1)
                    {
                        movingForward = false;
                        currentWaypointIndex--;
                    }
                    else
                    {
                        currentWaypointIndex++;
                    }
                }
                else
                {
                    if (currentWaypointIndex == 0)
                    {
                        movingForward = true;
                        currentWaypointIndex++;
                    }
                    else
                    {
                        currentWaypointIndex--;
                    }
                }
            }
            else
            {
                // Обычный цикл: 0 -> 1 -> ... -> последний -> 0
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            }
        }
        else
        {
            // Без зацикливания: останавливаемся на последней
            if (currentWaypointIndex < waypoints.Count - 1)
            {
                currentWaypointIndex++;
            }
            else
            {
                enabled = false; // остановить движение
            }
        }
    }

    // Визуализация точек в редакторе
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] != null)
            {
                Gizmos.DrawSphere(waypoints[i].position, 0.05f);
                if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }
        }

        // Замкнуть линию, если loop и не reverse
        if (loop && !reverseAtEnd && waypoints.Count > 1 && waypoints[0] != null && waypoints[waypoints.Count - 1] != null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
        }
    }
}