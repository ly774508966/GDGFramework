using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace GDG.ModuleManager
{
    public class LogManager : AbsLazySingleton<LogManager>
    {
        public void ConsoleLogInfo(
            object message,
            string tag = "INFO",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            Debug.Log(message);
        }
        public void ConsoleLogError(
            object message,
            string tag = "INFO",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1)
        {
            Debug.Log(message);
        }
    }
}
