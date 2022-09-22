using System.Collections;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Road
    {
        public Transform spawnMarker;
        public Transform stopMarker;
        public Transform destMarker;
        public Quaternion spawnQuaternion;
    }

    [System.Serializable]
    public class VehicleToSpawn
    {
        public GameObject vehicle;
    }

    private TrafficController trafficController;

    private const int numberOfRoads = TrafficController.numberOfRoads;

    [SerializeField]
    private float spawnCooldownMin, spawnCooldownMax;

    [SerializeField]
    private Road[] roadsToSpawnOn = new Road[numberOfRoads];

    [SerializeField]
    private VehicleToSpawn[] vehiclesToSpawn;

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

        if (trafficController.isFull(selectedSrc)) return;

        GameObject vehicleToSpawn = vehiclesToSpawn[
            (int)Mathf.Floor(Random.Range(0f, vehiclesToSpawn.Length))].vehicle;

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
        nav.Init(selectedSrc, roadToSpawnOn.stopMarker, destRoad.destMarker, trafficController, selectedSrc);
    }
}
