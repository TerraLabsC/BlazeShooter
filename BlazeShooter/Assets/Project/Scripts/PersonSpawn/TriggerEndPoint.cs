using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TriggerEndPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerRespawn playerRespawn = other.GetComponent<PlayerRespawn>();
        if (playerRespawn == null)
            return;

        PlayerShoot playerShoot = other.GetComponent<PlayerShoot>();

        // Получаем цвет игрока
        PersonColor personColor = other.GetComponent<PersonColor>();
        Color playerColor = Color.white;
        if (personColor != null)
            playerColor = personColor.colorPerson;
        else
            Debug.LogWarning("PersonColor не найден, цвет кнопки будет белым");

        // Получаем имя префаба
        string prefabName = playerRespawn.prefabName;
        if (string.IsNullOrEmpty(prefabName))
        {
            Debug.LogError($"Игрок {other.name}: prefabName не задан в PlayerRespawn!");
            return;
        }

        // Количество оставшихся патронов (если PlayerShoot отсутствует, по умолчанию 0)
        int remainingAmmo = playerShoot != null ? playerShoot.currentAmmo : 0;

        // Регистрируем кнопку с патронами
        if (BaseObjects.Instance != null)
            BaseObjects.Instance.RegisterPlayer(prefabName, playerColor, remainingAmmo);
        else
            Debug.LogError("BaseObjects.Instance не найден на сцене!");

        // Запускаем анимацию удаления
        if (playerShoot != null)
            playerShoot.EmptyMagazine();
        else
            Destroy(other.gameObject); // на всякий случай
    }
}