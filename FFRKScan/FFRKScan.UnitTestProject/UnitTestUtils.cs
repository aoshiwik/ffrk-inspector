using Fiddler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKScan.UnitTestProject
{
    static class UnitTestUtils
    {
        public static void SimulateResponse(FFRKScanAutoTamper proxy, string filePath, string requestPath)
        {
            var content = File.ReadAllBytes(filePath);

            var session = Simulation.CreateFiddlerSession(requestPath, content);

            proxy.AutoTamperResponseAfter(session);
        }
    }
}
