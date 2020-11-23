using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : Soldier
{
    public Vector2 moveTouchStartPosition;
    public Vector2 moveTouchCurrentPosition;
    public Vector2 fireTouchPosition;
    public bool isShootingInputActive;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        moveTouchStartPosition = Vector2.zero;
        moveTouchCurrentPosition = Vector2.zero;
        isShootingInputActive = false;
    }

    void CheckInput()
    {
        isShootingInputActive = false;

        if (Input.touchCount == 0)
        {
            moveTouchStartPosition = Vector2.zero;
            moveTouchCurrentPosition = Vector2.zero;
        }

        for (int i = 0; i < Input.touchCount; ++i)
        {
            if(i == 0)
            {
                if(Input.GetTouch(i).phase == TouchPhase.Began)
                {
                    moveTouchStartPosition = Input.GetTouch(i).position;
                    moveTouchCurrentPosition = moveTouchStartPosition;
                }
                else if (Input.GetTouch(i).phase == TouchPhase.Ended)
                {
                    moveTouchStartPosition = Vector2.zero;
                    moveTouchCurrentPosition = Vector2.zero;
                }
                else
                {
                    moveTouchCurrentPosition = Input.GetTouch(i).position;
                }
            }
            else
            {
                isShootingInputActive = true;
            }
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if(LevelManager.levelManagerInstance.HasGameEnded())
        {
            return;
        }

        CheckInput();

        if (currentState != SOLDIER_STATE.DYING && moveTouchStartPosition != moveTouchCurrentPosition)
        {
            movementDirection = moveTouchCurrentPosition - moveTouchStartPosition;
            movementDirection.Normalize();

            if (isShootingInputActive)
            {
                weapon.Fire();
                currentState = SOLDIER_STATE.SHOOTING;
                fireTouchPosition = Input.GetTouch(1).position;
            }
        }
        else
        {
            movementDirection = Vector2.zero;
        }

        switch (currentState)
        {
            case SOLDIER_STATE.IDLE:
                {
                    if(movementDirection != Vector2.zero)
                    {
                        currentState = SOLDIER_STATE.MOVING;
                    }

                    break;
                }
            case SOLDIER_STATE.MOVING:
                {
                    if (movementDirection == Vector2.zero)
                    {
                        currentState = SOLDIER_STATE.IDLE;
                    }

                    break;
                }
            case SOLDIER_STATE.SHOOTING:
                {
                    if(!isShootingInputActive)
                    {
                        currentState = SOLDIER_STATE.IDLE;
                    }

                    break;
                }
            case SOLDIER_STATE.DYING:
                {
                    //SceneManager.LoadScene("MainMenu");
                    break;
                }
            default:
                break;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        switch (currentState)
        {
            case SOLDIER_STATE.IDLE:
                {
                    break;
                }
            case SOLDIER_STATE.MOVING:
                {
                    if (rigidBody != null)
                        MoveToPosition(rigidBody.position + movementDirection * movementSpeed * Time.fixedDeltaTime);
                    break;
                }
            case SOLDIER_STATE.SHOOTING:
                {
                    if(rigidBody != null)
                        RotateTowards(rigidBody.position + movementDirection);
                    break;
                }
            case SOLDIER_STATE.DYING:
                {
                    break;
                }
            default:
                break;
        }
    }
}
