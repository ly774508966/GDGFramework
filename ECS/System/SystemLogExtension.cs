using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GDG.ModuleManager;
using UnityEngine;
namespace GDG.ECS
{
    public static class SystemLogExtension
    {
        public static void log(
            this ISystem system,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.RuntimeLogInfo(message, invoker, callerFilePath, callerLineNumber);
        }
        public static void logsuccess(
            this ISystem system,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {    
            LogManager.Instance.RuntimeLogSuccess(message, invoker, callerFilePath, callerLineNumber);
        }
        public static void logwarning(
            this ISystem system,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.RuntimeLogWarning(message, invoker, callerFilePath, callerLineNumber);
        }
        public static void Logerror(
            this ISystem system,
            object message,
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogManager.Instance.RuntimeLogError(message, invoker, callerFilePath, callerLineNumber);
        }
    }
}