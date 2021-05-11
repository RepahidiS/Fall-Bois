using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seesaw : MonoBehaviour
{
    // we need 2 triggers. 1 left 1 right.
    // we need min & max appliable weight values foreach character. center->min edge->max
    // we need min & max rotate values for seesaw

    // when trigger enter or trigger stay statement we have to calculate our character how far away from center
    // then we have to calculate how many rotation value need to be apply to seesaw
    // then apply new rotation value to seesaw using linear interpolation

    // if there is 2 or more characters on seesaw we need to find which side is more heavy than other
    // then apply new rotation value to seesaw using linear interpolation

    // last thing : we need to apply some slide velocity to characters at this seesaw
    public SeesawSide leftSide;
    public SeesawSide rightSide;

    public float defaultRotate = 0.0f;
    public float leftMaxRotate = -30.0f;
    public float rightMaxRotate = 30.0f;
    public float rotatingSpeed = 0.1f;
    public float restoringRotationMultiplier = 5;

    private void FixedUpdate()
    {
        float currentRotation = defaultRotate;
        float currentSpeed = rotatingSpeed * restoringRotationMultiplier;
        if (leftSide.GetWeight() > rightSide.GetWeight())
        {
            currentRotation = leftMaxRotate;
            currentSpeed = rotatingSpeed * leftSide.GetWeight();
        }
        else if (leftSide.GetWeight() < rightSide.GetWeight())
        {
            currentRotation = rightMaxRotate;
            currentSpeed = rotatingSpeed * rightSide.GetWeight();
        }

        Vector3 rot = transform.rotation.eulerAngles;
        rot.z = currentRotation;
        Quaternion qRot = Quaternion.Slerp(transform.rotation, Quaternion.Euler(rot), Time.deltaTime * currentSpeed);
        transform.rotation = qRot;
    }
}