using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Настройки пули")]
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 3f;
    [SerializeField] private GameObject impactEffect;

    [Header("Цвет пули")]
    [SerializeField] private Color bulletColor = Color.white;
    [SerializeField] private float colorTolerance = 0.1f;

    private Rigidbody rb;
    private ColoredCube targetCube; // запоминаем, к какому кубику привязана пуля

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    public void SetColor(Color newColor)
    {
        bulletColor = newColor;
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = newColor;
    }

    public void AssignTarget(ColoredCube cube)
    {
        targetCube = cube;
        targetCube.AssignedBullet = gameObject;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = rb.position + transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    private void OnTriggerEnter(Collider other)
    {
        ColoredCube cube = other.GetComponent<ColoredCube>();

        if (cube != null)
        {
            if (ColorsAreClose(cube.CubeColor, bulletColor, colorTolerance))
            {
                Destroy(other.gameObject);
                if (impactEffect != null)
                    Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Когда пуля уничтожается, очищаем ссылку в кубике
        if (targetCube != null)
        {
            targetCube.ClearAssignedBullet(gameObject);
        }
    }

    private bool ColorsAreClose(Color a, Color b, float tolerance)
    {
        return Mathf.Abs(a.r - b.r) < tolerance &&
               Mathf.Abs(a.g - b.g) < tolerance &&
               Mathf.Abs(a.b - b.b) < tolerance;
    }
}