using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using GDG.Utils;
using GDG.ECS;

namespace GDG.Utils
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
    public class Cell : IComparable<Cell>
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
        public Utils.Grid<Cell> grid;
        private float cellSize;
        private Cell destinationCell;
        private GridType gridType;
        public FlowField(Vector3 startPos, Vector3 endPos, float cellSize, GridType gridType = GridType.Grid3D)
        {
            this.cellSize = cellSize;
            this.gridType = gridType;
            if (gridType == GridType.Grid3D) grid = new Grid3D<Cell>(
                 Mathf.CeilToInt(Mathf.Abs(startPos.x - endPos.x) / cellSize),
                 Mathf.CeilToInt(Mathf.Abs(startPos.z - endPos.z) / cellSize),
                 cellSize,
                 new Cell(),
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
                 new Cell(),
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

        private List<Cell> GetNeighborCells_4Directions(Vector2Int index)
        {
            List<Cell> cells = new List<Cell>();
            cells.Add(grid.GetValue(index.x - 1, index.y));
            cells.Add(grid.GetValue(index.x + 1, index.y));
            cells.Add(grid.GetValue(index.x, index.y - 1));
            cells.Add(grid.GetValue(index.x, index.y + 1));
            return cells;
        }
        private Cell GetMinCellFromNeighbor_8Directions(Vector2Int index)
        {
            List<Cell> cells = new List<Cell>();

            Cell destinationCell = grid.GetValue(index.x, index.y);
            Cell minCell = destinationCell;

            for (int i = index.x - 1 ; i <= index.x + 1; i++)
            {
                for (int j = index.y - 1; j <= index.y + 1; j++)
                {
                    if (i == index.x && j == index.y)
                        continue;

                    var cell = grid.GetValue(i, j);
                    if(cell!=null)
                        minCell = GetMinCell(cell, minCell);
                }
            }
            return minCell;
        }
        private Cell GetMinCell(Cell cell1,Cell cell2)
        {
            if(cell1.CompareTo(cell2)<0)
                return cell1;
            return cell2;
        }
        private FieldDirection GetBestDirection(Vector2Int index)
        {
            var cell = grid.GetValue(index.x, index.y);
            if(cell==null)
                return FieldDirection.None;

            var minCell = GetMinCellFromNeighbor_8Directions(index);

            (int, int) dir = (minCell.index.x - index.x, minCell.index.y - index.y);
            cell.direction = (minCell.worldPos - cell.worldPos).normalized;
            switch (dir)
            {
                case ( 1,  0): return FieldDirection.East;
                case (-1,  0): return FieldDirection.West;
                case ( 0,  1): return FieldDirection.North;
                case ( 0, -1): return FieldDirection.South;
                case ( 1,  1): return FieldDirection.NorthEast;
                case (-1,  1): return FieldDirection.NorthWest;
                case ( 1, -1): return FieldDirection.SouthEast;
                case (-1, -1): return FieldDirection.SouthWest;
                default: return FieldDirection.None;
            }
        }
       
        //生成成本场
        internal void GenarateCostField(string impassibleLayerName, int firstRoughTerrainLayer = 0, int lastRoughTerrainLayer = 0, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            Collider[] colliders;
            var halfExtents = Vector3.one * cellSize / 2;
            var hasIncrease = false;
            //过滤层级
            int mask = 0;
            if (firstRoughTerrainLayer == 0 || lastRoughTerrainLayer == 0)
                mask = LayerMask.GetMask(impassibleLayerName);
            else
                mask = LayerMask.GetMask(impassibleLayerName) | firstRoughTerrainLayer << lastRoughTerrainLayer;

            foreach (var cell in grid.gridArray)
            {
                hasIncrease = false;
                colliders = Physics.OverlapBox(cell.worldPos, halfExtents, Quaternion.identity, mask);
                foreach (var collider in colliders)
                {
                    if (collider.gameObject.layer == LayerMask.NameToLayer(impassibleLayerName))
                    {
                        hasIncrease = true;
                        cell.IncreaseCost(255);
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
        }
        //生成积分场
        internal void GenerateIntegrationField(Vector3 destinationWorldPos)
        {
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
                return;

            destinationCell.integration = 0;
            destinationCell.fieldType = FieldType.Integration;
            Queue<Cell> cellQueue = new Queue<Cell>();
            cellQueue.Enqueue(destinationCell);

            grid.SetValue(destinationCell.index.x, destinationCell.index.y, destinationCell);

            Cell currentCell;
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
                    }
                }
            }
            this.destinationCell = destinationCell;
        }
        //生成矢量场
        internal void GenerateVectorField()
        {
            if (destinationCell == null)
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
            var cell = grid.GetValue(x,y);
            if(cell!=null)
                return cell.direction;
            else
                return Vector3.zero;
        }
        public Vector3 GetFieldDirection(Vector3 worldPosition)
        {
            Cell cell;

            if(gridType == GridType.Grid3D) 
                cell =(grid as Grid3D<Cell>).GetValue(worldPosition);
            else    
                cell =(grid as Grid2D<Cell>).GetValue(worldPosition);

            if(cell!=null)
                return cell.direction;
            else
                return Vector3.zero;
        }

        internal void Dispose()
        {
            foreach (var item in grid.textEntityList)
            {
                World.EntityManager.RecycleEntity(item);
            }
            grid = null;
        }
    }
    public static class FlowFieldExtension
    {
        public static void SetDestination(this FlowField flowField, Vector3 destinationWorldPos)
        {
            flowField.GenerateIntegrationField(destinationWorldPos);
            flowField.GenerateVectorField();
        }
    }
}
