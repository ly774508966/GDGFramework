using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using GDG.ECS;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.Utils
{
    public class Grid<T>
    {
        protected int width;
        protected int height;
        public T[,] gridArray;
        public TextMesh[,] gridTextArray;
        public float cellSize;
        public Vector3 localPosition;
        public List<Entity> textEntityList = new List<Entity>();

        public Grid(int width, int height, float cellSize,T defaultValue,Vector3 localPosition,UnityAction<T,int,int,Vector3> i_j_localPos_Callback)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.localPosition = localPosition;
            gridArray = new T[this.width, this.height];
#if DEBUG
            gridTextArray = new TextMesh[width, height];
#endif
        }
        protected Vector3 GetWorldPositionXY(int x, int y) => new Vector3(x, y) * cellSize + localPosition;
        protected Vector3 GetWorldPositionXZ(int x, int z, int y = 0) => new Vector3(x, y, z) * cellSize + localPosition;
        protected void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt((worldPosition - localPosition).x / cellSize);
            y = Mathf.FloorToInt((worldPosition - localPosition).y / cellSize);
        }
        protected void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition - localPosition).x / cellSize);
            z = Mathf.FloorToInt((worldPosition - localPosition).z / cellSize);
        }
        public void SetValue(int gridX, int gridY, T value)
        {
            if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height)
                return;
            gridArray[gridX, gridY] = value;
# if DEBUG
            gridTextArray[gridX, gridY].text = value.ToString();
# endif
        }
        public T GetValue(int gridX, int gridY)
        {
            if (gridX < 0 || gridX >= width || gridY < 0 || gridY >= height)
                return default(T); 

            return gridArray[gridX, gridY];
        }
        public void Clear()
        {
            gridArray = new T[width, height];
# if DEBUG
            gridTextArray = new TextMesh[width, height];
            //textEntityList = new List<Entity>();
            textEntityList.Clear();
# endif
        }
    }
}