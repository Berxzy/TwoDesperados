using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Soldier
{
    //tracking
    public static int ID;
    public int myID;

    public List<Field> pathToFollow;
    public Entity target;

    public float attackRange = 20;

    public LayerMask targetsLayerMask;
    public SpriteRenderer sprite;

    public bool isBuffed;

    public Ghost ghostToSpawn;
    public float chanceForGhost;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        isBuffed = false;

        RecalculatePath();

        myID = Enemy.ID++;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case SOLDIER_STATE.IDLE:
                {
                    if(pathToFollow.Count > 0)
                    {
                        currentState = SOLDIER_STATE.MOVING;
                    }

                    break;
                }
            case SOLDIER_STATE.MOVING:
                {
                    if (pathToFollow.Count == 0)
                    {
                        currentState = SOLDIER_STATE.IDLE;
                    }

                    break;
                }
            case SOLDIER_STATE.SHOOTING:
                {
                    if (target == null)
                        currentState = SOLDIER_STATE.MOVING;

                    break;
                }
            case SOLDIER_STATE.DYING:
                {
                    Destroy(gameObject, 3);
                    RecieveDamage(1);

                    break;
                }
            default:
                break;
        }

        // check if player is in range
        if(currentState != SOLDIER_STATE.DYING && !LevelManager.levelManagerInstance.HasGameEnded())
        {
            CheckForTargets();
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
                    if(pathToFollow != null && pathToFollow.Count > 0)
                    {
                        if (MoveToPosition(pathToFollow[0].position))
                        {
                            pathToFollow.RemoveAt(0);
                        }
                    }

                    break;
                }
            default:
                break;
        }
    }

    public virtual void RecalculatePath()
    {
        PlayerBase playerBase = LevelManager.levelManagerInstance.playerBaseGameObject;

        if(playerBase != null)
        {
            Vector2 basePosition = new Vector2(LevelManager.levelManagerInstance.playerBaseGameObject.transform.position.x, LevelManager.levelManagerInstance.playerBaseGameObject.transform.position.y);
            Vector2 myPosition = new Vector2(transform.position.x, transform.position.y);

            pathToFollow = LevelManager.levelManagerInstance.pathFinder.GetPath(myPosition, basePosition, this);
        }
        else
        {
            pathToFollow = new List<Field>();
        }
    }

    public virtual void CheckForTargets()
    {
        target = null;

        Player player = LevelManager.levelManagerInstance.playerGameObject;
        PlayerBase playerBase = LevelManager.levelManagerInstance.playerBaseGameObject;

        if (CheckTarget(player))
        {
            target = player;
        }
        else if (CheckTarget(playerBase))
        {
            target = playerBase;
        }
        else if (pathToFollow.Count > 0 && pathToFollow[0].fieldType == Field.FieldType.DestructibleWall && 
            (pathToFollow[0].field != null && CheckTarget(pathToFollow[0].field.GetComponent<Entity>())))
        {
            target = pathToFollow[0].field.GetComponent<Entity>();
        }

        if (target != null)
        {
            Vector2 targetPosition = new Vector2(target.transform.position.x, target.transform.position.y);

            RotateTowards(targetPosition);
            weapon.Fire();
            currentState = SOLDIER_STATE.SHOOTING;
        }
    }

    public bool CheckTarget(Entity target)
    {
        if (target != null && target.health > 0 && Vector3.Distance(transform.position, target.transform.position) <= attackRange)
        {
            Vector3 direction = target.transform.position - transform.position;

            Collider2D collider = GetComponent<Collider2D>();

            Vector3 startPoint = transform.position + (collider.transform.lossyScale.x / 2) * direction.normalized;

            int numberOfRayCasts = 3;
            bool onlyTargetHit = true;

            for(int i = -(numberOfRayCasts - 1); i < numberOfRayCasts - 2; i++)
            {
                Vector3 newStartPoint = RotatePointAroundPivot(startPoint, this.transform.position, new Vector3(0, 0, i * 20));

                RaycastHit2D hit = Physics2D.Raycast(newStartPoint, direction, direction.magnitude, targetsLayerMask);

                Debug.DrawLine(newStartPoint, target.transform.position);

                if (hit.collider == null || hit.collider.gameObject != target.gameObject)
                {
                    onlyTargetHit = false;
                    break;
                }
            }

            return onlyTargetHit;
        }

        return false;
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public override void Die()
    {
        base.Die();

        float chance = Random.Range(0, 100);

        if (chance <= chanceForGhost * 100)
        {
            Instantiate(ghostToSpawn, this.transform.position, Quaternion.identity, null);
        }

        LevelManager.levelManagerInstance.deadEnemiesCount++;
    }
}
