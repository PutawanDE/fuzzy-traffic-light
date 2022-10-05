using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Road
    {
        public Transform spawnMarker;
        public Transform destMarker;
        public Quaternion spawnQuaternion;
        [Range(0f, 1f)]
        public float spawnProb;
        public bool busSpawnable;
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

    private Queue<GameObject>[] spawnQueues = new Queue<GameObject>[numberOfRoads];
    private float[] cumulativeProb = new float[numberOfRoads];
    private IEnumerator spawnCourantine;

    private void Start()
    {
        trafficController = GetComponent<TrafficController>();
        InitCumulativeProb();
        InitSpawnQueueArray();
        spawnCourantine = WaitAndSpawn();
        StartCoroutine(spawnCourantine);
    }

    // If value in the inspecter change, then update
    private void OnValidate()
    {
        InitCumulativeProb();
    }

    private void InitCumulativeProb()
    {
        cumulativeProb[0] = roadsToSpawnOn[0].spawnProb;
        for (int i = 1; i < numberOfRoads; i++)
        {
            cumulativeProb[i] = cumulativeProb[i - 1] + roadsToSpawnOn[i].spawnProb;
        }
    }

    private void InitSpawnQueueArray()
    {
        for (int i = 0; i < numberOfRoads; i++)
        {
            spawnQueues[i] = new Queue<GameObject>();
        }
    }

    private void Update()
    {
        for (int i = 0; i < numberOfRoads; i++)
        {
            if (spawnQueues[i].Count > 0)
            {
                SpawnVehicleInQueues(i);
            }
        }
    }

    private IEnumerator WaitAndSpawn()
    {
        while (true)
        {
            float waitTime = Random.Range(spawnCooldownMin, spawnCooldownMax);
            yield return new WaitForSeconds(waitTime);
            RandSpawnVehicle();
        }
    }

    private void RandSpawnVehicle()
    {
        int selectedSrc = RouletteWheelSelectRoad();

        if (selectedSrc == -1) return;

        GameObject randVehicle =
            Random.Range(0f, 1f) >= carBusSpawnThreshold && roadsToSpawnOn[selectedSrc].busSpawnable
            ? bus : car;

        spawnQueues[selectedSrc].Enqueue(randVehicle);
    }

    private void SpawnVehicleInQueues(int selectedSrc)
    {
        if (!roadsToSpawnOn[selectedSrc].spawnMarker.GetComponent<CheckSpawnable>().isSpawnable)
            return;

        GameObject vehicleToSpawn = spawnQueues[selectedSrc].Dequeue();
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

    private int RouletteWheelSelectRoad()
    {
        float rand = Random.Range(0f, cumulativeProb[numberOfRoads - 1]);

        if (0f <= rand && rand <= cumulativeProb[0]) return 0;

        for (int i = 1; i < numberOfRoads; i++)
        {
            if (cumulativeProb[i - 1] <= rand && rand <= cumulativeProb[i]) return i;
        }

        return -1;
    }
}
