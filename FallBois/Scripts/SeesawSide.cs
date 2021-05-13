using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum Side
{
    Left,
    Right
}

public class WeightSource
{
    public Transform transform;
    public float distance;

    public WeightSource(Transform _transform, float _distance)
    {
        transform = _transform;
        distance = _distance;
    }

    public void UpdateDistance(float _distance)
    {
        distance = _distance;
    }

    public float GetDistance()
    {
        return distance;
    }
}

public class SeesawSide : MonoBehaviour
{
    public Side side;
    public float minValue = 0.1f;
    public float maxValue = 2.0f;
    public float centerPos = 0.82f;
    public float edgePos = 9.2f;
    public List<WeightSource> weightSources = new List<WeightSource>();
    public float totalWeight = 0.0f;

    private void OnTriggerEnter(Collider other)
    {
        float distance = GetDistanceFromCenter(other.transform.position);

        WeightSource result = GetWeightSource(other.transform);
        if(result == null)
        {
            WeightSource source = new WeightSource(other.transform, distance);
            weightSources.Add(source);
        }
        else result.UpdateDistance(distance);

        UpdateTotalWeight();
    }

    private void OnTriggerStay(Collider other)
    {
        float distance = GetDistanceFromCenter(other.transform.position);

        WeightSource result = GetWeightSource(other.transform);
        if (result == null)
        {
            WeightSource source = new WeightSource(other.transform, distance);
            weightSources.Add(source);
        }
        else result.UpdateDistance(distance);

        UpdateTotalWeight();
    }

    private void OnTriggerExit(Collider other)
    {
        WeightSource result = GetWeightSource(other.transform);
        if (result != null)
        {
            weightSources.Remove(result);
            UpdateTotalWeight();
        }
    }

    WeightSource GetWeightSource(Transform transform)
    {
        return weightSources.Where(i => i.transform == transform).FirstOrDefault();
    }

    float GetDistanceFromCenter(Vector3 pos)
    {
        if (side == Side.Left)
        {
            float distance = pos.x - transform.parent.transform.position.x;
            return distance < 0.0f ? distance * -1 : distance;
        }
        else if (side == Side.Right)
        {
            float distance = transform.parent.transform.position.x - pos.x;
            return distance < 0.0f ? distance * -1 : distance;
        }
        return 0.0f;
    }

    void UpdateTotalWeight()
    {
        totalWeight = 0.0f;
        foreach (WeightSource source in weightSources)
            totalWeight += source.distance - minValue / maxValue - minValue;
    }

    public float GetWeight()
    {
        //string strSide = side == Side.Left ? "left" : "right";
        //Debug.Log("side : " + strSide + " weight : " + totalWeight);
        return totalWeight;
    }
}