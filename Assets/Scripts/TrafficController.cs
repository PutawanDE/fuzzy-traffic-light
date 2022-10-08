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

    [SerializeField, NonReorderable, EditorReadOnly]
    private VehicleQueue[] vehicleQueues = new VehicleQueue[numberOfRoads];

    [SerializeField, EditorReadOnly]
    private int[] currentRoadPriorites = new int[numberOfRoads];

    [SerializeField, EditorReadOnly]
    private float[] greenLightDuration = new float[numberOfRoads];

    [SerializeField]
    private float maxWaitTimeLimit;

    [SerializeField]
    private bool isFixedTime = false;

    [SerializeField]
    private float fixedGreenLightDuration;

    private TrafficLightFuzzyLogic fuzzyLogic;
    private MeasureSim measureSim;

    private bool isStart;

    public void EnqueueVehicle(GameObject v, int roadNum)
    {
        isStart = true;
        vehicleQueues[roadNum].list.Add(v);
        currentRoadPriorites[roadNum] += v.GetComponent<VehicleNav>().typePriority;
        UpdateGreenLightDuration(roadNum);
    }

    public void DequeueVehicle(int roadNum)
    {
        currentRoadPriorites[roadNum] -= vehicleQueues[roadNum].list[0]
            .GetComponent<VehicleNav>().typePriority;
        measureSim.VehiclePassed(vehicleQueues[roadNum].list[0]);

        vehicleQueues[roadNum].list.RemoveAt(0);
    }

    public int GetVehiclesCount(int roadNum)
    {
        return vehicleQueues[roadNum].list.Count;
    }

    public float GetWaitTimeFirstVehicle(int roadNum)
    {
        if (GetVehiclesCount(roadNum) > 0)
        {
            return vehicleQueues[roadNum].list[0].GetComponent<VehicleNav>().GetWaitTime();
        }
        else
        {
            return 0;
        }
    }

    private void Awake()
    {
        fuzzyLogic = GetComponent<TrafficLightFuzzyLogic>();
        measureSim = GetComponent<MeasureSim>();
    }

    private void Start()
    {
        StartCoroutine(ControlTrafficLights());
    }

    private void UpdateGreenLightDuration(int roadIdx)
    {
        greenLightDuration[roadIdx] = isFixedTime
                        ? fixedGreenLightDuration
                        : fuzzyLogic.Evaluate(GetVehiclesCount(roadIdx), currentRoadPriorites[roadIdx]);
    }

    private IEnumerator ControlTrafficLights()
    {
        int roadIdx = -1;
        while (true)
        {
            if (isStart)
            {
                if (isFixedTime)
                {
                    roadIdx++;
                    if (roadIdx >= 4) roadIdx = 0;
                }
                else
                {
                    roadIdx = FindNextGreenLightRoadIdx();
                }

                if (GetVehiclesCount(roadIdx) > 0)
                {
                    stopMarkers[roadIdx].SetActive(false);
                    stopOtherRoads(roadIdx);
                    measureSim.MeasureGreenLightTime(greenLightDuration[roadIdx]);
                    yield return new WaitForSeconds(greenLightDuration[roadIdx]);

                    stopMarkers[roadIdx].SetActive(true);
                    yield return new WaitForSeconds(3.0f);
                }
                else
                {
                    yield return new WaitForSeconds(3f);
                }
            }
            yield return null;
        }
    }

    private int FindNextGreenLightRoadIdx()
    {
        int maxPriority = 0;
        int maxPriorityRoadIdx = 0;
        for (int i = 0; i < numberOfRoads; i++)
        {
            if (currentRoadPriorites[i] > maxPriority)
            {
                maxPriority = currentRoadPriorites[i];
                maxPriorityRoadIdx = i;
            }
        }

        float maxWait = 0;
        int maxWaitRoadIdx = 0;
        for (int i = 0; i < numberOfRoads; i++)
        {
            if (GetWaitTimeFirstVehicle(i) > maxWait)
            {
                maxWait = GetWaitTimeFirstVehicle(i);
                maxWaitRoadIdx = i;
            }
        }

        int nextGreenLightIdx = 0;
        if (maxWait >= maxWaitTimeLimit)
        {
            nextGreenLightIdx = maxWaitRoadIdx;
        }
        else
        {
            nextGreenLightIdx = maxPriorityRoadIdx;
        }

        Debug.Log("Next Green Light: " + nextGreenLightIdx + " ,wait time: " +
                    GetWaitTimeFirstVehicle(nextGreenLightIdx));
        return nextGreenLightIdx;
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
}
