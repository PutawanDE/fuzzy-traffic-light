using System.Collections.Generic;
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
    private VehicleQueue[] vehicleQueues = new VehicleQueue[numberOfRoads];

    [SerializeField]
    private int[] roadCapacity = new int[numberOfRoads];

    public void EnqueueVehicle(GameObject v, int roadNum)
    {
        vehicleQueues[roadNum].list.Add(v);
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
}
