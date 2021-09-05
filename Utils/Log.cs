using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GDG.ModuleManager;
using System.Runtime.CompilerServices;
namespace GDG.Utils
{
    public class Log
    {
        public static void Info(
            object message,
            string tag = "INFO",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.LogInfo(message, tag, invoker, callerFilePath, callerLineNumber);
        }
        public static void Sucess(
            object message,
            string tag = "SUCCESS",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.LogSucess(message, tag, invoker, callerFilePath, callerLineNumber);
        }
        public static void Warning(
            object message,
            string tag = "WARNING",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.LogWarning(message, tag, invoker, callerFilePath, callerLineNumber);
        }
        public static void Error(
            object message,
            string tag = "ERROR",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.LogError(message, tag, invoker, callerFilePath, callerLineNumber);
        }
        // public static void EditorLog(
        //     object message,
        //     string tag = "INFO",
        //     [CallerMemberNameAttribute] string invoker = "unknown",
        //     [CallerFilePath] string callerFilePath = "unknown",
        //     [CallerLineNumber] int callerLineNumber = -1)
        // {

        //     LogManager.EditorLogInfo(message, tag, invoker, callerFilePath, callerLineNumber);
        // }
        // public static void EditorLogSuccess(
        //     object message,
        //     string tag = "SUCCESS",
        //     [CallerMemberNameAttribute] string invoker = "unknown",
        //     [CallerFilePath] string callerFilePath = "unknown",
        //     [CallerLineNumber] int callerLineNumber = -1)
        // {
        //     LogManager.EditorLogSuccess(message, tag, invoker, callerFilePath, callerLineNumber);
        // }
        // public static void EditorLogWarning(
        //     object message,
        //     string tag = "WARNING",
        //     [CallerMemberNameAttribute] string invoker = "unknown",
        //     [CallerFilePath] string callerFilePath = "unknown",
        //     [CallerLineNumber] int callerLineNumber = -1)
        // {
        //     LogManager.EditorLogWarning(message, tag, invoker, callerFilePath, callerLineNumber);
        // }
        // public static void EditorLogError(
        //     object message,
        //     string tag = "ERROR",
        //     [CallerMemberNameAttribute] string invoker = "unknown",
        //     [CallerFilePath] string callerFilePath = "unknown",
        //     [CallerLineNumber] int callerLineNumber = -1)
        // {
        //     LogManager.EditorLogError(message, tag, invoker, callerFilePath, callerLineNumber);
        // }
    }
}