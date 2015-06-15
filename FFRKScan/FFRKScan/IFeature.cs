using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FFRKScan
{
    public interface IFeatureHub
    {
        T GetFeature<T>() where T : class;
        IEnumerable<T> GetFeaturesByInterface<T>();
        IEnumerable<IFeature> GetFeatures();
    }

    public struct Request
    {
        public string RequestPath { get; set; }
        public string Body { get; set; }

        public T DeserializeBody<T>()
        {
            return JsonConvert.DeserializeObject<T>(Body);
        }
    }

    public struct Response
    {
        public Request Request { get; set; }
        public string Body { get; set; }

        public T DeserializeBody<T>()
        {
            return JsonConvert.DeserializeObject<T>(Body);
        }
    }

    public interface IFeature
    {
        string Name { get; }

        void Initialize(IFeatureHub hub);
        Control CreateControl();

        bool CanInspectRequest(string requestPath);
        void InspectRequest(Request request);

        bool CanInspectResponse(string requestPath);
        void InspectResponse(Response response);
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class TamperAttribute : Attribute
    {
        readonly string requestPath;

        public TamperAttribute(string requestPath)
        {
            this.requestPath = requestPath;
        }

        public string RequestPath
        {
            get { return requestPath; }
        }
    }

    public abstract class Feature : IFeature, INotifyPropertyChanged
    {
        public struct Handler
        {
            public string Name { get; set; }
            public string RequestPath { get; set; }
            public Action<Request> InspectRequest { get; set; }
            public Action<Response> InspectResponse { get; set; }

            public bool CanInspectRequest(string requestPath)
            {
                return RequestPath.Equals(requestPath, StringComparison.OrdinalIgnoreCase) && InspectRequest != null;
            }

            public bool CanInspectResponse(string requestPath)
            {
                return RequestPath.Equals(requestPath, StringComparison.OrdinalIgnoreCase) && InspectResponse != null;
            }
        }

        public IFeatureHub FeatureHub { get; private set; }
        List<Handler> handlers = new List<Handler>();
        public string Name { get; set; }

        public Feature(string name)
        {
            this.Name = name;
        }

        private void InitializeHandlers()
        {
            var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var item in methods)
            {
                var attr = Attribute.GetCustomAttribute(item, typeof(TamperAttribute)) as TamperAttribute;

                if (attr == null)
                {
                    continue;
                }

                var methodParams = item.GetParameters();

                if (methodParams.Length == 1 && methodParams[0].ParameterType == typeof(Response))
                {
                    var action = (Action<Response>)Delegate.CreateDelegate(typeof(Action<Response>), this, item);
                    AddHandler(new Handler { Name = this.Name, RequestPath = attr.RequestPath, InspectResponse = action });
                }
                else if (methodParams.Length == 1 && methodParams[0].ParameterType == typeof(Request))
                {
                    var action = (Action<Request>)Delegate.CreateDelegate(typeof(Action<Request>), this, item);
                    AddHandler(new Handler { Name = this.Name, RequestPath = attr.RequestPath, InspectRequest = action });
                }
            }
        }

        protected virtual void AddHandler(Handler h)
        {
            handlers.Add(h);
        }

        public virtual Control CreateControl()
        {
            return null;
        }

        public virtual bool CanInspectResponse(string requestPath)
        {
            return handlers.Any(each => each.CanInspectResponse(requestPath));
        }

        public virtual void InspectResponse(Response response)
        {
            foreach (var item in handlers.Where(each => each.CanInspectResponse(response.Request.RequestPath)))
            {
                try
                {
                    item.InspectResponse(response);
                }
                catch (Exception ex)
                {
                    Logger.WriteException(string.Format("{0} : {1}", item.Name, response.Request.RequestPath), ex);
                }
            }
        }

        public virtual bool CanInspectRequest(string requestPath)
        {
            return handlers.Any(each => each.CanInspectRequest(requestPath));
        }

        public virtual void InspectRequest(Request request)
        {
            foreach (var item in handlers.Where(each => each.CanInspectRequest(request.RequestPath)))
            {
                try
                {
                    item.InspectRequest(request);
                }
                catch (Exception ex)
                {
                    Logger.WriteException(string.Format("{0} : {1}", item.Name, request.RequestPath), ex);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                try
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
                catch (Exception ex)
                {
                    Logger.WriteException(string.Format("{0} : {1}", Name, propertyName), ex);
                }
            }
        }

        public virtual void Initialize(IFeatureHub hub)
        {
            this.FeatureHub = hub;

            InitializeHandlers();
        }
    }
}
