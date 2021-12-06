using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GDG.Utils;
using UnityEngine;
using GDG.Base;

namespace GDG.ModuleManager
{
    public class RuntimeLogManager : Singleton<RuntimeLogManager>
    {
        public static bool EnableRuntimeLog = true;
        public static bool EnableEditorLog = true;
        public static bool EnableTime = true;
        public static bool EnableFilePath = true;
        public static bool EnableLog = true;
        DataSet ds;
        DataTable dt;
        Texture circle;
        Texture square_1;
        Texture square_2;
        Texture square_3;
        GUIStyle style_1;
        GUIStyle style_2;
        GUIStyle style_3;
        GUIStyle btnstyle;
        public RuntimeLogManager()
        {
            ds = new DataSet();
            dt = ds.Tables.Add("logtable");
            dt.Columns.Add("type", typeof(string));
            dt.Columns.Add("info", typeof(string));
            circle = ResourcesManager.Instance.LoadResource<Texture>("Circle");
            square_1 = ResourcesManager.Instance.LoadResource<Texture>("Square_1");
            square_2 = ResourcesManager.Instance.LoadResource<Texture>("square_2");
            square_3 = ResourcesManager.Instance.LoadResource<Texture>("square_3");

            style_1 = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
            };
            style_1.normal.background = square_1 as Texture2D;
            style_1.normal.textColor = Color.white;
            style_1.padding = new RectOffset(8, 3, 8, 3);
            style_1.margin = new RectOffset(5, 5, 2, 2);

            style_2 = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 12,
            };
            style_2.normal.background = square_2 as Texture2D;
            style_2.normal.textColor = Color.white;
            style_2.padding = new RectOffset(8, 3, 8, 3);
            style_2.margin = new RectOffset(5, 5, 2, 2);

            style_3 = new GUIStyle()
            {
                fontStyle = FontStyle.Normal,
                fontSize = 12,
            };
            style_3.normal.textColor = Color.white;
            style_3.normal.background = square_3 as Texture2D;
            style_3.padding = new RectOffset(4, 3, 3, 3);
            style_3.alignment = TextAnchor.MiddleLeft;



            btnstyle = new GUIStyle()
            {
                fontStyle = FontStyle.Bold,
                fontSize = 11,
            };
            btnstyle.normal.textColor = Color.white;
            btnstyle.normal.background = square_3 as Texture2D;
            btnstyle.onHover.background = square_3 as Texture2D;
            btnstyle.alignment = TextAnchor.MiddleCenter;
            btnstyle.padding = new RectOffset(0, 0, 0, 2);
            MonoWorld.Instance.AddOrRemoveListener(OnGUI, "OnGUI");
        }
        #region Runtime日志
        enum LogType
        {
            Info,
            Warning,
            Error,
            Success,
            All,
        }
        LogType logType = LogType.All;
        bool isClose = false;
        bool isHide = false;
        bool isInMove = false;
        float maxheight = 400f;
        float minheight = 23f;
        float height = 300f;
        string showbtn = "Show";
        string search = "";
        bool flag = true;
        Vector2 scrollView = Vector2.zero;
        Vector2 maxscrollView = Vector2.zero;

        void OnGUI()
        {
            if (!EnableRuntimeLog || !EnableLog)
                return;

            if (!isClose)
                GUI.Window(1, new Rect(15, 0, Screen.width - 30, height), DrawWindow, "");

            if (isInMove)
                Move();
        }

        void Move()
        {
            if (isHide)
            {
                if (height > minheight)
                    height -= 2f;
                else
                    isInMove = false;
            }
            else
            {
                if (height < maxheight)
                    height += 2f;
                else
                    isInMove = false;
            }
        }
        public void DrawWindow(int id)
        {

            GUI.color = Color.white;
            if (GUI.Button(new Rect(15, 2, 16, 16), circle))
                logType = LogType.Info;
            GUI.color = Color.white;

            GUI.color = Color.green;
            if (GUI.Button(new Rect(35, 2, 16, 16), circle))
                logType = LogType.Success;
            GUI.color = Color.white;

            GUI.color = new Color(250 / 256f, 160 / 256f, 35 / 256f);
            if (GUI.Button(new Rect(55, 2, 16, 16), circle))
                logType = LogType.Warning;
            GUI.color = Color.white;

            GUI.color = new Color(247 / 256f, 44 / 256f, 120 / 256f);
            if (GUI.Button(new Rect(75, 2, 16, 16), circle))
                logType = LogType.Error;
            GUI.color = Color.white;

            search = GUI.TextField(new Rect(95, 3.1f, 120, 14), search, 10, style_3);

            if (GUI.Button(new Rect(220, 3.1f, 32, 14), "All", btnstyle))
                logType = LogType.All;

            //展卷
            if (GUI.Button(new Rect(Screen.width - 85, 1, 50, 18), showbtn))
            {
                isInMove = true;
                isHide = !isHide;
                showbtn = isHide ? "Show" : "Hide";
                if (isHide && height > 100) maxheight = height;
            }
            height = GUI.HorizontalSlider(new Rect(Screen.width - 320, 5, 200, 20), height, minheight, 350);
            if (height > 100 && !isInMove)
                maxheight = height;

            //滚动日志
            scrollView = GUILayout.BeginScrollView(scrollView);

            DataRow[] rows = null;
            if (search != "")
            {
                rows = dt.Select($"info like '%{search}%'");
            }
            else
            {
                switch (logType)
                {
                    case LogType.Info: rows = dt.Select("type = 'info'"); break;
                    case LogType.Warning: rows = dt.Select("type = 'warning'"); break;
                    case LogType.Error: rows = dt.Select("type = 'error'"); break;
                    case LogType.Success: rows = dt.Select("type = 'success'"); break;
                    default: rows = dt.Rows.Cast<DataRow>().ToArray(); break;
                }
            }
            foreach (DataRow row in rows)
            {
                string logtype = row["type"].ToString();
                switch (logtype)
                {
                    case "info":
                        GUI.color = Color.white;
                        break;
                    case "warning":
                        GUI.color = new Color(250 / 256f, 160 / 256f, 35 / 256f);
                        break;
                    case "error":
                        GUI.color = new Color(247 / 256f, 44 / 256f, 120 / 256f);
                        break;
                    case "success":
                        GUI.color = new Color(139 / 256f, 191 / 256f, 65 / 256f);
                        break;
                }
                if (flag)
                {
                    GUILayout.TextField(row["info"].ToString(), style_1, GUILayout.Height(60));
                }
                else
                {
                    GUILayout.TextField(row["info"].ToString(), style_2, GUILayout.Height(60));
                }
                flag = !flag;

                GUI.color = Color.white;
            }

            GUILayout.EndScrollView();
            GUILayout.Space(22);

            //关闭窗口
            if (GUI.Button(new Rect(Screen.width - 130, maxheight - 30, 70, 23), "Close"))
            {
                isClose = true;
            }
            //清空日志
            if (GUI.Button(new Rect(Screen.width - 210, maxheight - 30, 70, 23), "Clear"))
            {
                ds.Tables["logtable"].Clear();
            }
        }
        string RuntimeMessageFormat(object message, string invoker, string callerFilePath, int callerLineNumber)
        {
            if (!EnableLog)
                return "";
            if(message==null)
                message = "Null";
            string info = "";
            if (EnableTime)
            {
                if (EnableFilePath)
                    info = $"<color=#7a7e83> [{DateTime.Now.ToString("HH:mm:ss")}]  [{callerFilePath} | {invoker}( ) | {callerLineNumber}] </color> \n\n{message.ToString()}";
                else
                    info = $"<color=#7a7e83> [{DateTime.Now.ToString("HH:mm:ss")}] </color> \n\n{message.ToString()}";
            }
            else
            {
                if (EnableFilePath)
                    info = $"<color=#7a7e83> [{callerFilePath} | {invoker}( ) | {callerLineNumber}] </color>\n\n{message.ToString()}";
                else
                    info = $"\n{message.ToString()}\n";
            }
            return info;
        }
        public void RuntimeLogSuccess(
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableRuntimeLog || !EnableLog)
                return;
            var info = RuntimeMessageFormat(message, invoker, callerFilePath, callerLineNumber);
            DataRow row = dt.NewRow();
            row["type"] = "success";
            row["info"] = info;
            dt.Rows.Add(row);
            scrollView.y = height * dt.Rows.Count * 80;
        }
        public void RuntimeLogInfo(
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableRuntimeLog || !EnableLog)
                return;
            var info = RuntimeMessageFormat(message, invoker, callerFilePath, callerLineNumber);
            DataRow row = dt.NewRow();
            row["type"] = "info";
            row["info"] = info;
            dt.Rows.Add(row);
            scrollView.y = height * dt.Rows.Count * 80;
        }
        public void RuntimeLogWarning(
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableRuntimeLog || !EnableLog)
                return;

            var info = RuntimeMessageFormat(message, invoker, callerFilePath, callerLineNumber);
            DataRow row = dt.NewRow();
            row["type"] = "warning";
            row["info"] = info;
            dt.Rows.Add(row);
            scrollView.y = height * dt.Rows.Count * 80;
        }
        public void RuntimeLogError(
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableRuntimeLog || !EnableLog)
                return;

            var info = RuntimeMessageFormat(message, invoker, callerFilePath, callerLineNumber);

            DataRow row = dt.NewRow();
            row["type"] = "error";
            row["info"] = info;
            dt.Rows.Add(row);
            scrollView.y = height * dt.Rows.Count * 80;
        }
        #endregion
    }
}