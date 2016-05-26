﻿using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using DeviceStateNamespace;
using Newtonsoft.Json.Linq;

using StateProcessorService;
using System.Threading.Tasks;

namespace StateManagementServiceWebAPI.Controllers
{
    [RoutePrefix("devices")]
    public class DevicesController : ApiController
    {
        private IStateProcessorRemoting StateProcessorClient = ServiceProxy.Create<IStateProcessorRemoting>(new Uri("fabric:/StateManagementService/StateProcessorService"));

        [Route("{deviceId}")]
        public async Task<IHttpActionResult> Get([FromUri]string deviceId)
        {
            var deviceState = await StateProcessorClient.GetStateAsync(deviceId);

            // When no state the DeviceRepository returns an instance with default values
            // use the DeviceID to test if we have an actual result as that should always be set
            if (deviceState.DeviceID == null)
            {
                return NotFound();
            }
            return Ok(deviceState);
        }

        // PUT devices/{DeviceId} 
        // To call using Swagger UI: http://localhost:9013/swagger/ui/index
        // Or to call from fiddler:
        // Method: PUT
        // Host: http://localhost:9013/devices/{DeviceId}
        // Headers:
        // User-Agent: Fiddler
        // Host: localhost:9013
        // Content-type: application/json
        // Body:
        // {
        //"Xaxis" : "1" ,
        //"Yaxis : "2" ,
        //"Zaxis" : "3"
        // }
        [Route("{deviceId}")]
        public async Task<DeviceState> Put([FromUri]string deviceId, [FromBody]JToken stateValue)
        {
            // TODO: add error handling. return HttpResponseException if StateValue is null (not well formated JSON)
            var deviceState = await StateProcessorClient.CreateStateAsync(deviceId, stateValue.ToString());
            return deviceState;
        }


    }
}
