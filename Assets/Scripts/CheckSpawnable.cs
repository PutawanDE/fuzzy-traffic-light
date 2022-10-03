using UnityEngine;

public class CheckSpawnable : MonoBehaviour
{
    public bool isSpawnable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle") isSpawnable = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Vehicle") isSpawnable = true;
    }
}
