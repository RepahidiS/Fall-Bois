using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimState
{
    Idle = 0,
    Walk,
    Run
}

public class PlayerBase : MonoBehaviour
{
    public Rigidbody rb;
    public Animator animator;
    public float movementSpeed = 10.0f;
    public float rotatingSpeed = 15.0f;
    public float gravity = 5.0f;
    public bool isFinished = false;

    public float bounceForce = 0.0f;
    public Vector3 bounceDir = Vector3.zero;
    public Vector3 barrelForce = Vector3.zero;
    public Vector3 hardForce = Vector3.zero;

    public AnimState characterAnimState = AnimState.Idle;

    // checkpoint things
    public CheckpointManager checkpointManager;
    public GameObject currentCheckpoint;

    public bool IsGrounded()
    {
        // using QueryTriggerInteraction.Ignore because we dont want to raycast hit to triggers like deathzone
        Debug.DrawRay(transform.position, Vector3.down * 0.1f, Color.red);
        return Physics.Raycast(transform.position, Vector3.down, 0.1f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }

    public bool IsAtMidAir()
    {
        return !Physics.Raycast(transform.position, Vector3.down, 5.0f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            // if collider is further away from the previous one
            if (currentCheckpoint.transform.position.z > other.transform.position.z)
                currentCheckpoint = other.gameObject;
        }
        else if (other.tag == "DeathZone")
        {
            // TODO : maybe punish player? or track how many times player dead?
            transform.position = checkpointManager.GetSpawnablePos(currentCheckpoint);
            bounceDir = Vector3.zero;
            bounceForce = 0.0f;
            hardForce = Vector3.zero;
        }else if(other.tag == "Finish")
            isFinished = true;
    }

    public void SetHardForce(Vector3 force)
    {
        hardForce = force;
    }

    public void SetBarrelForce(Vector3 force)
    {
        barrelForce = force;
    }

    public void HitPlayer(Vector3 velocityF, float time)
    {
        velocityF.y = 0.0f; // just keep our character on ground

        bounceForce = velocityF.magnitude;
        bounceDir = Vector3.Normalize(velocityF);
        StartCoroutine(Decrease(velocityF.magnitude, time));
    }

    private IEnumerator Decrease(float value, float duration)
    {
        float delta = value / duration;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            yield return null;

            bounceForce -= Time.deltaTime * delta;
            bounceForce = bounceForce < 0 ? 0 : bounceForce;
        }
    }
}