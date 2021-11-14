using System;
using System.Collections;
using System.Collections.Generic;
using GDG.ECS;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace GDG.AI
{
    public class Grid2D<T> : GridBase<T>
    {
        public Grid2D(int width, int height, float cellSize, T defaultValue = default(T), Vector3 localPosition = default(Vector3), UnityAction<T,int,int,Vector3> i_j_localPos_Callback = null) : base(width, height, cellSize, defaultValue, localPosition,i_j_localPos_Callback)
        {
# if EDITOR_DEBUG
            var grid = World.EntityManager.CreateEntity<GameObjectComponent>((gameObjectComponent) =>
             {
                 gameObjectComponent.gameObject = new GameObject("Grid");
             });
            var gameObject = grid.GetComponent<GameObjectComponent>().gameObject;
            gameObject.transform.SetParent(ECS.World.monoWorld.transform);
#endif
            bool isClass = typeof(T).IsClass;
            for (int i = 0; i < gridArray.GetLength(0); i++)
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    if(isClass)
                        gridArray[i, j] = ObjectPool<T>.Instance.Pop();
                    else
                        gridArray[i, j] = defaultValue;
# if EDITOR_DEBUG
                    gridTextArray[i, j] = GDGTools.CreateWorldText(
                        out GameObject obj, 
                        gridArray[i, j].ToString(), 
                        fontSize, 
                        GetWorldPositionXY(i, j) + new Vector3(cellSize, cellSize, 0) * 0.5f,
                        gameObject.transform,
                        (entity)=>{
                            this.textEntityList.Add(entity);
                        });
#endif
                    if(i_j_localPos_Callback!=null)
                        i_j_localPos_Callback(gridArray[i, j], i, j,GetWorldPositionXY(i, j) + new Vector3(cellSize, cellSize) * 0.5f);
# if EDITOR_DEBUG                    
                    Debug.DrawLine(GetWorldPositionXY(i, j), GetWorldPositionXY(i, j + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPositionXY(i, j), GetWorldPositionXY(i + 1, j), Color.white, 100f);
#endif
                }
# if EDITOR_DEBUG   
            Debug.DrawLine(GetWorldPositionXY(0, height), GetWorldPositionXY(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPositionXY(width, 0), GetWorldPositionXY(width, height), Color.white, 100f);
#endif
        }
        public void SetValue(Vector3 worldPosition, T value)
        {
            GetXY(worldPosition, out int x, out int y);
            SetValue(x, y, value);
        }
        public T GetValue(Vector3 worldPosition)
        {
            GetXY(worldPosition, out int x, out int y);
            return GetValue(x, y);
        }
    }
}
