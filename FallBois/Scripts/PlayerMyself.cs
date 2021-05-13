using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMyself : PlayerBase
{
    Vector2 touchStartPos;
    Vector2 touchCurrentPos;
    [SerializeField] Vector2 touchDiff;
    [SerializeField] Vector3 moveDirection;
    bool touchEnded = false;
    bool isMoving = false;
    public bool canMove = false;

    // just for debugging...
    public RawImage startImage;
    public RawImage currentImage;

    void Start()
    {
        rb.freezeRotation = true;
        rb.useGravity = false;

        if(rb == null)
        {
            Debug.LogError("MainCharacter rigidBody is null!");
            Debug.Break();
        }

        if (animator == null)
        {
            Debug.LogError("MainCharacter animator is null!");
            Debug.Break();
        }
    }

    public void ResetMainCharacter()
    {
        transform.position = new Vector3(0.0f, 2.1f, 14.0f);
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f));
        canMove = false;
        isFinished = false;
        touchStartPos = Vector2.zero;
        touchCurrentPos = Vector2.zero;
        touchDiff = Vector2.zero;
        moveDirection = Vector3.zero;
        characterAnimState = AnimState.Idle;
        rb.velocity = Vector3.zero;
        currentCheckpoint = checkpointManager.goSpawnPoint;
    }

    void Update()
    {
        // this feature is not working when we building the game...
        /*if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android
        || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            // is there any touch?
            if(Input.touchCount > 0)
            {
                // get first finger touch info
                Touch touch = Input.GetTouch(0);

                // touch just began
                if(touch.phase == TouchPhase.Began)
                {
                    touchEnded = false;
                    isMoving = true;
                    touchStartPos = touch.position;
                }

                // finger is moving to another position
                if(touch.phase == TouchPhase.Moved)
                {
                    touchEnded = false;
                    touchCurrentPos = touch.position;
                }

                // release this finger
                if(touch.phase == TouchPhase.Ended)
                {
                    touchEnded = true;
                    isMoving = false;
                    rb.velocity = Vector3.zero;
                }
            }
        }else if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
        {*/
            // left mouse button down
            if(Input.GetMouseButtonDown(0))
            {
                touchEnded = false;
                isMoving = true;
                touchStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            // dragging with left mouse button
            if(Input.GetMouseButton(0))
            {
                touchEnded = false;
                touchCurrentPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            // release left mouse button
            if(Input.GetMouseButtonUp(0))
            {
                touchEnded = true;
                isMoving = false;
                rb.velocity = Vector3.zero;
            }
        //}

        if(touchEnded)
        {
            touchStartPos = Vector2.zero;
            touchCurrentPos = Vector2.zero;
            touchDiff = Vector2.zero;
            moveDirection = Vector3.zero;
            characterAnimState = AnimState.Idle;
        }else if(isMoving)
        {
            touchDiff = touchCurrentPos - touchStartPos;
            // keep our touchDiff values between 100 & -100
            if (touchDiff.x != 0.0f)
                touchDiff.x = touchDiff.x > 0.0f ? Mathf.Min(touchDiff.x, 100.0f) : Mathf.Max(touchDiff.x, -100.0f);
            if (touchDiff.y != 0.0f)
                touchDiff.y = touchDiff.y > 0.0f ? Mathf.Min(touchDiff.y, 100.0f) : Mathf.Max(touchDiff.y, -100.0f);
            UpdateMoveDirection();

            // just for debugging...
            startImage.rectTransform.position = touchStartPos;
            currentImage.rectTransform.position = touchCurrentPos;
        }

        if (canMove && !isFinished)
            animator.SetInteger("AnimState", (int)characterAnimState);
        else animator.SetInteger("AnimState", (int)AnimState.Idle);
    }

    void FixedUpdate()
    {
        if (!canMove || isFinished)
            return;

        // rotate character
        if(moveDirection.x != 0 || moveDirection.z != 0)
        {
            Vector3 dir = moveDirection;
            dir.y = 0;

            Quaternion tr = Quaternion.LookRotation(dir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotatingSpeed);
            transform.rotation = targetRotation;
        }

        // if there is a hard force source then don't apply other movement forces
        if(hardForce != Vector3.zero)
            rb.velocity = hardForce;
        else
        {
            // apply movement
            rb.velocity = moveDirection * movementSpeed;

            if (bounceForce != 0.0f)
            {
                //Debug.Log("bounce force applying...");
                rb.velocity += bounceDir * bounceForce;

                // if we dont check maxVelocity then character will move faster towards to bounceDir when bouncing
                Vector3 tempVelocity = rb.velocity;
                // NOTE : this thing not working when character moving diagonal directions
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
        }

        // if character not grounded then apply gravity
        if (!IsGrounded())
        {
            if (IsAtMidAir())
                rb.velocity = new Vector3(0, -gravity * rb.mass, 0);
            else rb.velocity = new Vector3(rb.velocity.x, -gravity * rb.mass, rb.velocity.z);
        }
    }

    // in this method we have a 5 unit margin bacause we don't want to move
    // our character when we just touching to started position already.
    // and this statements "else if (characterAnimState != AnimState.Run)"
    // just fixing our walk->run->walk->run cycle when
    // x=1,z=0.1 or x=0.2,z=1.0 etc. situations
    void UpdateMoveDirection()
    {
        if (touchDiff.x > 5)
        {
            moveDirection.x = -(touchDiff.x / 100.0f);

            if (touchDiff.x > 30)
                characterAnimState = AnimState.Run;
            else characterAnimState = AnimState.Walk;
        }
        else if (touchDiff.x < -5)
        {
            moveDirection.x = touchDiff.x / -100.0f;

            if (touchDiff.x < -30)
                characterAnimState = AnimState.Run;
            else characterAnimState = AnimState.Walk;
        }
        else moveDirection.x = 0.0f;

        if (touchDiff.y > 5)
        {
            moveDirection.z = -(touchDiff.y / 100.0f);

            if (touchDiff.y > 30)
                characterAnimState = AnimState.Run;
            else if(characterAnimState != AnimState.Run)
                characterAnimState = AnimState.Walk;
        }
        else if (touchDiff.y < -5)
        {
            moveDirection.z = touchDiff.y / -100.0f;

            if (touchDiff.y < -30)
                characterAnimState = AnimState.Run;
            else if (characterAnimState != AnimState.Run)
                characterAnimState = AnimState.Walk;
        }
        else moveDirection.z = 0.0f;
    }
}