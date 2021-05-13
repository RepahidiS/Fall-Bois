using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ForceDirection
{
    Left,
    Right
}

public class Barrel : MonoBehaviour
{
    public ForceDirection forceDirection;
    public float rotateSpeed = 1.0f;
    public float forceSpeed = 10.0f;
    public Transform child;

    private void Start()
    {
        child = transform.GetChild(0);
        if(child == null)
        {
            Debug.LogError("Barrel child not found!");
            Debug.Break();
        }
    }

    void FixedUpdate()
    {
        child.Rotate(Vector3.right, rotateSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(forceDirection == ForceDirection.Left)
            other.gameObject.GetComponent<PlayerBase>().SetBarrelForce(Vector3.right * forceSpeed); // Vector3.right for left direction because "blender" ^^
        else if(forceDirection == ForceDirection.Right)
            other.gameObject.GetComponent<PlayerBase>().SetBarrelForce(Vector3.left * forceSpeed);
    }

    private void OnTriggerStay(Collider other)
    {
        if (forceDirection == ForceDirection.Left)
            other.gameObject.GetComponent<PlayerBase>().SetBarrelForce(Vector3.right * forceSpeed); // Vector3.right for left direction because "blender" ^^
        else if (forceDirection == ForceDirection.Right)
            other.gameObject.GetComponent<PlayerBase>().SetBarrelForce(Vector3.left * forceSpeed);
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<PlayerBase>().SetBarrelForce(Vector3.zero);
    }
}