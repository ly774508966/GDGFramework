using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using GDG.ECS;
using GDG.Utils;
using UnityEngine;
namespace GDG.ModuleManager
{
    public class LogManager : Singleton<LogManager>
    {
        private static string FilePath { get => $"{ UserFileManager.Path}/Logger/UnityLogger.txt"; }
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
        public LogManager()
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

            UserFileManager.BuildFile_Async("Logger", "UnityLogger.txt");
            World.monoWorld.AddOrRemoveListener(OnGUI, "OnGUI");
        }
        public static bool EnableConsoleLog = true;
        public static bool EnableRuntimeLog = true;
        public static bool EnableEditorLog = true;
        public static bool EnableWriteIntoLogFile = true;
        public static bool EnableTime = true;
        public static bool EnableFilePath = true;
        public static bool EnableLog = true;
        public static bool LogErrorOrThrowException = true;

        /// <summary>
        /// 日志文件最大容量(MB)，超过会自动清空
        /// </summary>
        public static float LoggerMaxMBSize = 50;
        private static void LoggerWriteIN(string info)
        {
            if (!EnableWriteIntoLogFile)
                return;

            if (new FileInfo(FilePath)?.Length > LogManager.LoggerMaxMBSize * 1024 * 1024)
            {
                UserFileManager.ClearFile("Logger/UnityLogger.txt");
                LogManager.LogWarning("清空了一次日志");
            }

            if (!LogManager.EnableTime)
                UserFileManager.Append("/Logger/UnityLogger.txt", $"[{DateTime.Now.ToString("HH:mm:ss")}]{info}");
            else
                UserFileManager.Append("/Logger/UnityLogger.txt", info);
        }
        private static string ColorToHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            int a = Mathf.RoundToInt(color.a * 255.0f);
            string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
            return hex;
        }

        #region 控制台日志
        public static string MessageFormat(object message, string tag, string invoker, string callerFilePath, int callerLineNumber)
        {
            if (!EnableLog)
                return "";
            if(message==null)
                message = "Null";
            string info;
            if (EnableTime)
                info = $"<b>[{tag}]</b>[{DateTime.Now.ToString("HH:mm:ss")}]   <b>{message.ToString()}</b>\n";
            else
                info = $"<b>[{tag}]</b>   {message.ToString()}\n";

            if (EnableFilePath)
                info = $"{info}From：{invoker}( )  |   line:{callerLineNumber}   |   {callerFilePath}";

            if (File.Exists(FilePath))
            {
                LoggerWriteIN($"[{tag}] [{DateTime.Now.ToString("HH:mm:ss")}] [File: {callerFilePath}，Method：{invoker}( )，Line：{callerLineNumber}] -{message.ToString()}");
            }
            else
            {
                UserFileManager.BuildFile("Logger", "UnityLogger.txt");
            }
            return info;
        }
        public static void LogCustom(object message,
            string tag,
            Color color,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableConsoleLog || !EnableLog)
                return;
            var info = MessageFormat(message, tag, invoker, callerFilePath, callerLineNumber);
            Debug.LogFormat(string.Format($"<color=#{ColorToHex(color)}>" + "{0}</color>", info));
        }
        public static void LogSucess(object message,
            string tag = "SUCESS",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableConsoleLog || !EnableLog)
                return;
            var info = MessageFormat(message, tag, invoker, callerFilePath, callerLineNumber);
            Debug.LogFormat(string.Format("<color=#8BBF41>{0}</color>", info));
        }
        public static void LogInfo(
            object message,
            string tag = "INFO",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableConsoleLog || !EnableLog)
                return;
            var info = MessageFormat(message, tag, invoker, callerFilePath, callerLineNumber);
            Debug.Log(info);
        }
        public static void LogWarning(
            object message,
            string tag = "WARNING",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableConsoleLog || !EnableLog)
                return;
            var info = MessageFormat(message, tag, invoker, callerFilePath, callerLineNumber);
            Debug.LogWarning(string.Format("<color=#E2B652>{0}</color>", info));
        }
        public static void LogError(
            object message,
            string tag = "ERROR",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            var info = MessageFormat(message, tag, invoker, callerFilePath, callerLineNumber);

            if (LogErrorOrThrowException && EnableLog && EnableConsoleLog)
                Debug.LogError(string.Format("<color=#FF534A>{0}</color>", info));
            else
                throw new CustomErrorException(info.ToString(), tag, invoker, callerFilePath, callerLineNumber);
        }
        #endregion
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
            if (File.Exists(FilePath))
            {
                LoggerWriteIN($"[{DateTime.Now.ToString("HH:mm:ss")}] [File: {callerFilePath}，Method：{invoker}( )，Line：{callerLineNumber}] -{message.ToString()}");
            }
            else
            {
                UserFileManager.BuildFile("Logger", "UnityLogger.txt");
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

            if (!LogErrorOrThrowException)
                throw new CustomErrorException(message.ToString(), "ERROR", invoker, callerFilePath, callerLineNumber);

            var info = RuntimeMessageFormat(message, invoker, callerFilePath, callerLineNumber);

            DataRow row = dt.NewRow();
            row["type"] = "error";
            row["info"] = info;
            dt.Rows.Add(row);
            scrollView.y = height * dt.Rows.Count * 80;
        }
        #endregion
        #region Editor下日志
        private static string EditorMessageFormat(object message, string tag, string invoker, string callerFilePath, int callerLineNumber)
        {
            if (!EnableLog)
                return "";
            if(message==null)
                message = "Null";
            string info;
            if (EnableTime)
                info = $"<b>[{tag}]</b>[{DateTime.Now.ToString("HH:mm:ss")}]   <b>{message.ToString()}</b>\n";
            else
                info = $"<b>[{tag}]</b>   {message.ToString()}\n";

            if (EnableFilePath)
                info = $"{info}From：{invoker}( )  |   line:{callerLineNumber}   |   {callerFilePath}";

            if (File.Exists(FilePath))
            {
                LoggerWriteIN($"[{tag}] [{DateTime.Now.ToString("HH:mm:ss")}] [File: {callerFilePath}，Method：{invoker}( )，Line：{callerLineNumber}] -{message.ToString()}");
            }
            else
            {
                UserFileManager.BuildFile("Logger", "UnityLogger.txt");
            }

            return info;
        }
        public static void EditorLog(
            object message,
            string tag = "EDITOR",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            if (!EnableEditorLog || !EnableLog)
                return;
            var info = EditorMessageFormat(message, tag, invoker, callerFilePath, callerLineNumber);
            Debug.LogFormat(string.Format("<color=#4E6EF2>{0}</color>", info));
        }
        #endregion
    }
}

