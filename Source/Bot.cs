using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : PlayerBase
{
    public WaypointManager waypointManager;
    public bool canMove = false;

    Vector3 currentWaypoint;
    int currentWaypointIndex = 0;
    float minDistanceToNextWaypoint = 1.0f;

    void Start()
    {
        rb.freezeRotation = true;
        rb.useGravity = false;

        if (rb == null)
        {
            Debug.LogError("Bot " + name + "'s rigidBody is null!");
            Debug.Break();
        }

        if (animator == null)
        {
            Debug.LogError("Bot " + name + "'s animator is null!");
            Debug.Break();
        }

        // testing...
        animator.SetInteger("AnimState", (int)AnimState.Idle);
        currentCheckpoint = checkpointManager.goSpawnPoint;

        GetFirstWaypoint();
    }

    public void UpdateCanMove(bool _canMove)
    {
        canMove = _canMove;
        animator.SetInteger("AnimState", canMove ? (int)AnimState.Run : (int)AnimState.Idle);
    }

    void FixedUpdate()
    {
        if (!canMove || isFinished)
        {
            animator.SetInteger("AnimState", (int)AnimState.Idle);
            return;
        }

        Debug.DrawRay(transform.position, currentWaypoint - transform.position, Color.red);

        Quaternion tr = Quaternion.LookRotation(currentWaypoint - transform.position);
        Vector3 eulerRotation = tr.eulerAngles;
        eulerRotation.x = 0.0f;
        eulerRotation.z = 0.0f;
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(eulerRotation), Time.deltaTime * rotatingSpeed);
        transform.rotation = targetRotation;

        // if there is a hard force source then don't apply other movement forces
        if (hardForce != Vector3.zero)
            rb.velocity = hardForce;
        else if (currentWaypoint != Vector3.zero)
        {
            // apply movement
            rb.velocity = transform.forward * movementSpeed;

            if (bounceForce != 0.0f)
            {
                //Debug.Log("bounce force applying...");
                rb.velocity += bounceDir * bounceForce;

                // if we dont check maxVelocity then character will move faster towards to bounceDir when bouncing
                Vector3 tempVelocity = rb.velocity;

                if (tempVelocity.x > 0 && tempVelocity.x > movementSpeed)
                    tempVelocity.x = movementSpeed;
                else if (tempVelocity.x < 0 && tempVelocity.x < -movementSpeed)
                    tempVelocity.x = -movementSpeed;

                if (tempVelocity.y > 0 && tempVelocity.y > movementSpeed)
                    tempVelocity.y = movementSpeed;
                else if (tempVelocity.x < 0 && tempVelocity.y < -movementSpeed)
                    tempVelocity.y = -movementSpeed;

                if (tempVelocity.z > 0 && tempVelocity.z > movementSpeed)
                    tempVelocity.z = movementSpeed;
                else if (tempVelocity.x < 0 && tempVelocity.z < -movementSpeed)
                    tempVelocity.z = -movementSpeed;

                rb.velocity = tempVelocity;
            }

            // apply barrel force
            if (barrelForce != Vector3.zero)
                rb.velocity += barrelForce;
        }else UpdateCanMove(false);

        // if character not grounded then apply gravity
        if (!IsGrounded())
        {
            if(IsAtMidAir())
                rb.velocity = new Vector3(0, -gravity * rb.mass, 0);
            else rb.velocity = new Vector3(rb.velocity.x, -gravity * rb.mass, rb.velocity.z);
        }

        Vector3 tempPos = transform.position;
        tempPos.y = currentWaypoint.y;
        if (Vector3.Distance(tempPos, currentWaypoint) <= minDistanceToNextWaypoint)
            GetNextWaypoint();
    }

    public void GetFirstWaypoint()
    {
        currentWaypoint = waypointManager.GetFirstWaypoint(transform.position);
        currentWaypointIndex = 1;
    }

    void GetNextWaypoint()
    {
        currentWaypoint = waypointManager.GetClosestWaypoint(transform.position, currentWaypointIndex);
        currentWaypointIndex++;
    }
}