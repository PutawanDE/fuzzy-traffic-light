using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MeasureSim : MonoBehaviour
{
    [SerializeField]
    private float simulationSpeed = 1.0f;

    [ReadOnly, SerializeField] private float totalRunningTime;
    [ReadOnly, SerializeField] private int numberOfVehiclesPassed;
    [ReadOnly, SerializeField] private float vehiclesPerMin;
    [ReadOnly, SerializeField] private int numberOfBusesPassed;
    [ReadOnly, SerializeField] private float averageWaitTime;
    [ReadOnly, SerializeField] private int greenLightSwitchCount;

    private float totalWaitTime = 0f;

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
    }

    public void VehiclePassed(GameObject vehicle)
    {
        numberOfVehiclesPassed++;
        if (vehicle.tag == "Bus") numberOfBusesPassed++;
    }

    public void MeasureWaitTime(float waitTime)
    {
        greenLightSwitchCount++;
        totalWaitTime += waitTime;
        averageWaitTime = totalWaitTime / greenLightSwitchCount;
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
