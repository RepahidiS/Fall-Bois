using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI_NotWorking : MonoBehaviour
{
    public float frontRayDistance = 5.0f;
    public float sideRayDistance = 7.0f;
    public int rayCount = 20;
    public float rayAngle = -90.0f;

    public float movementSpeed = 0.1f;
    public float rotationSpeed = 0.1f;

    public bool rotatingToClearestWay = false;
    public float rotatingToClearestWayModifier = 0.0f;

    void Start()
    {
    }

    // we need to use rays to run our bots.
    // I'll use 2 group of rays. 1-) front rays 2-) back rays
    // this rays are seperating 3 groups 1-) left 2-) front 3-) right
    // we need to check front rays first
    // if front rays front(1.2) is clear then ai need to run towards
    // otherwise need to check left(1.1) & right(1.2) side rays
    // if left or right rays clear then ai need to rotate to theese angles and run towards
    // if we dont have any clear rays in front rays we need to cast back rays
    // if back rays are casting then check same steps and move backwards
    // our priority need to be front rays every update
    // TODO : right now our character acting like a pin-ball I need to fix that ^^
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        Quaternion rot = transform.rotation;

        // front front rays
        if (IsFrontClear())
        {
            // move towards
            pos += transform.forward * movementSpeed * Time.deltaTime;
            rotatingToClearestWay = false;
            rotatingToClearestWayModifier = 0.0f;
        }
        else if (!rotatingToClearestWay)
        {
            int blockedLeftCount = GetBlockedLeftCount();
            int blockedRightCount = GetBlockedRightCount();

            // right is our clearest way
            if (blockedLeftCount > blockedRightCount)
            {
                Vector3 vRot = rot.eulerAngles;
                vRot.y -= rotationSpeed;
                rot = Quaternion.Euler(vRot);
                rotatingToClearestWay = true;
                rotatingToClearestWayModifier = -1.0f;
            }
            else // left is our clearest way or each sides equally clear
            {
                Vector3 vRot = rot.eulerAngles;
                vRot.y += rotationSpeed;
                rot = Quaternion.Euler(vRot);
                rotatingToClearestWay = true;
                rotatingToClearestWayModifier = 1.0f;
            }
        } else
        {
            Vector3 vRot = rot.eulerAngles;
            vRot.y += rotationSpeed * rotatingToClearestWayModifier;
            rot = Quaternion.Euler(vRot);
        }

        transform.position = pos;
        transform.rotation = rot;
    }

    bool IsFrontClear()
    {
        for (int i = 0; i < 3; i++)
        {
            Quaternion rot = transform.rotation;
            Quaternion mod = Quaternion.AngleAxis((i + 7) / ((float)rayCount - 1) * rayAngle * 2 - rayAngle, transform.up);
            Vector3 dir = rot * mod * new Vector3(0, 0, frontRayDistance);
            RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, frontRayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position, dir, Color.red);

            if (hits.Length > 0)
                return false;
        }

        return true;
    }

    int GetBlockedLeftCount()
    {
        int blockedCount = 0;
        for (int i = 0; i < 7; i++)
        {
            Quaternion rot = transform.rotation;
            Quaternion mod = Quaternion.AngleAxis(i / ((float)rayCount - 1) * rayAngle * 2 - rayAngle, transform.up);
            Vector3 dir = rot * mod * new Vector3(0, 0, sideRayDistance);
            RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, sideRayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position, dir, new Color32(255, 155, 0, 255));

            if (hits.Length > 0)
                blockedCount++;
        }

        return blockedCount;
    }

    int GetBlockedRightCount()
    {
        int blockedCount = 0;
        for (int i = 0; i < 7; i++)
        {
            Quaternion rot = transform.rotation;
            Quaternion mod = Quaternion.AngleAxis((i + 10) / ((float)rayCount - 1) * rayAngle * 2 - rayAngle, transform.up);
            Vector3 dir = rot * mod * new Vector3(0, 0, sideRayDistance);
            RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, sideRayDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(transform.position, dir, new Color32(255, 155, 0, 255));

            if (hits.Length > 0)
                blockedCount++;
        }

        return blockedCount;
    }
}