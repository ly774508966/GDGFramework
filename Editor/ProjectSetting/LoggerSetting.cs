using System.Collections;
using System.Collections.Generic;
using GDG.Config;
using GDG.ModuleManager;
using GDG.Utils;
using UnityEditor;
using UnityEngine;


partial class ProjectSetting
{
    private class LogHandle
    {
        public bool EnableLog;
        public bool EnableConsoleLog;
        public bool EnableRuntimeLog;
        public bool EnableEditorLog;
        public bool EnableEventLog;
        public bool EnableWriteIntoLogFile;
        public bool EnableTime;
        public bool EnableFilePath;
        public bool LogErrorOrThrowException;
        public float LoggerMaxMBSize;

        public LogHandle(bool enableLog, bool enableConsoleLog, bool enableRuntimeLog, bool enableEditorLog, bool enableEventLog, bool enableWriteIntoLogFile, bool enableTime, bool enableFilePath, bool logErrorOrThrowException, float loggerMaxMBSize)
        {
            EnableLog = enableLog;
            EnableConsoleLog = enableConsoleLog;
            EnableRuntimeLog = enableRuntimeLog;
            EnableEditorLog = enableEditorLog;
            EnableEventLog = enableEventLog;
            EnableWriteIntoLogFile = enableWriteIntoLogFile;
            EnableTime = enableTime;
            EnableFilePath = enableFilePath;
            LogErrorOrThrowException = logErrorOrThrowException;
            LoggerMaxMBSize = loggerMaxMBSize;
        }
    }
    private string m_LoggerConfigPath = Configurations.ConfigPath + "\\LoggerConfig.json";
    private static bool s_IsEnableLog { get => LogManager.EnableLog; set => LogManager.EnableLog = value; }
    public static bool s_IsEnableConsoleLog { get => LogManager.EnableConsoleLog; set => LogManager.EnableConsoleLog = value; }
    public static bool s_IsEnableRuntimeLog { get => LogManager.EnableRuntimeLog; set => LogManager.EnableRuntimeLog = value; }
    public static bool s_IsEnableEditorLog { get => LogManager.EnableEditorLog; set => LogManager.EnableEditorLog = value; }
    public static bool s_IsEnableEventLog { get => EventManager.EnableEventLog; set => EventManager.EnableEventLog = value; }
    public static bool s_IsEnableWriteIntoLogFile { get => LogManager.EnableWriteIntoLogFile; set => LogManager.EnableWriteIntoLogFile = value; }
    public static bool s_IsEnableTime { get => LogManager.EnableTime; set => LogManager.EnableTime = value; }
    public static bool s_IsEnableFilePath { get => LogManager.EnableFilePath; set => LogManager.EnableFilePath = value; }
    public static bool s_LogErrorOrThrowException { get => LogManager.LogErrorOrThrowException; set => LogManager.LogErrorOrThrowException = value; }
    public static float s_LoggerMaxMBSize { get => LogManager.LoggerMaxMBSize; set => LogManager.LoggerMaxMBSize = value; }

    public void SaveLoggerConfig()
    {
        var LogHandle = new LogHandle(s_IsEnableLog, s_IsEnableConsoleLog, s_IsEnableRuntimeLog,
        s_IsEnableEditorLog, s_IsEnableEventLog, s_IsEnableWriteIntoLogFile, s_IsEnableTime, s_IsEnableFilePath,
        s_LogErrorOrThrowException, s_LoggerMaxMBSize);
        JsonManager.SaveData<LogHandle>(LogHandle, m_LoggerConfigPath);
        AssetDatabase.Refresh();
        Log.Sucess("Save Succesfully !");
    }
    public void LoadLoggerConfig()
    {
        var logConfig = JsonManager.LoadData<LogHandle>(m_LoggerConfigPath);

        if (logConfig == null)
            return;

        s_IsEnableLog = logConfig.EnableLog;
        s_IsEnableConsoleLog = logConfig.EnableConsoleLog;
        s_IsEnableRuntimeLog = logConfig.EnableRuntimeLog;
        s_IsEnableEditorLog = logConfig.EnableEditorLog;
        s_IsEnableEventLog = logConfig.EnableEventLog;
        s_IsEnableWriteIntoLogFile = logConfig.EnableWriteIntoLogFile;
        s_IsEnableTime = logConfig.EnableTime;
        s_IsEnableFilePath = logConfig.EnableFilePath;
        s_LogErrorOrThrowException = logConfig.LogErrorOrThrowException;
        s_LoggerMaxMBSize = logConfig.LoggerMaxMBSize;
    }
    private void DrawLogger()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            using (var enableLog = new EditorGUILayout.ToggleGroupScope("Enable Log", s_IsEnableLog))
            {
                s_IsEnableLog = enableLog.enabled;
                s_IsEnableConsoleLog = EditorGUILayout.Toggle("Enable ConsoleLog", s_IsEnableConsoleLog);
                s_IsEnableRuntimeLog = EditorGUILayout.Toggle("Enable RuntimeLog", s_IsEnableRuntimeLog);
                s_IsEnableEditorLog = EditorGUILayout.Toggle("Enable EditorLog", s_IsEnableEditorLog);
                using (new EditorGUILayout.HorizontalScope())
                {
                    s_IsEnableWriteIntoLogFile = EditorGUILayout.Toggle("Enable Write Into LogFile", s_IsEnableWriteIntoLogFile, GUILayout.ExpandWidth(false));
                    GUILayout.Space(30);
                    if(s_IsEnableWriteIntoLogFile)
                    {
                        EditorGUILayout.LabelField("MaxSize(MB): ", GUILayout.Width(85));
                        s_LoggerMaxMBSize = EditorGUILayout.DelayedFloatField(s_LoggerMaxMBSize, GUILayout.Width(30));
                    }
                }
                GUILayout.Label($"—————————————————————————————");
                s_IsEnableTime = EditorGUILayout.Toggle("Enable Time", s_IsEnableTime);
                s_IsEnableFilePath = EditorGUILayout.Toggle("Enable FilePath", s_IsEnableFilePath);
                s_LogErrorOrThrowException = EditorGUILayout.Toggle("LogError/ThrowException", s_LogErrorOrThrowException);
                GUILayout.Label($"—————————————————————————————");
                s_IsEnableEventLog = EditorGUILayout.Toggle("Enable EventLog", s_IsEnableEventLog);
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Apply"))
            {
                SaveLoggerConfig();
                AssetDatabase.Refresh();
            }
        }
    }
}
