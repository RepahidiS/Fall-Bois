using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    None,
    Vertical,
    HorizontalX,
    HorizontalZ
}

public enum CurrentMove
{
    None,
    ToMin,
    ToMax
}

public class MovableObstacle : MonoBehaviour
{
    public MoveType moveType;
    public float minVal = 0.0f;
    public float maxVal = 0.0f;
    public float changeVal = 0.0f;

    public float cooldown = 0.0f;
    public float nextCooldown = 0.0f;
    public float completedTime = 0.0f;
    public CurrentMove currentMove = CurrentMove.None;

    void Start()
    {
        if (moveType == MoveType.None
        || minVal == 0.0f || maxVal == 0.0f || changeVal == 0.0f)
        {
            Debug.LogError(gameObject.name + "'s obstacle config is not set!");
            Debug.Break();
        }

        if(moveType == MoveType.Vertical)
        {
            if (transform.position.y == minVal)
                currentMove = CurrentMove.ToMax;
            else currentMove = CurrentMove.ToMin;
        }else if(moveType == MoveType.HorizontalX)
        {
            if (transform.position.x == minVal)
                currentMove = CurrentMove.ToMax;
            else currentMove = CurrentMove.ToMin;
        }else if(moveType == MoveType.HorizontalZ)
        {
            if (transform.position.y == minVal)
                currentMove = CurrentMove.ToMax;
            else currentMove = CurrentMove.ToMin;
        }
    }

    void FixedUpdate()
    {
        if (nextCooldown > Time.time)
            return;

        Vector3 pos = transform.position;
        if(moveType == MoveType.Vertical)
        {
            if(currentMove == CurrentMove.ToMax)
            {
                pos.y += changeVal;
                if(pos.y >= maxVal)
                {
                    pos.y = maxVal;
                    currentMove = CurrentMove.ToMin;
                    nextCooldown = Time.time + cooldown;
                }
            }else if(currentMove == CurrentMove.ToMin)
            {
                pos.y -= changeVal;
                if(pos.y <= minVal)
                {
                    pos.y = minVal;
                    currentMove = CurrentMove.ToMax;
                    nextCooldown = Time.time + cooldown;
                }
            }
        }else if(moveType == MoveType.HorizontalX)
        {
            if (currentMove == CurrentMove.ToMax)
            {
                pos.x += changeVal;
                if (pos.x >= maxVal)
                {
                    pos.x = maxVal;
                    currentMove = CurrentMove.ToMin;
                    nextCooldown = Time.time + cooldown;
                }
            }
            else if (currentMove == CurrentMove.ToMin)
            {
                pos.x -= changeVal;
                if (pos.x <= minVal)
                {
                    pos.x = minVal;
                    currentMove = CurrentMove.ToMax;
                    nextCooldown = Time.time + cooldown;
                }
            }
        }else if(moveType == MoveType.HorizontalZ)
        {
            if (currentMove == CurrentMove.ToMax)
            {
                pos.z += changeVal;
                if (pos.z >= maxVal)
                {
                    pos.z = maxVal;
                    currentMove = CurrentMove.ToMin;
                    nextCooldown = Time.time + cooldown;
                }
            }
            else if (currentMove == CurrentMove.ToMin)
            {
                pos.z -= changeVal;
                if (pos.z <= minVal)
                {
                    pos.z = minVal;
                    currentMove = CurrentMove.ToMax;
                    nextCooldown = Time.time + cooldown;
                }
            }
        }

        transform.position = pos;
    }
}