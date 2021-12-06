using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using GDG.ECS;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace GDG.AI
{
    public class GridBase<T>
    {
        public static int fontSize = 35;
        protected int width;
        protected int height;
        public T[,] gridArray;
        public TextMesh[,] gridTextArray;
        public float cellSize;
        public Vector3 localPosition;
        public List<Entity> textEntityList = new List<Entity>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width">网格的宽</param>
        /// <param name="height">网格的长度</param>
        /// <param name="cellSize">每一格的大小</param>
        /// <param name="defaultValue">每一格的默认值</param>
        /// <param name="localPosition">网格的本地位置</param>
        /// <param name="i_j_localPos_Callback">创建每一个格子时的回调，参数分别为：当前格子的值、第i行、第j列、网格所在的坐标</param>
        public GridBase(int width, int height, float cellSize,T defaultValue,Vector3 localPosition,UnityAction<T,int,int,Vector3> i_j_localPos_Callback)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.localPosition = localPosition;
            gridArray = new T[this.width, this.height];
            gridTextArray = new TextMesh[width, height];
        }
        public Vector3 GetWorldPositionXY(int x, int y) => new Vector3(x, y) * cellSize + localPosition;
        public Vector3 GetWorldPositionXZ(int x, int z, int y = 0) => new Vector3(x, y, z) * cellSize + localPosition;
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
# if EDITOR_DEBUG
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
# if EDITOR_DEBUG
            gridTextArray = new TextMesh[width, height];
            //textEntityList = new List<Entity>();
            textEntityList.Clear();
# endif
        }
        public static TextMesh CreateWorldText(out GameObject gameobject,string text,int fontSize = 40,Vector3 localPosition = default(Vector3),Transform parent = null,Action<Entity> callback = null,Color color = default(Color),TextAnchor textAnchor = TextAnchor.MiddleCenter,TextAlignment textAlignment = TextAlignment.Left)
        {
            ComponentTypes componentTypes = new ComponentTypes(typeof(GridComponent));

            var entity = World.EntityManager.CreateEntity<GameObjectComponent>((gameObjectComponent) =>
            {
                gameObjectComponent.gameObject = new GameObject("WorldText", typeof(TextMesh));
            });


            if(callback!=null)
            {
                callback(entity);
            }

            gameobject = entity.GetComponent<GameObjectComponent>().gameObject;

            Transform trans = gameobject.transform;
            trans.SetParent(parent, false);
            trans.localPosition = localPosition;

            TextMesh textMesh = gameobject.GetComponent<TextMesh>();
            textMesh.text = text;
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.fontSize = fontSize;
            if(color == null || color == default(Color))
                color = Color.white;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = 1000;
            return textMesh;
        }
    }

}