using UnityEngine;

public class PersonColor : MonoBehaviour
{
    [SerializeField] private GameObject person;
    public Color colorPerson;

    void Start()
    {
        Renderer rend = person.GetComponent<Renderer>();
        if (rend != null)
        {
            colorPerson = rend.material.GetColor("_Color");
        }
    }
}