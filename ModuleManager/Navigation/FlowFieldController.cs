using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;

namespace GDG.ModuleManager
{
    public class FlowFieldController : AbsLazySingleton<FlowFieldController>
    {
        private readonly Dictionary<string, FlowField> m_Name2FlowFieldMapping = new Dictionary<string, FlowField>();

        public FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, string impassibleLayerName, int firstRoughTerrainLayer = 0, int lastRoughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
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

            flowField.GenarateCostField(impassibleLayerName, firstRoughTerrainLayer, lastRoughTerrainLayer, roughTerrainLayerName_Cost);

            return flowField;
        }
        public FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, GridType gridType = GridType.Grid3D)
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
            return flowField;
        }
        public FlowField GenerateFlowField(string flowFieldName, int width, int height, int cellSize, Vector3 localPosition, string impassibleLayerName, int firstRoughTerrainLayer = 0, int lastRoughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
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


            flowField.GenarateCostField(impassibleLayerName, firstRoughTerrainLayer, lastRoughTerrainLayer, roughTerrainLayerName_Cost);

            return flowField;
        }
        public FlowField GenerateFlowField(string flowFieldName, int width, int height, int cellSize, Vector3 localPosition, GridType gridType = GridType.Grid3D)
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
            return flowField;
        }
        public void DestoryFlowField(string flowFieldName)
        {
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField flowField))
            {
                m_Name2FlowFieldMapping.Remove(flowFieldName);
                flowField.Dispose();
            }
        }
        public void DestoryFlowField(FlowField flowField)
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
        public bool TryDestoryFlowField(string flowFieldName)
        {
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField flowField))
            {
                m_Name2FlowFieldMapping.Remove(flowFieldName);
                flowField.Dispose();
                return true;
            }
            return false;
        }
        public bool TryDestoryFlowField(FlowField flowField)
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
        public FlowField GetFlowField(string flowFieldName)
        {
            if (m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField flowField))
            {
                return flowField;
            }
            return null;
        }
        public int FlowFieldCount { get => m_Name2FlowFieldMapping.Count; }
    }
}