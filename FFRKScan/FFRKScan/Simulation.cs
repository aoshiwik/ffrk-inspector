using Fiddler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKScan
{
    public static class Simulation
    {
        public static Session CreateFiddlerSession(string requestPath, byte[] content)
        {
            var requestHeaders = new HTTPRequestHeaders(requestPath, new[] { "Host: ffrk.denagames.com" });
            var session = new Session(requestHeaders, null);
            session.oResponse.headers = new HTTPResponseHeaders(0, new[] { "Content-Type: application/json; charset=utf-8" });
            session.responseBodyBytes = content;
            return session;
        }
    }
}
