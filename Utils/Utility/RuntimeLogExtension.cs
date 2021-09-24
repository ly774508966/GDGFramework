using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GDG.ModuleManager;
using UnityEngine;
namespace GDG.Utils
{
    public static class RuntimeLogExtension
    {
        public static void log(
            this object obj,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.RuntimeLogInfo(message, invoker, callerFilePath, callerLineNumber);
        }
        public static void logsuccess(
            this object obj,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {    
            LogManager.Instance.RuntimeLogSuccess(message, invoker, callerFilePath, callerLineNumber);
        }
        public static void logwarning(
            this object obj,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.RuntimeLogWarning(message, invoker, callerFilePath, callerLineNumber);
        }
        public static void Logerror(
            this object obj,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.RuntimeLogError(message, invoker, callerFilePath, callerLineNumber);
        }
    }
}