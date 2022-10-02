using UnityEngine;
using UnityEngine.AI;

public class VehicleNav : MonoBehaviour
{
    [SerializeField]
    private float maxRayDistance;
    [SerializeField]
    private float spaceBetweenVehicle;
    [SerializeField]
    private float intersectionStopDistance;
    [SerializeField]
    private bool debugOn;
    [SerializeField]
    private int currentlyOnRoad;

    // private Transform stopMarker;
    private Transform destTransform;

    private NavMeshAgent navMeshAgent;
    private TrafficController trafficController;

    private bool isInit;

    public void Init(int srcRoadNum, Transform stopMarker, Transform destTransform,
        TrafficController trafficController, int roadNum)
    {
        // this.stopMarker = stopMarker;
        this.destTransform = destTransform;
        this.trafficController = trafficController;
        this.currentlyOnRoad = roadNum;

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
            if (hit.collider.tag == "Vehicle" || hit.collider.tag == "Stop")
            {
                if (debugOn) Debug.Log("Hit a vehicle. Distance: " + hit.distance);

                if (hit.distance <= spaceBetweenVehicle)
                {
                    navMeshAgent.isStopped = true;
                    return;
                }
            }
            else if (hit.collider.tag == "Stop")
            {
                if (debugOn) Debug.Log("Hit a stop marker. Distance: " + hit.distance);

                if (hit.distance <= intersectionStopDistance)
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
        else if (other.tag == "JunctionMarker")
        {
            trafficController.DequeueVehicle(currentlyOnRoad);
        }
    }
}
