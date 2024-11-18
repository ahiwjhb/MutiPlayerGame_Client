#nullable enable
#nullable enable
using Network.Protocol;
using System;
using UnityEngine;

namespace Logger
{
    internal class DebugLogger : ILogger
    {
        public LogLevel Level { get; set; }

        public DebugLogger() {
            Level = LogLevel.Log | LogLevel.Warring | LogLevel.Error;
        }

        void ILogger.ErrorFormart_Internal(string format, params object[] args) {
            Debug.LogErrorFormat(format, args);
        }

        void ILogger.Error_Internal(string info, Exception? e) {
            Debug.LogError($"{info} {e?.Message}\n{e}");
        }

        void ILogger.LogFormat_Internal(string format, params object[] args) {
            if (args.Length == 2 && args[1].ToString() == typeof(SyncPlayerPositionRequest).Name) return;
            Debug.LogFormat(format, args);
        }

        void ILogger.Log_Internal(string info) {
            Debug.Log(info);
        }

        void ILogger.WarringFormat_Internal(string format, params object[] args) {
            Debug.LogWarningFormat(format, args);
        }

        void ILogger.Warring_Internal(string info) {
            Debug.LogWarning(info);
        }
    }
}
