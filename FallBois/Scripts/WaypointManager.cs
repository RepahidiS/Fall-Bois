using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Waypoint
{
    public Vector3 center;
    public float min;
    public float max;

    public Waypoint(Vector3 _center, float _min, float _max)
    {
        center = _center;
        min = _min;
        max = _max;
    }
}

[System.Serializable]
public class Sector
{
    public int id;
    public List<Waypoint> waypoints = new List<Waypoint>();
}

public class WaypointManager : MonoBehaviour
{
    public List<Sector> sectors = new List<Sector>();

    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++) // sectors
        {
            Sector newSector = new Sector();
            newSector.id = i;

            Transform currentSector = transform.GetChild(i);
            for(int j = 0; j < currentSector.childCount; j++) // waypoints
            {
                Transform currentWaypoint = currentSector.GetChild(j);
                Transform tMax = currentWaypoint.GetChild(0);
                Transform tMin = currentWaypoint.GetChild(1);

                newSector.waypoints.Add(new Waypoint(currentWaypoint.position, tMin.position.x, tMax.position.x));
            }

            sectors.Add(newSector);
        }
    }

    public Vector3 GetFirstWaypoint(Vector3 pos)
    {
        Sector currentSector = sectors.Where(i => i.id == 0).FirstOrDefault();
        if (currentSector != null)
        {
            float distance = float.MaxValue;
            Waypoint closestWaypoint = null;
            foreach (Waypoint waypoint in currentSector.waypoints)
            {
                float currentDistance = Vector3.Distance(pos, waypoint.center);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closestWaypoint = waypoint;
                }
            }

            if (closestWaypoint != null)
                return new Vector3(Random.Range(closestWaypoint.min, closestWaypoint.max), closestWaypoint.center.y, closestWaypoint.center.z);
        }

        return Vector3.zero;
    }

    // get next sector waypoints
    // calculate and find closest waypoint
    // get random x coordinate and return it
    public Vector3 GetClosestWaypoint(Vector3 pos, int sectorId)
    {
        Sector currentSector = sectors.Where(i => i.id == sectorId).FirstOrDefault();
        if(currentSector != null)
        {
            float distance = float.MaxValue;
            Waypoint closestWaypoint = null;
            foreach(Waypoint waypoint in currentSector.waypoints)
            {
                float currentDistance = Vector3.Distance(pos, waypoint.center);
                if (currentDistance < distance)
                {
                    distance = currentDistance;
                    closestWaypoint = waypoint;
                }
            }

            if (closestWaypoint != null)
                return new Vector3(Random.Range(closestWaypoint.min, closestWaypoint.max), closestWaypoint.center.y, closestWaypoint.center.z);
        }

        return Vector3.zero;
    }
}