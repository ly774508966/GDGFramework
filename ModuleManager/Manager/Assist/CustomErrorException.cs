using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace GDG.ModuleManager
{
    public class CustomErrorException : Exception
    {
        private string FilePath { get => $"{ UserFileManager.Path}/Logger/UnityLogger.txt"; }
        public CustomErrorException(
            string message,
            string tag = "ERROR",
            [CallerMemberNameAttribute] string invoker = "unknown",
            [CallerFilePath] string callerFilePath = "unknown",
            [CallerLineNumber] int callerLineNumber = -1
            )
        {
            var logMgr = LogManager.Instance;

            var info = logMgr.MessageFormat(message, tag, invoker, callerFilePath, callerLineNumber);
            if (logMgr.EnableConsoleLog)
                throw new Exception(info);
        }
    }
}