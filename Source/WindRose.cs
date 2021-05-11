using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindRose : MonoBehaviour
{
    public float rotateSpeed = 1.0f;

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        transform.Rotate(Vector3.up, rotateSpeed);
    }
}