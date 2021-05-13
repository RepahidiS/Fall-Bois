using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BounceType
{
    EveryDirections,
    WindRose,
    Barrel
}

public class Bounce : MonoBehaviour
{
    public BounceType type;
    public float bounceVal = 10.0f;

    private void OnCollisionEnter(Collision collision)
    {
        if(type == BounceType.EveryDirections)
        {
            Debug.DrawRay(collision.contacts[0].point, -collision.contacts[0].normal * 2, Color.red, 5.0f);
            collision.gameObject.GetComponent<PlayerBase>().HitPlayer(-collision.contacts[0].normal * bounceVal, 2);
        }else if(type == BounceType.WindRose)
            collision.gameObject.GetComponent<PlayerBase>().SetHardForce(Vector3.left * bounceVal);
    }
}