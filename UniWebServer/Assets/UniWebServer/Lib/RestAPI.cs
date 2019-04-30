using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UniWebServer
{

    using HandlerFunc = System.Action<Request, Response>;
    [RequireComponent(typeof(EmbeddedWebServerComponent))]
    public class RestAPI : MonoBehaviour, IWebResource
    {
        protected string prefix = "/api";

        protected Dictionary<string, HandlerFunc> endpoints = new Dictionary<string, HandlerFunc> ();

        EmbeddedWebServerComponent server;

        protected virtual void Start ()
        {
            // Add the API prefix ("/api", by default) to the webserver.
            // Any paths/patterns below that (e.g. /api/endpoint1/arg)
            // will be handled by our `endpoints` dictionary
            server = GetComponent<EmbeddedWebServerComponent>();
            server.AddResource(prefix, this);
        }
	
        public void HandleRequest (Request request, Response response)
        {
            response.statusCode = 200;
			response.message = "OK.";

            string shortUrl = request.uri.LocalPath.Replace(prefix, "");
            bool endpointFound = false;
            // Look through endpoints for any that match the url we received.
            foreach(KeyValuePair<string, HandlerFunc> entry in endpoints) {
                Match match = Regex.Match(shortUrl, entry.Key);
                // By testing for endpointFound, we guarantee we'll only run the
                // *first* matching endpoint; Be careful how these are added, since
                // some might override others.
                if (!endpointFound && match.Success)
                {
                    endpointFound = true;
                    entry.Value(request, response);
                }
            }

            // If none of our endpoints matched, send 404
            if (!endpointFound){
                response.Write404(request.uri.LocalPath);
            }
        }

    }
}

