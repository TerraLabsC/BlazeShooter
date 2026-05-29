using UnityEngine;

public class Ball : MonoBehaviour
{
    public ColorType assignedColor;
    public float speed = 20f;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    private void Update()
    {
        float step = speed * Time.deltaTime;
        Vector3 direction = transform.forward;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, step))
        {
            // Получаем компонент цвета блока для проверки соответствия
            BlockColor blockColor = hit.collider.GetComponent<BlockColor>();

            if (blockColor != null && blockColor.colorType == assignedColor)
            {
                // Ищем компонент анимации уничтожения на блоке
                BlockDestroyAnimation destroyAnim = hit.collider.GetComponent<BlockDestroyAnimation>();
                if (destroyAnim != null)
                {
                    destroyAnim.PlayDestroyAnimation();

                    hit.collider.GetComponent<BoxCollider>().enabled = false;
                }
                else
                {
                    // Если компонента нет – просто удаляем блок сразу (запасной вариант)
                    Destroy(hit.collider.gameObject);
                }
            }

            // Шарик в любом случае уничтожается при столкновении
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(direction * step, Space.World);
        }
    }
}