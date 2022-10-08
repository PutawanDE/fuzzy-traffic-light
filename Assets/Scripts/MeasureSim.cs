using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MeasureSim : MonoBehaviour
{
    [SerializeField] private float simulationSpeed = 1.0f;
    [SerializeField] private float endTime;

    [EditorReadOnly, SerializeField] private float totalRunningTime;
    [EditorReadOnly, SerializeField] public int longestQueue;
    [EditorReadOnly, SerializeField] public int numberOfIncomingVehicles;
    [EditorReadOnly, SerializeField] private int numberOfVehiclesPassed;
    [EditorReadOnly, SerializeField] private float vehiclesPerMin;
    [EditorReadOnly, SerializeField] private int numberOfBusesPassed;
    [EditorReadOnly, SerializeField] private float averageWaitTime;
    [EditorReadOnly, SerializeField] private float averageBusWaitTime;
    [EditorReadOnly, SerializeField] private float averageGreenLightTime;
    [EditorReadOnly, SerializeField] private int greenLightSwitchCount;

    private float totalWaitTime = 0f;
    private float totalBusWaitTime = 0f;
    private float totalGreenLightTime = 0f;

    private void Awake()
    {
        Time.timeScale = simulationSpeed;
        StartCoroutine(ComputeVehiclesPerMin());
    }

    private void OnValidate()
    {
        Time.timeScale = simulationSpeed;
    }

    private void Update()
    {
        totalRunningTime += Time.deltaTime;
        if (totalRunningTime >= endTime)
        {
            StopAllCoroutines();
            Time.timeScale = 0f;
        }
    }

    public void VehiclePassed(GameObject vehicle)
    {
        float waitTime = vehicle.GetComponent<VehicleNav>().GetWaitTime();

        numberOfVehiclesPassed++;
        if (vehicle.tag == "Bus")
        {
            numberOfBusesPassed++;
            totalBusWaitTime += waitTime;
            averageBusWaitTime = totalBusWaitTime / numberOfBusesPassed;
        }

        totalWaitTime += waitTime;
        averageWaitTime = totalWaitTime / numberOfVehiclesPassed;
    }

    public void MeasureGreenLightTime(float greenLightTime)
    {
        greenLightSwitchCount++;
        totalGreenLightTime += greenLightTime;
        averageGreenLightTime = totalGreenLightTime / greenLightSwitchCount;
    }

    private IEnumerator ComputeVehiclesPerMin()
    {
        while (true)
        {
            yield return new WaitForSeconds(60.0f);
            vehiclesPerMin = numberOfVehiclesPassed / (Time.time / 60.0f);
        }
    }
}
