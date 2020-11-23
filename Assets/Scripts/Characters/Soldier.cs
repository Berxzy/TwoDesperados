using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Entity
{
    public enum SOLDIER_STATE
    {
        IDLE,
        MOVING,
        SHOOTING,
        DYING
    }

    public SOLDIER_STATE currentState = SOLDIER_STATE.IDLE;

    public float movementSpeed = 5f;

    public Rigidbody2D rigidBody;

    public Vector2 movementDirection = Vector2.zero;
    public Vector2 lookDirection = Vector2.zero;

    public Animator animator;
    public Weapon weapon;

    // Start is called before the first frame update
    public virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        weapon = GetComponent<Weapon>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        switch (currentState)
        {
            case SOLDIER_STATE.IDLE:
            case SOLDIER_STATE.SHOOTING:
                {
                    if(animator != null)
                    {
                        animator.SetFloat("Speed", 0f);
                        animator.SetFloat("PlayerWalking", 0f);
                    }
                    break;
                }
            case SOLDIER_STATE.MOVING:
                {
                    if (animator != null)
                    {
                        animator.SetFloat("Speed", 1f);
                        animator.SetFloat("PlayerWalking", 1f);
                    }
                    break;
                }
            case SOLDIER_STATE.DYING:
                {
                    if (animator != null)
                    {
                        animator.SetInteger("Alive", 0);
                    }
                    break;
                }
            default:
                break;
        }
    }

    public virtual void FixedUpdate()
    {
        if(rigidBody != null)
        {
            rigidBody.velocity = Vector3.zero;
        }
    }

    public virtual bool MoveToPosition(Vector2 wantedPosition)
    {
        bool gotToPosition = false;

        Vector2 distanceVector = wantedPosition - rigidBody.position;

        if (distanceVector.magnitude < 0.1f)
        {
            rigidBody.MovePosition(wantedPosition);
            gotToPosition = true;
        }
        else
        {
            Vector2 newPosition = rigidBody.position + distanceVector.normalized * movementSpeed * Time.fixedDeltaTime;
            rigidBody.MovePosition(newPosition);
        }

        RotateTowards(wantedPosition);

        return gotToPosition;
    }

    public void RotateTowards(Vector2 target)
    {
        lookDirection = (target - rigidBody.position).normalized;

        //look in the direction you move
        Quaternion rotation = new Quaternion();
        rotation.eulerAngles = new Vector3(0, 0, (Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg));
        transform.rotation = rotation;
    }

    public override void Die()
    {
        base.Die();

        if(animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetFloat("PlayerWalking", 0f);
        }

        currentState = SOLDIER_STATE.DYING;

        Destroy(rigidBody);
        Destroy(GetComponent<Collider2D>());
    }
}
