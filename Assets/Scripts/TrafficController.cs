using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
    public const int numberOfRoads = 4;

    [System.Serializable]
    public class VehicleQueue
    {
        public List<GameObject> list;
    }

    [SerializeField]
    private GameObject[] stopMarkers = new GameObject[numberOfRoads];

    [SerializeField]
    private VehicleQueue[] vehicleQueues = new VehicleQueue[numberOfRoads];

    [SerializeField]
    private int[] currentRoadPriorites = new int[numberOfRoads];

    [SerializeField]
    private float[] greenLightDuration = new float[numberOfRoads];

    [SerializeField]
    private int[] roadCapacity = new int[numberOfRoads];

    [SerializeField]
    private bool isFixedTime = false;

    [SerializeField]
    private float fixedGreenLightDuration;

    private TrafficLightFuzzyLogic fuzzyLogic;
    private bool isStart;

    public void EnqueueVehicle(GameObject v, int roadNum)
    {
        isStart = true;
        vehicleQueues[roadNum].list.Add(v);
        // currentRoadPriorites += 
        UpdateGreenLightDuration();
    }

    public void DequeueVehicle(int roadNum)
    {
        vehicleQueues[roadNum].list.RemoveAt(0);
    }

    public int GetVehiclesCount(int roadNum)
    {
        return vehicleQueues[roadNum].list.Count;
    }

    public bool isFull(int roadNum)
    {
        return GetVehiclesCount(roadNum) >= roadCapacity[roadNum];
    }

    private void Awake()
    {
        fuzzyLogic = GetComponent<TrafficLightFuzzyLogic>();
    }

    private void Start()
    {
        StartCoroutine(ControlTrafficLights());
    }

    private void UpdateGreenLightDuration()
    {
        for (int i = 0; i < numberOfRoads; i++)
        {
            greenLightDuration[i] = isFixedTime
                ? fixedGreenLightDuration
                : fuzzyLogic.Evaluate(GetVehiclesCount(i), currentRoadPriorites[i]);
        }
    }

    private IEnumerator ControlTrafficLights()
    {
        int roadIdx = -1;
        while (true)
        {
            if (isStart)
            {
                roadIdx = isFixedTime
                ? (roadIdx + 1) % numberOfRoads
                : FindMaxGreenLightRoadIdx();

                Debug.Log(roadIdx);
                if (GetVehiclesCount(roadIdx) != 0)
                {
                    stopMarkers[roadIdx].SetActive(false);
                    stopOtherRoads(roadIdx);
                    ToggleGreenLight();
                    yield return new WaitForSeconds(greenLightDuration[roadIdx]);

                    stopMarkers[roadIdx].SetActive(true);
                    ToggleYellowLight();
                    yield return new WaitForSeconds(3.0f);

                    ToggleRedLight();
                }
            }
            yield return null;
        }
    }

    private int FindMaxGreenLightRoadIdx()
    {
        float maxGreenLightDuration = 0f;
        int maxGreenLightRoadIdx = 0;
        for (int i = 0; i < numberOfRoads; i++)
        {
            if (greenLightDuration[i] > maxGreenLightDuration)
            {
                maxGreenLightDuration = greenLightDuration[i];
                maxGreenLightRoadIdx = i;
            }
        }
        return maxGreenLightRoadIdx;
    }

    private void stopOtherRoads(int goRoad)
    {
        for (int i = 0; i < numberOfRoads; i++)
        {
            if (i != goRoad)
            {
                stopMarkers[i].SetActive(true);
            }
        }
    }

    private void ToggleGreenLight()
    {

    }

    private void ToggleYellowLight()
    {

    }

    private void ToggleRedLight()
    {

    }
}
