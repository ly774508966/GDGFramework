using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using GDG.Utils;
using UnityEngine;
using GDG.ModuleManager;
using GDG.Base;

namespace GDG.AI
{

    public class FlowFieldController
    {
        private class FlowFieldManager : LazySingleton<FlowFieldManager>
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
                if (flowFieldName != null && m_Name2FlowFieldMapping.TryGetValue(flowFieldName, out FlowField flowField))
                {
                    return flowField;
                }
                return null;
            }
            public int FlowFieldCount { get => m_Name2FlowFieldMapping.Count; }
        }
        public static int FlowFieldCount { get => FlowFieldManager.Instance.FlowFieldCount; }
        /// <summary>
        /// 生成流场、但暂未设置目标点
        /// </summary>
        /// <param name="flowFieldName">流场名</param>
        /// <param name="startPos">开始坐标</param>
        /// <param name="endPos">结束坐标</param>
        /// <param name="cellSize">网格大小</param>
        /// <param name="impassibleLayerName">障碍Layer层的名称</param>
        /// <param name="roughTerrainLayer">硬地面Layer层（指对流场有加权影响的Layer）</param>
        /// <param name="gridType">流场网格类型</param>
        /// <param name="roughTerrainLayerName_Cost">硬地面Layer层名称和所需费用</param>
        /// <returns></returns>
        public static FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, string impassibleLayerName, int roughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            return FlowFieldManager.Instance.GenerateFlowField(flowFieldName, startPos, endPos, cellSize, impassibleLayerName, roughTerrainLayer, gridType, roughTerrainLayerName_Cost);
        }
        /// <summary>
        /// 生成流场、但暂未设置目标点，障碍Layer层默认为 Imapssible
        /// </summary>
        /// <param name="flowFieldName">流场名</param>
        /// <param name="startPos">开始坐标</param>
        /// <param name="endPos">结束坐标</param>
        /// <param name="cellSize">网格大小</param>
        /// <param name="gridType">流场网格类型</param>
        /// <returns></returns>
        public static FlowField GenerateFlowField(string flowFieldName, Vector3 startPos, Vector3 endPos, float cellSize, GridType gridType = GridType.Grid3D)
        {
            return FlowFieldManager.Instance.GenerateFlowField(flowFieldName, startPos, endPos, cellSize, gridType);
        }
        /// <summary>
        /// 生成流场、但暂未设置目标点
        /// </summary>
        /// <param name="flowFieldName">流场名</param>
        /// <param name="width">网格宽度</param>
        /// <param name="height">网格长度</param>
        /// <param name="cellSize">网格大小</param>
        /// <param name="localPosition">网格本地坐标</param>
        /// <param name="impassibleLayerName">障碍Layer层的名称</param>
        /// <param name="roughTerrainLayer">硬地面Layer层（指对流场有加权影响的Layer）</param>
        /// <param name="gridType">流场网格类型</param>
        /// <param name="roughTerrainLayerName_Cost">硬地面Layer层名称和所需费用</param>
        /// <returns></returns>
        public static FlowField GenerateFlowField(string flowFieldName, int width, int height, int cellSize, Vector3 localPosition, string impassibleLayerName, int roughTerrainLayer = 0, GridType gridType = GridType.Grid3D, params (string, byte)[] roughTerrainLayerName_Cost)
        {
            return FlowFieldManager.Instance.GenerateFlowField(flowFieldName, width, height, cellSize, localPosition, impassibleLayerName, roughTerrainLayer, gridType, roughTerrainLayerName_Cost);
        }
        /// <summary>
        ////
        /// </summary>
        /// <param name="flowFieldName"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="cellSize"></param>
        /// <param name="localPosition"></param>
        /// <param name="gridType"></param>
        /// <returns></returns>

        /// <summary>
        /// 生成流场、但暂未设置目标点，障碍Layer层默认为 Imapssible
        /// </summary>
        /// <param name="flowFieldName">流场名</param>
        /// <param name="width">网格宽度</param>
        /// <param name="height">网格长度</param>
        /// <param name="cellSize">网格大小</param>
        /// <param name="localPosition">网格本地坐标</param>
        /// <param name="gridType">流场网格类型</param>
        /// <returns></returns>
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