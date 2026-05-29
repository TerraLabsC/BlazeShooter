using System.Collections.Generic;
using UnityEngine;

public class CarRowManager : MonoBehaviour
{
    public enum AlignmentAxis
    {
        WorldX,
        WorldY,
        WorldZ,
        StartForward,
        StartRight,
        StartUp
    }

    [Header("Настройки полосы")]
    [Tooltip("Точка, где должна находиться первая машина.")]
    public Transform laneStartPoint;

    [Tooltip("Вдоль какой оси строится ряд машин.")]
    public AlignmentAxis alignmentAxis = AlignmentAxis.WorldX;

    [Tooltip("Дистанция между машинами (откладывается вдоль выбранной оси).")]
    public float spacing = 5f;

    [Tooltip("Скорость перемещения машин.")]
    public float moveSpeed = 3f;

    [Tooltip("Список машин полосы: [0] – головная, [1] – следующая и т.д.")]
    public List<Transform> cars = new List<Transform>();

    // Вычисляем вектор направления для текущего ряда
    private Vector3 GetAlignmentDirection()
    {
        return alignmentAxis switch
        {
            AlignmentAxis.WorldX => Vector3.right,
            AlignmentAxis.WorldY => Vector3.up,
            AlignmentAxis.WorldZ => Vector3.forward,
            AlignmentAxis.StartForward => laneStartPoint != null ? laneStartPoint.forward : Vector3.forward,
            AlignmentAxis.StartRight => laneStartPoint != null ? laneStartPoint.right : Vector3.right,
            AlignmentAxis.StartUp => laneStartPoint != null ? laneStartPoint.up : Vector3.up,
            _ => Vector3.right
        };
    }

    private void Update()
    {
        if (laneStartPoint == null)
        {
            Debug.LogError("Не назначена точка старта полосы!");
            return;
        }

        cars.RemoveAll(car => car == null);
        if (cars.Count == 0) return;

        Vector3 queueDir = GetAlignmentDirection();

        // Первая машина движется к точке старта
        Transform firstCar = cars[0];
        Vector3 targetFirst = laneStartPoint.position;
        firstCar.position = Vector3.MoveTowards(firstCar.position, targetFirst, moveSpeed * Time.deltaTime);

        // Остальные машины выстраиваются позади впереди идущей
        for (int i = 1; i < cars.Count; i++)
        {
            Transform currentCar = cars[i];
            Transform carAhead = cars[i - 1];

            // Отступаем на spacing против направления очереди (позади)
            Vector3 targetPos = carAhead.position - queueDir * spacing;
            currentCar.position = Vector3.MoveTowards(currentCar.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (laneStartPoint == null) return;

        Vector3 queueDir = GetAlignmentDirection();

        // Точка старта
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(laneStartPoint.position, 0.3f);
        // Направление очереди (куда смотрит первая машина)
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(laneStartPoint.position, queueDir * 2f);

        // Цели для машин
        for (int i = 0; i < cars.Count; i++)
        {
            if (cars[i] == null) continue;

            Vector3 target;
            if (i == 0)
                target = laneStartPoint.position;
            else
                target = cars[i - 1].position - queueDir * spacing;

            bool reached = Vector3.Distance(cars[i].position, target) < 0.05f;
            Gizmos.color = reached ? Color.green : Color.yellow;
            Gizmos.DrawSphere(target, 0.25f);
            Gizmos.DrawLine(cars[i].position, target);
        }
    }

    public void RemoveFirstCar()
    {
        if (cars.Count > 0 && cars[0] != null)
            Destroy(cars[0].gameObject);
    }
}