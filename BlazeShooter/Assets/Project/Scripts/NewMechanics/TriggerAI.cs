using UnityEngine;

public class TriggerAI : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerShoot>() != null)
        {
            other.gameObject.GetComponent<PlayerShoot>().IsActive = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if (other.gameObject.GetComponent<PlayerShoot>() != null)
        {
            other.gameObject.GetComponent<PlayerShoot>().IsActive = false;
        }
    }
}
