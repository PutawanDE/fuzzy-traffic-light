using UnityEngine;
using UnityEngine.AI;

public class CarNav : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;

    private NavMeshAgent navMeshAgent;

    private void Awake() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        navMeshAgent.destination = targetTransform.position;
    }
}
