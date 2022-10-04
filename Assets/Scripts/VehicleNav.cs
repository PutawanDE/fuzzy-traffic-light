using UnityEngine;
using UnityEngine.AI;

public class VehicleNav : MonoBehaviour
{
    public int typePriority;

    [SerializeField]
    private float raycastVerticalOffset;
    [SerializeField]
    private float maxRayDistance;
    [SerializeField]
    private float spaceBetweenVehicle;
    [SerializeField]
    private float intersectionStopDistance;
    [SerializeField]
    private bool debugOn;
    [SerializeField, ReadOnly]
    private int currentlyOnRoad;
    [SerializeField, ReadOnly]
    private Transform destTransform;

    private NavMeshAgent navMeshAgent;
    private TrafficController trafficController;

    private bool isInit;

    public void Init(int srcRoadNum, Transform destTransform, TrafficController trafficController,
                        int roadNum)
    {
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
        Vector3 raycastPos = new Vector3(
            transform.position.x,
            transform.position.y + raycastVerticalOffset,
            transform.position.z);
        Ray ray = new Ray(raycastPos, transform.forward);
        if (debugOn) Debug.DrawRay(ray.origin, ray.direction * maxRayDistance);

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            if (hit.collider.tag == "Car" || hit.collider.tag == "Bus")
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
