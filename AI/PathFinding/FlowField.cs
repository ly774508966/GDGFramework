using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using GDG.Utils;
using GDG.ECS;
using GDG.Async;
using GDG.Base;
namespace GDG.AI
{
    public enum GridType
    {
        Grid2D,
        Grid3D
    }
    public enum FieldType
    {
        Cost,
        Integration,
        Vector,
    }
    public enum FieldDirection
    {
        North,
        South,
        East,
        West,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
        None,
        Destinatoin
    }
    public class Cell : IComparable<Cell>,IRecyclable
    {
        public Vector3 worldPos;
        public byte cost = 1;
        public ushort integration = ushort.MaxValue;
        public string directionStr;
        public Vector3 direction;
        public Vector2Int index;
        public FieldType fieldType = FieldType.Cost;

        public int CompareTo(Cell other)
        {
            if (integration > other.integration)
                return 1;
            else if (integration < other.integration)
                return -1;
            return 0;
        }

        public void IncreaseCost(int value)
        {
            if (cost == byte.MaxValue)
                return;
            if (cost + value >= 255)
                cost = byte.MaxValue;
            else
                cost += (byte)value;
        }

        public void OnRecycle()
        {
            worldPos = direction = default(Vector3);
            cost = 1;
            integration = ushort.MaxValue;
            directionStr = null;
            index = default(Vector2Int);
            fieldType = FieldType.Cost;
        }

        public override string ToString()
        {
            switch (fieldType)
            {
                case FieldType.Cost: return cost.ToString();
                case FieldType.Integration: return integration.ToString();
                default: return directionStr;
            }
        }
    }
    public class FlowField
    {
        public GridBase<Cell> grid;
        private float cellSize;
        private Cell destinationCell;
        private GridType gridType;
        private int impasableCellCount;
        internal bool EnableSmartPathFinding = true;
        public FlowField(Vector3 startPos, Vector3 endPos, float cellSize, GridType gridType = GridType.Grid3D)
        {
            this.cellSize = cellSize;
            this.gridType = gridType;
            if (gridType == GridType.Grid3D) grid = new Grid3D<Cell>(
                 Mathf.CeilToInt(Mathf.Abs(startPos.x - endPos.x) / cellSize),
                 Mathf.CeilToInt(Mathf.Abs(startPos.z - endPos.z) / cellSize),
                 cellSize,
                 null,
                 startPos,
                 (cell, i, j, pos) =>
                 {
                     cell.worldPos = pos;
                     cell.index = new Vector2Int(i, j);
                 });
            else grid = new Grid2D<Cell>(
                 Mathf.CeilToInt(Mathf.Abs(startPos.x - endPos.x)),
                 Mathf.CeilToInt(Mathf.Abs(startPos.y - endPos.y)),
                 cellSize,
                 null,
                 startPos,
                 (cell, i, j, pos) =>
                 {
                     cell.worldPos = pos;
                     cell.index = new Vector2Int(i, j);
                 });
        }
        public FlowField(int width, int height, Vector3 localPosition, int cellSize, GridType gridType = GridType.Grid3D)
        {
            this.cellSize = cellSize;
            this.gridType = gridType;
            if (gridType == GridType.Grid3D) grid = new Grid3D<Cell>(
                 width,
                 height,
                 cellSize,
                 null,
                 localPosition,
                 (cell, i, j, pos) =>
                 {
                     cell.worldPos = pos;
                     cell.index = new Vector2Int(i, j);
                 });
            else grid = new Grid2D<Cell>(
                 width,
                 height,
                 cellSize,
                 null,
                 localPosition,
                 (cell, i, j, pos) =>
                 {
                     cell.worldPos = pos;
                     cell.index = new Vector2Int(i, j);
                 });
        }

        private Cell[] GetNeighborCells_4Directions(Vector2Int index)
        {
            Cell[] cells = new Cell[4]
            {
                grid.GetValue(index.x - 1, index.y),
                grid.GetValue(index.x + 1, index.y),
                grid.GetValue(index.x, index.y - 1),
                grid.GetValue(index.x, index.y + 1),
            };
            // List<Cell> cells = new List<Cell>();
            // cells.Add(grid.GetValue(index.x - 1, index.y));//左
            // cells.Add(grid.GetValue(index.x + 1, index.y));//右
            // cells.Add(grid.GetValue(index.x, index.y - 1));//上
            //cells.Add(grid.GetValue(index.x, index.y + 1));//下
            return cells;
        }
        private Cell GetMinCellFromNeighbor_8Directions(Vector2Int index)
        {
            List<Cell> cells = new List<Cell>();

            Cell destinationCell = grid.GetValue(index.x, index.y);
            Cell minCell = destinationCell;

            for (int i = index.x - 1; i <= index.x + 1; i++)
            {
                for (int j = index.y - 1; j <= index.y + 1; j++)
                {
                    if (i == index.x && j == index.y)
                        continue;

                    var cell = grid.GetValue(i, j);
                    if (cell != null)
                        minCell = GetMinCell(cell, minCell);
                }
            }
            return minCell;
        }
        private Cell GetMinCell(Cell cell1, Cell cell2)
        {
            if (cell1.CompareTo(cell2) < 0)
                return cell1;
            return cell2;
        }
        private FieldDirection GetBestDirection(Vector2Int index)
        {
            var cell = grid.GetValue(index.x, index.y);
            if (cell == null)
                return FieldDirection.None;

            var minCell = GetMinCellFromNeighbor_8Directions(index);

            (int, int) dir = (minCell.index.x - index.x, minCell.index.y - index.y);
            cell.direction = (minCell.worldPos - cell.worldPos).normalized;
            switch (dir)
            {
                case (1, 0): return FieldDirection.East;
                case (-1, 0): return FieldDirection.West;
                case (0, 1): return FieldDirection.North;
                case (0, -1): return FieldDirection.South;
                case (1, 1): return FieldDirection.NorthEast;
                case (-1, 1): return FieldDirection.NorthWest;
                case (1, -1): return FieldDirection.SouthEast;
                case (-1, -1): return FieldDirection.SouthWest;
                default: return FieldDirection.None;
            }
        }

        //生成成本场
        internal void GenarateCostField(string impassibleLayerName = "Impassible", int roughTerrainLayer = 0, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            if (grid == null)
                return;
            impasableCellCount = 0;
            Collider[] colliders;
            Collider2D[] collider2Ds;
            var halfExtents = Vector3.one * cellSize / 2;
            var hasIncrease = false;
            //过滤层级
            int mask = 0;
            if (roughTerrainLayer == 0)
                mask = LayerMask.GetMask(impassibleLayerName);
            else
                mask = LayerMask.GetMask(impassibleLayerName) | roughTerrainLayer;

            foreach (var cell in grid.gridArray)
            {
                hasIncrease = false;

                if (gridType == GridType.Grid3D)
                {
                    colliders = Physics.OverlapBox(cell.worldPos, halfExtents, Quaternion.identity, mask);
                    foreach (var collider in colliders)
                    {
                        if (collider.gameObject.layer == LayerMask.NameToLayer(impassibleLayerName))
                        {
                            hasIncrease = true;
                            cell.IncreaseCost(255);
                            impasableCellCount++;
                            grid.SetValue(cell.index.x, cell.index.y, cell);
                        }
                        else
                        {
                            foreach (var item in roughTerrainLayerName_Cost)
                            {
                                if (collider.gameObject.layer == LayerMask.NameToLayer(item.Item1))
                                {
                                    hasIncrease = true;
                                    cell.IncreaseCost(item.Item2);
                                    grid.SetValue(cell.index.x, cell.index.y, cell);
                                }
                            }
                        }
                    }
                    if (!hasIncrease)
                    {
                        cell.cost = 1;
                        grid.SetValue(cell.index.x, cell.index.y, cell);
                    }
                }
                else
                {
                    collider2Ds = Physics2D.OverlapBoxAll(cell.worldPos, halfExtents, mask);
                    foreach (var collider in collider2Ds)
                    {
                        if (collider.gameObject.layer == LayerMask.NameToLayer(impassibleLayerName))
                        {
                            hasIncrease = true;
                            cell.IncreaseCost(255);
                            impasableCellCount++;
                            grid.SetValue(cell.index.x, cell.index.y, cell);
                        }
                        else
                        {
                            foreach (var item in roughTerrainLayerName_Cost)
                            {
                                if (collider.gameObject.layer == LayerMask.NameToLayer(item.Item1))
                                {
                                    hasIncrease = true;
                                    cell.IncreaseCost(item.Item2);
                                    grid.SetValue(cell.index.x, cell.index.y, cell);
                                }
                            }
                        }
                    }
                }
            }
        }
        //生成积分场
        internal bool GenerateIntegrationField(Vector3 destinationWorldPos, out bool isEnclosedArea)
        {
            isEnclosedArea = false;

            if (grid == null)
                return false;

            bool canReach = true;
            foreach (var cell in grid.gridArray)
            {
                cell.integration = ushort.MaxValue;
            }

            Cell destinationCell;

            if (gridType == GridType.Grid3D)
                destinationCell = (grid as Grid3D<Cell>).GetValue(destinationWorldPos);
            else
                destinationCell = (grid as Grid2D<Cell>).GetValue(destinationWorldPos);

            if (destinationCell == null)
                return false;

            //如果目的地是障碍物，则选择一个里目的地最近的非障碍物cell作为新目的地
            if (destinationCell.cost == byte.MaxValue)
            {
                if (!EnableSmartPathFinding)
                    return false;
                canReach = false;
                Queue<Cell> desCellQueue = new Queue<Cell>();
                desCellQueue.Enqueue(destinationCell);
                Cell cell;
                while (desCellQueue.Count > 0)
                {
                    cell = desCellQueue.Dequeue();
                    if (cell == null)
                        continue;

                    var neighbourCells = GetNeighborCells_4Directions(cell.index);

                    bool isFoundDes = false;
                    foreach (var item in neighbourCells)
                    {
                        if (item == null)
                            continue;
                        if (item.cost != byte.MaxValue)
                        {
                            isFoundDes = true;
                            destinationCell = item;
                            break;
                        }
                        else
                            desCellQueue.Enqueue(item);
                    }
                    if (isFoundDes)
                        break;
                }
            }


            destinationCell.integration = 0;
            destinationCell.fieldType = FieldType.Integration;
            Queue<Cell> cellQueue = new Queue<Cell>();
            cellQueue.Enqueue(destinationCell);

            grid.SetValue(destinationCell.index.x, destinationCell.index.y, destinationCell);

            Cell currentCell;
            var count = 0;
            while (cellQueue.Count > 0)
            {
                currentCell = cellQueue.Dequeue();
                if (currentCell == null)
                    continue;
                var neighbourCells = GetNeighborCells_4Directions(currentCell.index);

                foreach (var item in neighbourCells)
                {
                    if (item == null)
                        continue;

                    item.fieldType = FieldType.Integration;

                    //如果是路障则跳过
                    if (item.cost == byte.MaxValue)
                    {
                        item.integration = (ushort)item.cost;
                        grid.SetValue(item.index.x, item.index.y, item);
                        continue;
                    }
                    //否则比较费用
                    if (currentCell.integration + item.cost < item.integration)
                    {
                        item.integration = (ushort)(currentCell.integration + item.cost);
                        cellQueue.Enqueue(item);
                        grid.SetValue(item.index.x, item.index.y, item);
                        count++;
                    }
                }
            }
            isEnclosedArea = count >= grid.gridArray.Length - impasableCellCount;
            this.destinationCell = destinationCell;
            return canReach;
        }

        //生成矢量场
        internal void GenerateVectorField()
        {
            if (destinationCell == null || grid == null)
                return;

            foreach (var cell in grid.gridArray)
            {
                cell.fieldType = FieldType.Vector;
                var dir = GetBestDirection(cell.index);

                if (cell.cost == byte.MaxValue)
                {
                    cell.directionStr = "×";
                    grid.SetValue(cell.index.x, cell.index.y, cell);
                    continue;
                }

                switch (dir)
                {
                    case FieldDirection.North: cell.directionStr = "↑"; break;
                    case FieldDirection.South: cell.directionStr = "↓"; break;
                    case FieldDirection.East: cell.directionStr = "→"; break;
                    case FieldDirection.West: cell.directionStr = "←"; break;
                    case FieldDirection.NorthEast: cell.directionStr = "↗"; break;
                    case FieldDirection.NorthWest: cell.directionStr = "↖"; break;
                    case FieldDirection.SouthEast: cell.directionStr = "↘"; break;
                    case FieldDirection.SouthWest: cell.directionStr = "↙"; break;
                }
                grid.SetValue(cell.index.x, cell.index.y, cell);
            }
            destinationCell.directionStr = "●";
            destinationCell.direction = Vector3.zero;
            grid.SetValue(destinationCell.index.x, destinationCell.index.y, destinationCell);
        }
        public Vector3 GetFieldDirection(int x, int y)
        {
            if (grid == null)
                return Vector3.zero;

            var cell = grid.GetValue(x, y);
            if (cell != null)
                return cell.direction;
            else
                return Vector3.zero;
        }
        public Vector3 GetFieldDirection(Vector3 worldPosition)
        {
            if (grid == null)
                return Vector3.zero;

            Cell cell;

            if (gridType == GridType.Grid3D)
                cell = (grid as Grid3D<Cell>).GetValue(worldPosition);
            else
                cell = (grid as Grid2D<Cell>).GetValue(worldPosition);

            if (cell != null)
                return cell.direction;
            else
                return Vector3.zero;
        }

        internal void Dispose()
        {
            if (grid != null)
            {
                AsyncRunner.RunAsync(() =>
                {
                    for (int i = 0; i < grid.gridArray.GetLength(0); i++)
                        for (int j = 0; j < grid.gridArray.GetLength(1); j++)
                        {
                            ObjectPool<Cell>.Instance.Push(grid.gridArray[i, j]);
                        }
                    AsyncRunner.SyncToMainThread(() =>
                    {
                        foreach (var item in grid.textEntityList)
                        {
                            World.EntityManager.DestroyEntity(item);
                        }
                        grid.gridArray = null;
                        grid.gridTextArray = null;
                        grid = null;
                    });
                });
            }
        }
    }
    public static class FlowFieldExtension
    {
        /// <summary>
        /// 设置目的地
        /// </summary>
        /// <param name="destinationWorldPos">目的地世界坐标</param>
        /// <param name="enableSmartPathFinding">若为true，则当目的地处在Impassable Layer时，自动重新调整目的地</param>
        /// <returns>目的地是否处在Impassable Layer中，或是被设置在流场范围外</returns>
        public static bool SetDestination(this FlowField flowField, Vector3 destinationWorldPos, bool enableSmartPathFinding = true)
        {
            if (flowField.grid == null)
                return false;
            flowField.EnableSmartPathFinding = enableSmartPathFinding;
            var canReach = flowField.GenerateIntegrationField(destinationWorldPos, out bool _);
            flowField.GenerateVectorField();
            return canReach;
        }
        /// <summary>
        /// 设置目的地
        /// </summary>
        /// <param name="destinationWorldPos">目的地世界坐标</param>
        /// <param name="isEnclosedArea">相对于整个流场而言，目的地是否被设置在一个封闭区域，比如围墙内</param>
        /// <param name="enableSmartPathFinding">若为true，则当目的地处在Impassable Layer时，自动重新调整目的地</param>
        /// <returns>目的地是否处在Impassable Layer中，或是被设置在流场范围外</returns>
        public static bool SetDestination(this FlowField flowField, Vector3 destinationWorldPos, out bool isEnclosedArea, bool enableSmartPathFinding = true)
        {
            isEnclosedArea = false;
            if (flowField.grid == null)
                return false;
            flowField.EnableSmartPathFinding = enableSmartPathFinding;
            var canReach = flowField.GenerateIntegrationField(destinationWorldPos, out isEnclosedArea);
            flowField.GenerateVectorField();
            return canReach;
        }
    }
}
