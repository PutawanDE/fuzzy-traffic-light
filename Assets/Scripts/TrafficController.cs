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

    [SerializeField, NonReorderable]
    private VehicleQueue[] vehicleQueues = new VehicleQueue[numberOfRoads];

    [SerializeField]
    private int[] currentRoadPriorites = new int[numberOfRoads];

    [SerializeField]
    private float[] greenLightDuration = new float[numberOfRoads];

    [SerializeField]
    private float maxWaitTime;

    [SerializeField]
    private bool isFixedTime = false;

    [SerializeField]
    private float fixedGreenLightDuration;

    private float[] waitTime = new float[numberOfRoads];
    private float[] lastGreenLightTime = new float[numberOfRoads];


    private TrafficLightFuzzyLogic fuzzyLogic;
    private bool isStart;

    public void EnqueueVehicle(GameObject v, int roadNum)
    {
        isStart = true;
        vehicleQueues[roadNum].list.Add(v);
        currentRoadPriorites[roadNum] += v.GetComponent<VehicleNav>().priority;
        UpdateGreenLightDuration(roadNum);
    }

    public void DequeueVehicle(int roadNum)
    {
        currentRoadPriorites[roadNum] -= vehicleQueues[roadNum].list[0]
            .GetComponent<VehicleNav>().priority;
        vehicleQueues[roadNum].list.RemoveAt(0);
    }

    public int GetVehiclesCount(int roadNum)
    {
        return vehicleQueues[roadNum].list.Count;
    }

    private void Awake()
    {
        fuzzyLogic = GetComponent<TrafficLightFuzzyLogic>();
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
                    ToggleGreenLight();
                    yield return new WaitForSeconds(greenLightDuration[roadIdx]);

                    stopMarkers[roadIdx].SetActive(true);
                    lastGreenLightTime[roadIdx] = Time.time;
                    ToggleYellowLight();
                    yield return new WaitForSeconds(3.0f);

                    ToggleRedLight();
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
        int nextGreenLightIdx = 0;
        int maxPriority = 0;
        for (int i = 0; i < numberOfRoads; i++)
        {
            waitTime[i] = Time.time - lastGreenLightTime[i];
            Debug.Log("Wait Time: " + i + "=" + waitTime[i]);

            if (currentRoadPriorites[i] > maxPriority)
            {
                maxPriority = currentRoadPriorites[i];
                nextGreenLightIdx = i;
            }
        }

        float maxWait = 0;
        int maxWaitRoadIdx = 0;
        for (int i = 0; i < numberOfRoads; i++)
        {
            if (waitTime[i] > maxWait)
            {
                maxWait = waitTime[i];
                maxWaitRoadIdx = i;
            }
        }

        if (maxWait >= maxWaitTime) nextGreenLightIdx = maxWaitRoadIdx;

        Debug.Log("Next Green Light: " + nextGreenLightIdx + " ,wait time: " + waitTime[nextGreenLightIdx]);
        waitTime[nextGreenLightIdx] = 0f;
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
