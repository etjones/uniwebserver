using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using System;

namespace UniWebServer
{


    public class EmbeddedWebServerComponent : MonoBehaviour
    {
        public bool startOnAwake = true;
        public int port = 8079;
        public int workerThreads = 2;
        public bool processRequestsInMainThread = true;
        public bool logRequests = true;

        protected WebServer server;
        Dictionary<string, IWebResource> resources = new Dictionary<string, IWebResource> ();

        void Start ()
        {
            if (processRequestsInMainThread)
                Application.runInBackground = true;
            server = new WebServer (port, workerThreads, processRequestsInMainThread);
            server.logRequests = logRequests;
            server.HandleRequest += HandleRequest;
            if (startOnAwake) {
                server.Start ();
            }
        }

        void OnApplicationQuit ()
        {
            server.Dispose ();
        }

        void Update ()
        {
            if (server.processRequestsInMainThread) {
                server.ProcessRequests ();    
            }
        }

        void HandleRequest (Request request, Response response)
        {
            if (resources.ContainsKey (request.uri.LocalPath)) {
                try {
                    resources [request.uri.LocalPath].HandleRequest (request, response);
                } catch (Exception e) {
                    response.statusCode = 500;
                    response.Write (e.Message);
                }
            } else {
                // Call the appropriate handling method if our URL _starts_
                // with any of our the resource keys. This enables the Rest API
                // for any URLs under its prefix. (e.g. /api/endpoint1, /api/endpoint2/arg)
                // This could cause problems depending on the endpoints defined;
                // for now, try it out. -- 20190430
                // FIXME: this might get in the way of serving files, right?
                bool foundEndpoint = false;
                foreach(KeyValuePair<string, IWebResource> entry in resources) {
                    // do something with entry.Value or entry.Key
                    if (!foundEndpoint && request.uri.LocalPath.StartsWith(entry.Key)){
                        foundEndpoint = true;
                        try {
                            entry.Value.HandleRequest (request, response);
                        } catch (Exception e) {
                            response.statusCode = 500;
                            response.Write (e.Message);
                        }        
                    }
                }                
                if (!foundEndpoint){
                    response.statusCode = 404;
                    response.message = "Not Found.";
                    response.Write (request.uri.LocalPath + " not found.");
                }
            }
        }

        public void AddResource (string path, IWebResource resource)
        {
            resources [path] = resource;
        }

    }



}