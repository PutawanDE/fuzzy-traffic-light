using UnityEngine;

public class CheckSpawnable : MonoBehaviour
{
    public bool isSpawnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Car" || other.tag == "Bus") isSpawnable = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Car" || other.tag == "Bus") isSpawnable = true;
    }
}
