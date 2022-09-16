using UnityEngine;
using UnityEngine.AI;

public class VehicleNav : MonoBehaviour
{
    private Transform stopMarker;
    private Transform destTransform;

    private NavMeshAgent navMeshAgent;

    public void Init(int srcRoadNum, Transform stopMarker, Transform destTransform)
    {
        this.stopMarker = stopMarker;
        this.destTransform = destTransform;

        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.destination = destTransform.position;
    }

    public void StopAtIntersection()
    {
        navMeshAgent.destination = stopMarker.position;
    }

    public void GoAtIntersection()
    {
        navMeshAgent.destination = destTransform.position;
    }

    public void Delete()
    {
        Destroy(this);
    }
}
