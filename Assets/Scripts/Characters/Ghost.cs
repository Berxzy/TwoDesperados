using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Enemy
{
    public float idleTime = 3;

    Field lastVisitedField;
    Field currentField;

    List<Field> visitedFields;
    List<Field> currentPath;

    Pathfinder pathfinder;
    Enemy enemyToBuff;

    Color originalColor;
    public Color idleColor;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        pathfinder = LevelManager.levelManagerInstance.pathFinder;
        pathToFollow = new List<Field>();
        visitedFields = new List<Field>();
        currentPath = new List<Field>();

        Vector2Int currentPosition = pathfinder.WorldPositionToMatrixPosition(this.transform.position);
        currentField = pathfinder.fields[currentPosition.x, currentPosition.y];
        lastVisitedField = currentField;
        visitedFields.Add(currentField);

        sprite.transform.parent = null;

        originalColor = sprite.color;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (idleTime > 0)
        {
            idleTime -= Time.deltaTime;
            if (idleTime <= 0)
                sprite.color = originalColor;
            else
                sprite.color = idleColor;
        }
        else
        {
        }

        if (enemyToBuff != null && idleTime <= 0)
        {
            Vector3 distance = enemyToBuff.transform.position - transform.position;

            if(distance.magnitude < 0.05f)
            {
                enemyToBuff.weapon.fireRate /= 2;
                enemyToBuff.health *= 1.3f;
                enemyToBuff.sprite.transform.localScale *= 1.3f;
                enemyToBuff.sprite.color = Color.red;
                enemyToBuff.isBuffed = true;

                Destroy(sprite);
                Destroy(gameObject);
            }
        }
        else if (pathToFollow.Count == 0)
        {
            if(!visitedFields.Contains(lastVisitedField))
                visitedFields.Add(lastVisitedField);

            lastVisitedField = currentField;
            Vector2Int currentPosition = pathfinder.WorldPositionToMatrixPosition(this.transform.position);
            currentField = pathfinder.fields[currentPosition.x, currentPosition.y];

            RecalculatePath();

            if(pathToFollow.Count > 0)
            {
                Vector2 direction = pathToFollow[0].position - currentPosition;

                if (direction.x > 0)
                    sprite.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                else if (direction.x < 0)
                    sprite.transform.localScale = new Vector3(-0.1f, 0.1f, 0.1f);
            }
        }

        sprite.transform.position = this.transform.position;
    }

    public override void FixedUpdate()
    {
        if(enemyToBuff == null)
            base.FixedUpdate();
        else
        {
            Vector3 direction = (enemyToBuff.transform.position - transform.position).normalized;
            rigidBody.MovePosition(this.transform.position + direction * movementSpeed * Time.fixedDeltaTime);
        }
    }

    public override void RecalculatePath()
    {
        if(currentField != null)
        {
            pathToFollow = new List<Field>();

            List<Field> possiblePaths = LevelManager.levelManagerInstance.pathFinder.GetNeighbors(currentField.position);

            foreach (Field field in visitedFields)
                if (possiblePaths.Contains(field))
                    possiblePaths.Remove(field);

            if (possiblePaths.Count > 0)
            {
                int index = Random.Range(0, possiblePaths.Count);
                pathToFollow.Add(possiblePaths[index]);

                if (!currentPath.Contains(currentField))
                    currentPath.Add(currentField);
            }
            else if(currentPath.Count > 0)
            {
                pathToFollow.Add(currentPath[currentPath.Count - 1]);
                currentPath.RemoveAt(currentPath.Count - 1);
            }
            else
            {
                visitedFields.Clear();
                currentPath.Clear();
                RecalculatePath();
            }
        }
    }

    public override void CheckForTargets() { }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision != null && enemyToBuff == null && idleTime <= 0)
        {
            enemyToBuff = collision.gameObject.GetComponent<Enemy>();

            if(enemyToBuff is Ghost)
            {
                enemyToBuff = null;
            }

            if (enemyToBuff != null && enemyToBuff.isBuffed)
                enemyToBuff = null;
        }
    }

    public override void Die() { }
}
