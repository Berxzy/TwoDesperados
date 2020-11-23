using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class Pathfinder
{
    private int n;
    private int m;

    public Field[,] fields;

    public Pathfinder(Field[,] fields, int n, int m)
    {
        this.fields = fields;
        this.n = n;
        this.m = m;
    }

    public List<Field> GetPath(Vector2 startPos, Vector2 endPos, Soldier soldier = null)
    {
        return GetPath(WorldPositionToMatrixPosition(startPos), WorldPositionToMatrixPosition(endPos));
    }

    public List<Field> GetPath(Vector2Int startPos, Vector2Int endPos, Soldier soldier = null)
    {
        //reset
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < m; j++)
            {
                Field field = fields[i, j];

                field.g = m * n;

                field.h = Vector2Int.Distance(new Vector2Int(i, j), endPos);

                if (soldier != null)
                {
                    if(field.fieldType == Field.FieldType.DestructibleWall)
                    {
                        field.h += field.field.GetComponent<Entity>().health / (1 / soldier.weapon.fireRate) * soldier.weapon.ammunition.damage;
                    }
                    else
                    {
                        field.h += 1 / soldier.movementSpeed;
                    }                        
                }

                field.previousField = null;
            }
        }

        List<Field> openFields = new List<Field> { fields[startPos.x, startPos.y] };
        List<Field> closedFields = new List<Field>();

        Field currentField = openFields[0];
        currentField.g = 0;

        while (openFields.Count > 0)
        {
            int minIndex = 0;
            float minValue = openFields[0].GetF();

            for (int i = 1; i < openFields.Count; i++)
            {
                if (openFields[i].GetF() < minValue)
                {
                    minIndex = i;
                    minValue = openFields[i].GetF();
                }
            }

            currentField = openFields[minIndex];

            if (currentField.position == endPos)
                break;

            openFields.RemoveAt(minIndex);
            closedFields.Add(currentField);

            List<Field> neighbors = GetNeighbors(currentField.position);

            foreach (Field neighbor in neighbors)
            {
                if (closedFields.Contains(neighbor))
                    continue;

                if (neighbor.g > currentField.g + 1)
                {
                    neighbor.g = currentField.g + 1;
                    neighbor.previousField = currentField;
                }

                if (!openFields.Contains(neighbor))
                    openFields.Add(neighbor);
            }
        }

        List<Field> path = new List<Field>();

        Field endField = fields[endPos.x, endPos.y];
        while (endField != null)
        {
            endField = endField.previousField;

            if (endField != null)
            {
                path.Add(endField);
            }
        }

        path.Reverse();
        return path;
    }

    public List<Field> GetNeighbors(Vector2Int position)
    {
        List<Field> neighbours = new List<Field>();

        if (CheckField(new Vector2Int(position.x + 1, position.y)))
        {
            neighbours.Add(fields[position.x + 1, position.y]);
        }

        if (CheckField(new Vector2Int(position.x, position.y + 1)))
        {
            neighbours.Add(fields[position.x, position.y + 1]);
        }

        if (CheckField(new Vector2Int(position.x - 1, position.y)))
        {
            neighbours.Add(fields[position.x - 1, position.y]);
        }

        if (CheckField(new Vector2Int(position.x, position.y - 1)))
        {
            neighbours.Add(fields[position.x, position.y - 1]);
        }

        return neighbours;
    }
        
    bool CheckField(Vector2Int position)
    {
        bool isInRange = IsPositionInRange(position);

        return isInRange && (fields[position.x, position.y].fieldType == Field.FieldType.Floor || 
            fields[position.x, position.y].fieldType == Field.FieldType.Base ||
            fields[position.x, position.y].fieldType == Field.FieldType.DestructibleWall);
    }

    public bool IsPositionInRange(Vector2Int position)
    {
        return position.x >= 0 && position.x < n && position.y >= 0 && position.y < m;
    }

    public Vector2Int WorldPositionToMatrixPosition(Vector2 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    }
}
