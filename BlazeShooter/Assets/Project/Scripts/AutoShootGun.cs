using UnityEngine;

public class AutoShootGun : MonoBehaviour
{
    [Header("Настройки стрельбы")]
    [SerializeField] private float range = 50f;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private LayerMask targetLayers = ~0;

    [Header("Цвет пушки")]
    [SerializeField] private Color gunColor = Color.red;
    [SerializeField] private float colorTolerance = 0.1f;

    [Header("Точность попадания в центр")]
    [SerializeField] private float centerHitTolerance = 0.1f;

    [Header("Настройки пули")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    [Header("Визуализация")]
    [SerializeField] private bool showDetectionRay = true;

    private float nextFireTime = 0f;

    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            if (Physics.Raycast(GetFireOrigin(), transform.forward, out RaycastHit hit, range, targetLayers))
            {
                ColoredCube cube = hit.collider.GetComponent<ColoredCube>();
                if (cube != null && ColorsAreClose(cube.CubeColor, gunColor, colorTolerance))
                {
                    // Проверяем, не назначена ли уже пуля для этого кубика
                    if (cube.AssignedBullet == null)
                    {
                        if (IsHitCenter(hit))
                        {
                            Shoot(cube);
                            nextFireTime = Time.time + fireRate;
                        }
                    }
                }
            }
        }
    }

    private Vector3 GetFireOrigin()
    {
        return firePoint != null ? firePoint.position : transform.position;
    }

    private bool ColorsAreClose(Color a, Color b, float tolerance)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }

    private bool IsHitCenter(RaycastHit hit)
    {
        Vector3 colliderCenter = hit.collider.bounds.center;
        float distanceToCenter = Vector3.Distance(hit.point, colliderCenter);
        return distanceToCenter <= centerHitTolerance;
    }

    private void Shoot(ColoredCube targetCube)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet Prefab не назначен!");
            return;
        }

        Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position;
        Quaternion spawnRot = firePoint != null ? firePoint.rotation : transform.rotation;
        GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, spawnRot);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.SetColor(gunColor);
            bullet.AssignTarget(targetCube); // Привязываем пулю к кубику
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDetectionRay) return;
        Gizmos.color = gunColor;
        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Gizmos.DrawRay(origin, transform.forward * range);
    }
}