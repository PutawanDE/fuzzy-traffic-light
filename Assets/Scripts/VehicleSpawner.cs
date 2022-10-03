using System.Collections;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Road
    {
        public Transform spawnMarker;
        public Transform destMarker;
        public Quaternion spawnQuaternion;
    }

    private TrafficController trafficController;

    private const int numberOfRoads = TrafficController.numberOfRoads;

    [SerializeField]
    private float spawnCooldownMin, spawnCooldownMax;

    [SerializeField, NonReorderable]
    private Road[] roadsToSpawnOn = new Road[numberOfRoads];

    [SerializeField]
    private GameObject car;
    [SerializeField]
    private GameObject bus;
    [SerializeField, Range(0f, 1f)]
    private float carBusSpawnThreshold;

    private IEnumerator spawnCourantine;

    void Start()
    {
        trafficController = GetComponent<TrafficController>();
        spawnCourantine = WaitAndSpawn();
        StartCoroutine(spawnCourantine);
    }

    private IEnumerator WaitAndSpawn()
    {
        while (true)
        {
            float waitTime = Random.Range(spawnCooldownMin, spawnCooldownMax);
            yield return new WaitForSeconds(waitTime);
            SpawnVehicle();
        }
    }

    private void SpawnVehicle()
    {
        int selectedSrc = (int)Mathf.Floor(Random.Range(0f, numberOfRoads));

        if (!roadsToSpawnOn[selectedSrc].spawnMarker.GetComponent<CheckSpawnable>().isSpawnable) return;

        GameObject vehicleToSpawn = Random.Range(0f, 1f) >= carBusSpawnThreshold ? bus : car;

        if (vehicleToSpawn == null) return;

        int selectedDest = (int)Mathf.Floor(Random.Range(0f, numberOfRoads));
        while (selectedSrc == selectedDest)
        {
            selectedDest = (int)Mathf.Floor(Random.Range(0f, numberOfRoads));
        }

        Road roadToSpawnOn = roadsToSpawnOn[selectedSrc];
        Road destRoad = roadsToSpawnOn[selectedDest];
        Vector3 spawnPos = roadToSpawnOn.spawnMarker.position;

        GameObject newVehicle = (GameObject)Instantiate(vehicleToSpawn, spawnPos, roadToSpawnOn.spawnQuaternion);
        trafficController.EnqueueVehicle(newVehicle, selectedSrc);
        VehicleNav nav = newVehicle.GetComponent<VehicleNav>();
        nav.Init(selectedSrc, destRoad.destMarker, trafficController, selectedSrc);
    }
}
