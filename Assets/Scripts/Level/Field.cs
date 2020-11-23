using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Field
    {
        public enum FieldType
        {
            Base,
            Floor,
            Obstacle,
            DestructibleWall,
            Invalid
        }

        public GameObject field;
        public FieldType fieldType;

        public float g;
        public float h;

        public Vector2Int position;
        public Field previousField;

        public Field()
        {
            field = null;

            g = -1;
            h = -1;

            position = Vector2Int.zero;
            previousField = null;
            fieldType = FieldType.Invalid;
        }

        public float GetF()
        {
            return g + h;
        }

        public Vector2 GetWorldCoordinates()
        {
            Vector2 position = Vector2.zero;

            if (field != null)
                position = new Vector2(field.transform.position.x, field.transform.position.y);

            return position;
        }
    }
}
