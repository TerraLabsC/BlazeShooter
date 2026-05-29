using UnityEngine;
using TMPro;
using DG.Tweening;

public class PlayerShoot : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 50f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float ballSpeed = 20f;
    [SerializeField] private float fireRate = 0.5f;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 40;
    private int currentAmmo;

    [Header("UI & Visuals")]
    [SerializeField] private TextMeshProUGUI ammoText;
    private Transform objectToScale;
    [SerializeField] private float scaleDownDuration = 0.5f;

    [Header("Current Color")]
    [SerializeField] private ColorType currentColor = ColorType.Red;

    private float nextFireTime;
    private bool targetInSight;
    private bool isMagazineEmpty = false;

    public bool IsActive = false;

    [Header("Weapon Model Stretch")]
    [SerializeField] private Transform visualModelTransform;   // уже существующее поле

    // ──── Новые поля для анимации отдачи ────
    [Header("Recoil Animation")]
    [SerializeField] private float recoilDistance = 0.05f;     // насколько отъезжает назад (по локальной оси Z)
    [SerializeField] private float recoilDuration = 0.15f;     // длительность анимации
    [SerializeField] private int recoilVibrato = 5;            // количество колебаний
    [SerializeField] private float recoilElasticity = 0.3f;    // упругость (0..1)
    [SerializeField] private float recoilAngle = 2f;           // угол подброса ствола вверх (градусы)

    public bool IsActuve()   // сохранено как в оригинале, можно заменить на IsActive
    {
        return IsActive;
    }

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        if (objectToScale == null)
            objectToScale = transform;
    }

    private void Update()
    {
        if (isMagazineEmpty) return;

        if (IsActive)
        {
            targetInSight = PerformRaycastAndCheck();

            if (targetInSight && Time.time >= nextFireTime && currentAmmo > 0)
            {
                ShootBall();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    private bool PerformRaycastAndCheck()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.white);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            BlockColor block = hit.collider.GetComponent<BlockColor>();
            if (block != null)
            {
                if (block.colorType == currentColor)
                    return true;
            }
        }
        return false;
    }

    private void ShootBall()
    {
        if (ballPrefab == null || firePoint == null)
        {
            Debug.LogWarning("ballPrefab или firePoint не назначены!");
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();

        GameObject ball = Instantiate(ballPrefab, firePoint.position, firePoint.rotation);
        Ball ballScript = ball.GetComponent<Ball>();
        if (ballScript != null)
        {
            ballScript.assignedColor = currentColor;
            ballScript.speed = ballSpeed;
        }
        else
        {
            Debug.LogError("На префабе шарика нет компонента Ball!");
        }

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        // ──── ВЫЗОВ АНИМАЦИИ ОТДАЧИ ────
        PlayRecoil();

        if (currentAmmo <= 0)
        {
            EmptyMagazine();
        }
    }

    /// <summary>
    /// Пружинистая отдача визуальной модели оружия.
    /// </summary>
    private void PlayRecoil()
    {
        if (visualModelTransform == null) return;

        // Останавливаем предыдущую анимацию, чтобы не было наложения
        visualModelTransform.DOKill(true);

        // Откат по позиции (назад по локальной Z)
        visualModelTransform.DOPunchPosition(
            new Vector3(0, 0, -recoilDistance),
            recoilDuration,
            recoilVibrato,
            recoilElasticity
        );

        // Подброс ствола (поворот вокруг локальной оси X)
        visualModelTransform.DOPunchRotation(
            new Vector3(-recoilAngle, 0, 0),
            recoilDuration,
            recoilVibrato,
            recoilElasticity
        );
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
            ammoText.text = $"{currentAmmo}";
    }

    private void EmptyMagazine()
    {
        isMagazineEmpty = true;
        objectToScale.DOScale(Vector3.zero, scaleDownDuration)
                     .SetEase(Ease.InBack)
                     .OnComplete(() => Debug.Log("Магазин пуст, объект скрыт"));
    }
}