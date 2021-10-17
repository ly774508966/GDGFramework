using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GDG.ModuleManager;
using UnityEngine;
namespace GDG.Utils
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
            if (LogManager.EnableConsoleLog)
                throw new Exception(info);
        }
    }
}