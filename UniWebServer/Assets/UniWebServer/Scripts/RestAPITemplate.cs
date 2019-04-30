using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UniWebServer
{
    using HandlerFunc = System.Action<Request, Response>;
    [RequireComponent(typeof(EmbeddedWebServerComponent))]

    public class RestAPITemplate : RestAPI
    {
        protected override void Start ()
        {
            // NOTE: if you want to change the API prefix (default: "/api")
            // do that here, e.g.:
            // prefix = "/otherApi";

            base.Start();
            
            HandlerFunc Endpoint1Handler = (request, response) => 
            {   
                response.SetJSONHeader();
                string jsonString = @"{""value"":2, ""other"":""a_string""}";
                response.Write(jsonString);           
            };

            HandlerFunc Endpoint2Handler = (request, response) => 
            {   
                // TODO: get arg from URL
                // TODO: get payload data from POST/PUT calls
                response.SetJSONHeader();
                string jsonString = @"{""ENDPOINT #"":2, ""other"":""string with invalid \n newlines""}";
                response.Write(jsonString);
            };

            endpoints.Add(@"/endpoint1", Endpoint1Handler);
            endpoints.Add(@"/endpoint2/(?<intArg>\d+)", Endpoint2Handler);
        }

    }

}
