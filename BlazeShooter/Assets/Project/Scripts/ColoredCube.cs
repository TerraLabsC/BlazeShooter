using UnityEngine;

public class ColoredCube : MonoBehaviour
{
    [SerializeField] private Color cubeColor = Color.white;

    // —сылка на пулю, котора€ уже выпущена по этому кубику
    public GameObject AssignedBullet { get; set; }

    public Color CubeColor => cubeColor;

    /// <summary>
    /// ¬ызываетс€ из Bullet, когда пул€ уничтожаетс€ (не долетела или исчезла)
    /// </summary>
    public void ClearAssignedBullet(GameObject bullet)
    {
        if (AssignedBullet == bullet)
            AssignedBullet = null;
    }
}