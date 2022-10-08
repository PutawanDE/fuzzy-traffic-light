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
    private float intersectionStopDistanceMin;
    [SerializeField]
    private float intersectionStopDistanceMax;
    [SerializeField]
    private bool debugOn;
    [SerializeField, EditorReadOnly]
    private int currentlyOnRoad;
    [SerializeField, EditorReadOnly]
    private Transform destTransform;

    [SerializeField, EditorReadOnly]
    private float totalWaitTime;
    private float currentWaitTime;

    private float timeAtLastStop;

    private NavMeshAgent navMeshAgent;
    private TrafficController trafficController;

    private bool isInit;
    private bool isPassed;
    private bool isSetTimeAtLastStop;

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
        if (!isInit || isPassed) return;

        RaycastHit hit;
        Vector3 raycastPos = new Vector3(
            transform.position.x,
            transform.position.y + raycastVerticalOffset,
            transform.position.z);
        Ray ray = new Ray(raycastPos, transform.forward);
        if (debugOn) Debug.DrawRay(ray.origin, ray.direction * maxRayDistance, Color.green);

        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {
            if (hit.collider.tag == "Car" || hit.collider.tag == "Bus")
            {
                if (debugOn) Debug.Log("Hit a vehicle. Distance: " + hit.distance);

                if (hit.distance <= spaceBetweenVehicle)
                {
                    navMeshAgent.isStopped = true;
                    MeasureWaitTime();
                    return;
                }
            }
            else if (hit.collider.tag == "Stop")
            {
                if (debugOn) Debug.Log("Hit a stop marker. Distance: " + hit.distance);

                if (intersectionStopDistanceMin <= hit.distance
                    && hit.distance <= intersectionStopDistanceMax)
                {
                    navMeshAgent.isStopped = true;
                    MeasureWaitTime();
                    return;
                }
            }
        }

        navMeshAgent.isStopped = false;
        if (isSetTimeAtLastStop)
        {
            totalWaitTime += currentWaitTime;
            currentWaitTime = 0;
        }
        isSetTimeAtLastStop = false;
    }

    private void MeasureWaitTime()
    {
        if (!isSetTimeAtLastStop)
        {
            timeAtLastStop = Time.time;
            isSetTimeAtLastStop = true;
        }

        currentWaitTime = Time.time - timeAtLastStop;
    }

    public float GetWaitTime()
    {
        return currentWaitTime += totalWaitTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DestMarker")
        {
            Destroy(this.gameObject);
        }
        else if (other.tag == "JunctionMarker")
        {
            isPassed = true;
            trafficController.DequeueVehicle(currentlyOnRoad);
        }
    }
}
