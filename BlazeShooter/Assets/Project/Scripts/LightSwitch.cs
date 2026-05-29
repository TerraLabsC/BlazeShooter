using UnityEngine;

public class LightSwitch : MonoBehaviour
{
    [SerializeField] GameObject lightObject;

    [SerializeField] KeyCode key;

    [SerializeField] bool isOn = false;

    private void Start()
    {
        lightObject.SetActive(isOn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            isOn = !isOn;
            lightObject.SetActive(isOn);
        }
    }
}
