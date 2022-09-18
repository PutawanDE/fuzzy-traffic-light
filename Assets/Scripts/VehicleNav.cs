using UnityEngine;
using UnityEngine.AI;

public class VehicleNav : MonoBehaviour
{
    [SerializeField]
    private float maxRayDistance;
    [SerializeField]
    private float spaceBetweenVehicle;
    [SerializeField]
    private bool debugOn;

    private Transform stopMarker;
    private Transform destTransform;

    private NavMeshAgent navMeshAgent;

    private bool isInit;

    public void Init(int srcRoadNum, Transform stopMarker, Transform destTransform)
    {
        this.stopMarker = stopMarker;
        this.destTransform = destTransform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = destTransform.position;

        isInit = true;
    }

    private void FixedUpdate()
    {
        if (!isInit) return;

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (debugOn) Debug.DrawRay(ray.origin, ray.direction * maxRayDistance);

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            if (hit.collider.tag == "Vehicle")
            {
                if (debugOn) Debug.Log("Hit a vehicle. Distance: " + hit.distance);

                if (hit.distance <= spaceBetweenVehicle)
                {
                    navMeshAgent.isStopped = true;
                    return;
                }
            }
        }

        navMeshAgent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DestMarker")
        {
            Destroy(this.gameObject);
        }
    }

    public void StopAtIntersection()
    {
        navMeshAgent.destination = stopMarker.position;
    }

    public void GoAtIntersection()
    {
        navMeshAgent.destination = destTransform.position;
    }
}
