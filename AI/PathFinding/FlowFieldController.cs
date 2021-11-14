using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
using GDG.ModuleManager;

namespace GDG.AI
{
    internal class FlowFieldManager : LazySingleton<FlowFieldManager>
    {
        private readonly Dictionary<string, FlowField> m_Name2FlowFieldMapping = new Dictionary<string, FlowField>();

        internal FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, string impassibleLayerName, int roughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            FlowField flowField;
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField ff))
            {
                flowField = ff;
            }
            else
            {
                flowField = new FlowField(startPos, endPos, cellSize, gridType);
                m_Name2FlowFieldMapping.Add(flowFieldName, flowField);
            }
            flowField?.GenarateCostField(impassibleLayerName, roughTerrainLayer, roughTerrainLayerName_Cost);
            return flowField;
        }
        internal FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, GridType gridType = GridType.Grid3D)
        {
            FlowField flowField;
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField ff))
            {
                flowField = ff;
            }
            else
            {
                flowField = new FlowField(startPos, endPos, cellSize, gridType);
                m_Name2FlowFieldMapping.Add(flowFieldName, flowField);
            }
            flowField?.GenarateCostField();
            return flowField;
        }
        internal FlowField GenerateFlowField(string flowFieldName, int width, int height, int cellSize, Vector3 localPosition, string impassibleLayerName, int roughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            FlowField flowField;
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField ff))
            {
                flowField = ff;
            }
            else
            {
                flowField = new FlowField(width, height, localPosition, cellSize, gridType);
                m_Name2FlowFieldMapping.Add(flowFieldName, flowField);
            }
            flowField?.GenarateCostField(impassibleLayerName, roughTerrainLayer, roughTerrainLayerName_Cost);
            return flowField;
        }
        internal FlowField GenerateFlowField(string flowFieldName, int width, int height, int cellSize, Vector3 localPosition, GridType gridType = GridType.Grid3D)
        {
            FlowField flowField;
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField ff))
            {
                flowField = ff;
            }
            else
            {
                flowField = new FlowField(width, height, localPosition, cellSize, gridType);
                m_Name2FlowFieldMapping.Add(flowFieldName, flowField);
            }
            flowField?.GenarateCostField();
            return flowField;
        }
        internal void DestoryFlowField(string flowFieldName)
        {
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField flowField))
            {
                m_Name2FlowFieldMapping.Remove(flowFieldName);
                flowField.Dispose();
            }
        }
        internal void DestoryFlowField(FlowField flowField)
        {
            for (int i = 0; i < m_Name2FlowFieldMapping.Values.Count; i++)
            {
                if (m_Name2FlowFieldMapping.Values.ElementAt(i) == flowField)
                {
                    m_Name2FlowFieldMapping.Remove(m_Name2FlowFieldMapping.Keys.ElementAt(i));
                    flowField.Dispose();
                }
            }
        }
        internal bool TryDestoryFlowField(string flowFieldName)
        {
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField flowField))
            {
                m_Name2FlowFieldMapping.Remove(flowFieldName);
                flowField.Dispose();
                return true;
            }
            return false;
        }
        internal bool TryDestoryFlowField(FlowField flowField)
        {
            for (int i = 0; i < m_Name2FlowFieldMapping.Values.Count; i++)
            {
                if (m_Name2FlowFieldMapping.Values.ElementAt(i) == flowField)
                {
                    m_Name2FlowFieldMapping.Remove(m_Name2FlowFieldMapping.Keys.ElementAt(i));
                    flowField.Dispose();
                    return true;
                }
            }
            return false;
        }
        internal FlowField GetFlowField(string flowFieldName)
        {
            if (flowFieldName!=null && m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField flowField))
            {
                return flowField;
            }
            return null;
        }
        public int FlowFieldCount { get => m_Name2FlowFieldMapping.Count; }
    }
    public class FlowFieldController
    {
        public static int FlowFieldCount { get => FlowFieldManager.Instance.FlowFieldCount; }
        public static FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, string impassibleLayerName, int roughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            return FlowFieldManager.Instance.GenerateFlowField(flowFieldName, startPos, endPos, cellSize, impassibleLayerName, roughTerrainLayer, gridType, roughTerrainLayerName_Cost);
        }
        public static FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, GridType gridType = GridType.Grid3D)
        {
            return FlowFieldManager.Instance.GenerateFlowField(flowFieldName, startPos, endPos, cellSize, gridType);
        }
        public static FlowField GenerateFlowField(string flowFieldName, int width, int height, int cellSize, Vector3 localPosition, string impassibleLayerName, int roughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            return FlowFieldManager.Instance.GenerateFlowField(flowFieldName, width, height, cellSize, localPosition, impassibleLayerName, roughTerrainLayer, gridType, roughTerrainLayerName_Cost);
        }
        public static FlowField GenerateFlowField(string flowFieldName, int width, int height, int cellSize, Vector3 localPosition, GridType gridType = GridType.Grid3D)
        {
            return FlowFieldManager.Instance.GenerateFlowField(flowFieldName, width, height, cellSize, localPosition, gridType);
        }
        public static void DestoryFlowField(string flowFieldName)
        {
            FlowFieldManager.Instance.DestoryFlowField(flowFieldName);
        }
        public static void DestoryFlowField(FlowField flowField)
        {
            FlowFieldManager.Instance.DestoryFlowField(flowField);
        }
        public static bool TryDestoryFlowField(string flowFieldName)
        {
            return FlowFieldManager.Instance.TryDestoryFlowField(flowFieldName);
        }
        public static bool TryDestoryFlowField(FlowField flowField)
        {
            return FlowFieldManager.Instance.TryDestoryFlowField(flowField);
        }
        public static FlowField GetFlowField(string flowFieldName)
        {
            return FlowFieldManager.Instance.GetFlowField(flowFieldName);
        }
    }
}