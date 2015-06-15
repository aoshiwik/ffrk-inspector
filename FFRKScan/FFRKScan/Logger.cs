
using Fiddler;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFRKScan
{
    static class Logger
    {
        public static void Write(string sMsg)
        {
            FiddlerApplication.Log.LogString(FFRKScanAutoTamper.Name + " - " + sMsg);
        }

        public static void Write(string format, params object[] args)
        {
            FiddlerApplication.Log.LogFormat(FFRKScanAutoTamper.Name + " - " + format, args);
        }

        public static void WriteException(string sMsg, Exception ex)
        {
            Write("{0}\r\n\r\n{1}", new object[] { sMsg, ex.ToString() });
        }
    }
}
