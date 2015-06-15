using System;
using System.Windows.Forms;
using Fiddler;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;

[assembly: Fiddler.RequiredVersion("4.5.1.0")]

namespace FFRKScan
{
    public class FFRKScanAutoTamper : IAutoTamper, IFeatureHub    // Ensure class is public, or Fiddler won't see it!
    {
        const string Host = "ffrk.denagames.com";
        const string JsonMIMEType = "json";
        public const string Name = "FFRKScan";

        List<IFeature> features = new List<IFeature>();
        IUserInterfacePolicy userInterfacePolicy;

        public FFRKScanAutoTamper() : this(new FiddlerUIPolicy()) { }

        public FFRKScanAutoTamper(IUserInterfacePolicy userInterfacePolicy)
        {
            this.userInterfacePolicy = userInterfacePolicy;
        }

        public T GetFeature<T>() where T : class
        {
            foreach (var item in features)
            {
                var result = item as T;

                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public void OnLoad()
        {
            LoadFeatures();

            userInterfacePolicy.Initialize(Name, features);
        }

        private void LoadFeatures()
        {
            foreach (var eachType in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (eachType.GetInterface("IFeature") != null && eachType.GetConstructor(Type.EmptyTypes) != null && eachType.IsAbstract == false)
                {
                    var newFeature = Activator.CreateInstance(eachType) as IFeature;
                    features.Add(newFeature);
                }
            }

            features.Sort((a, b) => String.Compare(a.Name, b.Name));

            foreach (var item in features)
            {
                item.Initialize(this);
            }
        }

        public void OnBeforeUnload()
        {
        }

        public void AutoTamperRequestBefore(Session oSession)
        {
        }

        public void AutoTamperRequestAfter(Session oSession)
        {
            if (!oSession.oRequest.host.Equals(Host, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            Request? request = null;
            string requestPath = oSession.oRequest.headers.RequestPath;

            foreach (IFeature feature in this.features)
            {
                try
                {
                    if (feature.CanInspectRequest(requestPath))
                    {
                        if (!request.HasValue)
                        {
                            request = new Request
                            {
                                RequestPath = requestPath,
                                Body = oSession.GetRequestBodyAsString()
                            };
                        }

                        feature.InspectRequest(request.Value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteException(requestPath, ex);
                }
            }
        }

        public void AutoTamperResponseBefore(Session oSession)
        {
        }

        public void AutoTamperResponseAfter(Session oSession)
        {
            if (!oSession.oRequest.host.Equals(Host, StringComparison.CurrentCultureIgnoreCase))
            {
                return;
            }

            Logger.Write(oSession.oRequest.headers.RequestPath, new object[0]);

            if (!oSession.oResponse.MIMEType.Contains(JsonMIMEType))
            {
                return;
            }

            Response? response = null;
            string requestPath = oSession.oRequest.headers.RequestPath;

            foreach (IFeature feature in this.features)
            {
                try
                {
                    if (feature.CanInspectResponse(requestPath))
                    {
                        if (!response.HasValue)
                        {
                            response = new Response
                            {
                                Request = new Request
                                {
                                    RequestPath = requestPath,
                                    Body = oSession.GetRequestBodyAsString()
                                },
                                Body = oSession.GetResponseBodyAsString()
                            };
                        }

                        feature.InspectResponse(response.Value);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteException(requestPath, ex);
                }
            }
        }

        public void OnBeforeReturningError(Session oSession) { }

        public IEnumerable<T> GetFeaturesByInterface<T>()
        {
            return this.features.Where(each => typeof(T).IsAssignableFrom(each.GetType())).Select(each => (T)each);
        }

        public IEnumerable<IFeature> GetFeatures()
        {
            return this.features;
        }
    }
}
