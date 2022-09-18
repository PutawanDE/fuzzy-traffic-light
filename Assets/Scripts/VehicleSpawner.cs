using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO:
    -Separate vehicle queue to another class
    -Delete vehicle from queue if pass intersection
    -Use list instead of queue because Unity can't serialize queue
    -If that road is full, do not spawn.
*/ 
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

    [System.Serializable]
    public class VehicleQueue
    {
        public Queue<GameObject> queue = new Queue<GameObject>();
    }

    private const int numberOfRoads = 4;

    [SerializeField]
    private float spawnCooldownMin, spawnCooldownMax;

    [SerializeField]
    private Road[] roadsToSpawnOn = new Road[numberOfRoads];

    [SerializeField]
    private VehicleToSpawn[] vehiclesToSpawn;

    [SerializeField]
    private VehicleQueue[] vehiclesQueues = new VehicleQueue[numberOfRoads];

    private IEnumerator spawnCourantine;

    void Start()
    {
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
        GameObject vehicleToSpawn = vehiclesToSpawn[
            (int)Mathf.Floor(Random.Range(0f, vehiclesToSpawn.Length))].vehicle;

        if (vehicleToSpawn == null || selectedSrc == -1) return;

        int selectedDest = (int)Mathf.Floor(Random.Range(0f, numberOfRoads));
        while (selectedSrc == selectedDest)
        {
            selectedDest = (int)Mathf.Floor(Random.Range(0f, numberOfRoads));
        }

        Road roadToSpawnOn = roadsToSpawnOn[selectedSrc];
        Road destRoad = roadsToSpawnOn[selectedDest];
        Vector3 spawnPos = roadToSpawnOn.spawnMarker.position;

        GameObject newVehicle = (GameObject)Instantiate(vehicleToSpawn, spawnPos, roadToSpawnOn.spawnQuaternion);
        vehiclesQueues[selectedSrc].queue.Enqueue(newVehicle);
        VehicleNav nav = newVehicle.GetComponent<VehicleNav>();
        nav.Init(selectedSrc, roadToSpawnOn.stopMarker, destRoad.destMarker);
    }
}
