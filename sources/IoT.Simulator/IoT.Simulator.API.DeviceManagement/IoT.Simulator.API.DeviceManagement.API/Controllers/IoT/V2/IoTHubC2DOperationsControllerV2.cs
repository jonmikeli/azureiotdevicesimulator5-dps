﻿using AutoMapper;

using IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT;
using IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests;
using IoT.Simulator.API.DeviceManagement.Services.Contracts;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;//WARNING: this reference should not be done. It has been included only for JobResponse and save time for the project.

using Newtonsoft.Json;

using System;
using System.Threading.Tasks;

using SIoT = IoT.Simulator.API.DeviceManagement.Services.Model.IoT;



namespace IoT.Simulator.API.DeviceManagement.API.Controllers.IoT.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/devices")]
    public class IoTHubC2DOperationsControllerV2 : Controller
    {
        private readonly IIoTHubC2DOperationsService _iotService;
        private readonly IMapper _mapper;

#pragma warning disable CS1591
        public IoTHubC2DOperationsControllerV2(IIoTHubC2DOperationsService iotService, IMapper mapper)
        {
            _iotService = iotService;
            _mapper = mapper;
        }
#pragma warning restore CS1591

        #region GET
        [HttpGet("{id}/twin")]
        public async Task<Twins> Get(string id)
        {
            var data = await _iotService.GetTwinsAsync(id);

            if (data != null)
                return _mapper.Map<Twins>(data);
            else
                return null;
        }
        #endregion

        #region PUT
        [HttpPut()]
        [Route("invoke")]
        public async Task<string> InvokeDirectMethodAsync([FromBody] DirectMethodInvoqueRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            return await _iotService.InvokeDirectMethodAsync(request.DeviceId, request.MethodName, request.Payload);
        }

        [HttpPut()]
        [Route("twin/tags/update")]
        public async Task<Twins> UpdateTwinTagsAsync([FromBody] TwinUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.TwinTags == null)
                throw new ArgumentNullException("Tags");

            var businessRequestTags = _mapper.Map<SIoT.Tags>(request.TwinTags);

            if (businessRequestTags != null)
            {
                SIoT.Twins data = await _iotService.UpdateTwinsAsync(request.DeviceId, JsonConvert.SerializeObject(new { tags = businessRequestTags }));

                if (data != null)
                    return _mapper.Map<Twins>(data);
                else
                    return null;
            }
            else
                return null;

        }

        [HttpPut()]
        [Route("twin/properties/update")]
        public async Task<Twins> UpdateTwinPropertiesAsync([FromBody] TwinUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.TwinProperties == null)
                throw new ArgumentNullException("Properties");

            var businessRequestProperties = _mapper.Map<SIoT.Properties>(request.TwinProperties);

            if (businessRequestProperties != null)
            {
                SIoT.Twins data = await _iotService.UpdateTwinsAsync(request.DeviceId, JsonConvert.SerializeObject(new { properties = businessRequestProperties }));

                if (data != null)
                    return _mapper.Map<Twins>(data);
                else
                    return null;
            }
            else
                return null;
        }

        [HttpPut()]
        [Route("twin/properties/jobs/run")]
        public async Task<string> RunTwinPropertiesUpdateJobAsync([FromBody] TwinUpdateJobRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.TwinProperties == null)
                throw new ArgumentNullException("Properties");

            string jobId = Guid.NewGuid().ToString();

            var twin = new SIoT.Twins();
            twin.Properties = _mapper.Map<SIoT.Properties>(request.TwinProperties);

            if (twin.Properties != null)
            {

                //Send Twins but build new Twin Azure object at service level.
                var serviceRequest = new SIoT.TwinsSearchRequest();
                serviceRequest.WhereCondition = request.WhereConstraint;

                await _iotService.RunTwinUpdateJobAsync(jobId, request.WhereConstraint, twin);

                return jobId;
            }
            else return string.Empty;

        }

        [HttpPut()]
        [Route("twin/tags/jobs/run")]
        public async Task<string> RunTwinTagsUpdateJobAsync([FromBody] TwinUpdateJobRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.TwinTags == null)
                throw new ArgumentNullException("Tags");

            string jobId = Guid.NewGuid().ToString();

            var twin = new SIoT.Twins();
            twin.Tags = _mapper.Map<SIoT.Tags>(request.TwinTags);

            if (twin.Tags != null)
            {

                //Send Twins but build new Twin Azure object at service level.
                var serviceRequest = new SIoT.TwinsSearchRequest();
                serviceRequest.WhereCondition = request.WhereConstraint;

                await _iotService.RunTwinUpdateJobAsync(jobId, request.WhereConstraint, twin);

                return jobId;
            }
            else return string.Empty;

        }

        [HttpGet()]
        [Route("twin/jobs/{jobId}")]
        public async Task<JobResponse> MonitorJobWithDetailsAsync(string jobId)
        {
            if (string.IsNullOrEmpty(jobId))
                throw new ArgumentNullException("jobId");

            return await _iotService.MonitorJobWithDetailsAsync(jobId);
        }

        #endregion
    }
}
