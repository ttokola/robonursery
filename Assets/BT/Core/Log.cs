using UnityEngine;

namespace BT
{
    public static class Log
    {
        [System.Diagnostics.Conditional("LOG_INFO")]
        public static void Info(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        [System.Diagnostics.Conditional("LOG_VERBOSE")]
        public static void Verbose(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }
    }
}
