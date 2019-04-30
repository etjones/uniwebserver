using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UniWebServer
{
    using HandlerFunc = System.Action<Request, Response>;
    [RequireComponent(typeof(EmbeddedWebServerComponent))]

    public class RestAPITemplate : RestAPI
    {
        /*
        To add custom REST API to your Unity App: 
        -- Make a copy of this RestAPITemplate class for your own app (You could customize this too if you wanted)
        -- Define functions inside Start() for each endpoint; see GETExampleHandler & POSTExampleHandler below
        -- Add a (regexStr, handler) pair to the `endpoints` Dictionary. (see endpoints.Add(), below)
        -- Add a UniWebviewServer object to your Unity Project
        -- Add your RestAPI class as a component of the Server object in your Unity Scene
        -- Make GET/POST calls to, e.g. http://<unityAppIPAddress>:<port>/api/endpoint1

        This is the barest of bare-bones web servers. We don't handle 
        things like form-encoded arguments, authentication, encryption,
        argument validation, etc. But... if you wanted a fully functioning
        web server & API, why did you start writing a Unity app? ;-)
        */
        protected override void Start ()
        {
            // NOTE: if you want to change the API prefix (default: "/api")
            // do that here, e.g.:
            // prefix = "/otherApi";

            base.Start();

            HandlerFunc GETExampleHandler = (request, response) => {   
                // Basic example: hit a URL, run a handler function, return some
                // arbitrary JSON.
                response.SetJSONHeader();
                string jsonString = @"{""value"":2, ""other"":""a_string""}";
                response.Write(jsonString);           
            };

            HandlerFunc POSTExampleHandler = (request, response) => {   
                // Get a payload from a POST request, parse it as JSON, and 
                // return a value. Lots of ways to do this, but here's a start.

                // For the moment, assume JSON format. 
                // It would be more robust to obey the `Content-Type` header key in request.headers
                DummyPayload payload = JsonUtility.FromJson<DummyPayload>(request.body);
                // Raise an error if the payload isn't as expected:
                if (payload == null){
                    throw new System.ArgumentException($"Unable to create instance of DummyPayload from JSON string: {request.body}");
                }

                string status = $"Received payload with background_color: {payload.background_color}";
                string jsonString = @"{""result"": """ + status + @"""}";
                response.SetJSONHeader();
                response.Write(jsonString);
            };

            // Note these endpoints strings are evaluated as regexes.
            endpoints.Add(@"/endpoint1", GETExampleHandler);
            endpoints.Add(@"/endpoint2/?", POSTExampleHandler);
        }

    }

}

public class DummyPayload
{
    // See: https://medium.com/@MissAmaraKay/parsing-json-in-c-unity-573d1e339b6f

    /* A valid json payload to create an instance of this would be:
     {    
	    "background_color": "0x112233",
        "block_size": 6.2,
        "welcome_message": "Congratulations! It's an API!"
    }
    */
    public string background_color;
    public double block_size;
    public string welcome_message;

}