using AutoMapper;

using IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT;
using IoT.Simulator.API.DeviceManagement.API.DataContracts.IoT.Requests;
using IoT.Simulator.API.DeviceManagement.Services.Contracts;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Text.Json;
using System.Threading.Tasks;

using SIoT = IoT.Simulator.API.DeviceManagement.Services.Model.IoT;

namespace IoT.Simulator.API.DeviceManagement.API.Controllers.IoT.V1
{
    /// <summary>
    /// Controller containing device management related features (create, get, update, delete) and device properties oriented endpoints.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/devices")]//required for default versioning
    [Route("api/v{version:apiVersion}/devices")]
    public class DevicesControllerV1 : Controller
    {
        private readonly IDeviceManagementService _provisioningService;
        private readonly IMapper _mapper;

#pragma warning disable CS1591
        public DevicesControllerV1(IDeviceManagementService provisioningService, IMapper mapper)
        {
            _provisioningService = provisioningService;
            _mapper = mapper;
        }
#pragma warning restore CS1591

        #region GET
        /// <summary>
        /// Gets an IoT Hub device by its Id
        /// </summary>
        /// <param name="id">Device Id</param>
        /// <returns>Device entity containing device related information</returns>
        /// <response code="200">Returns the item corresponding to the provided Id.</response>
        /// <response code="204">If the item is null.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Device))]
        [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Device))]
        [HttpGet("{id}")]
        public async Task<Device> GetDevice(string id)
        {
            var data = await _provisioningService.GetDeviceAsync(id);

            if (data != null)
                return _mapper.Map<Device>(data);
            else
                return null;
        }

        /// <summary>
        /// Gets IoT Hub's registered devices
        /// </summary>
        /// <remarks>It seems that Newtonsoft's JArrays are not serialized properly in the last versions of .NET. This endpoint has been kept for educational purposes. Another one has been added solving the issue but adding additional dependencies.</remarks>
        /// <param name="maxCount">Max number of returned items</param>
        /// <returns>A collection of IoT Hub's registered devices</returns>
        [HttpGet("newtonsoft")]
        public async Task<JArray> GetDevicesAsync([FromQuery] int maxCount = 100)
        {
            return await _provisioningService.GetDevicesAsync(maxCount);
        }

        /// <summary>
        /// Gets IoT Hub's registered devices
        /// </summary>
        /// <remarks>This version of the GetDevices endpoint has been added to provide an alternative to the serialization issue of Newtonsoft's Jarrays in the last versions of .NET</remarks>
        /// <param name="maxCount">Max number of returned items</param>
        /// <returns>A collection of IoT Hub's registered devices</returns>
        [HttpGet("systemtext")]
        public async Task<JsonDocument> GetDevices2Async([FromQuery] int maxCount = 100)
        {
            return await _provisioningService.GetDevices2Async(maxCount);
        }

        /// <summary>
        /// Gets device's primary key or thumbprint, depending on Authentication Type.
        /// </summary>
        /// <param name="id">Device Id</param>
        /// <returns>Primary key or thumbprint</returns>
        [HttpGet()]
        [Route("{id}/key1")]
        public async Task<string> GetPrimaryKeyOrThumbprint(string id)
        {
            return await _provisioningService.GetPrimaryKeyOrThumbprintFromDeviceAsync(id);
        }

        /// <summary>
        /// Gets device's secondary key or thumbprint, depending on Authentication Type.
        /// </summary>
        /// <param name="id">Device Id</param>
        /// <returns>Secondary key or thumbprint</returns>
        [HttpGet()]
        [Route("{id}/key2")]
        public async Task<string> GetSecondaryKeyOrThumbprint(string id)
        {
            return await _provisioningService.GetSecondaryKeyOrThumbprintFromDeviceAsync(id);
        }
        #endregion

        #region POST
        /// <summary>
        /// Registers a device within a given IoT Hub
        /// </summary>       
        /// <remarks>
        /// The device is created and registered with sent properties.
        /// </remarks>
        /// <param name="value">Device provision settings</param>
        /// <returns>Device entity containing device related information</returns>
        /// <response code="201">Returns the newly created item.</response>s
        [HttpPost]
        [Route("addwithtags")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Device))]
        public async Task<Device> AddDeviceWithTags([FromBody] ProvisionDeviceWithTagsRequest value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            //NOTE: in this implementation, we chose to consider device options as mandatory in order to guarantee tags coherence
            if (value.Tags == null)
                throw new ArgumentNullException("Tags");

            var data = await _provisioningService.AddDeviceWithTagsAsync(value.DeviceId, JsonConvert.SerializeObject(value.Tags));

            if (data != null)
                return _mapper.Map<Device>(data);
            else
                return null;

        }

        /// <summary>
        /// Searches devices within a given IoT Hub, according to given search criteria
        /// </summary>
        /// <param name="request">Search criteria</param>
        /// <returns>Collection of devices fulfilling search criteria</returns>
        /// <response code="200">Collection of devices fulfilling search criteria.</response>s
        [HttpPost()]
        [Route("search")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(JArray))]
        public async Task<JArray> SearchDevicesAsync([FromBody] SearchDevicesRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (string.IsNullOrEmpty(request.Query))
                throw new ArgumentNullException("request.Query");

            if (request.MaxCount <= 0)
                throw new ArgumentException("MaxCount has to be greater than 0.", "request.MaxCount");

            return await _provisioningService.GetDevicesAsync(request.Query, request.MaxCount);
        }

        /// <summary>
        /// Registers a device module within a given IoT Hub
        /// </summary>       
        /// <remarks>
        /// The module is created and registered with no properties.
        /// </remarks>        
        /// <returns>Module entity containing device related information</returns>
        /// <response code="201">Returns the newly created item.</response>s
        [HttpPost]
        [Route("modules/add")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Device))]
        public async Task<Module> AddModuleToDevice([FromBody] ProvisionModuleRequest value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var data = await _provisioningService.AddModuleAsync(value.DeviceId, value.ModuleId);

            if (data != null)
                return _mapper.Map<Module>(data);
            else
                return null;

        }
        #endregion

        #region PUT
        /// <summary>
        /// Disables a device within a given IoT Hub
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns>Device entity containing device related information (with the updated status)</returns>
        /// <response code="200">Disabled device.</response>
        [HttpPut()]
        [Route("{id}/disable")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Device))]
        public async Task<Device> DisableDevice(string id)
        {
            var data = await _provisioningService.DisableDeviceAsync(id);

            if (data != null)
                return _mapper.Map<Device>(data);
            else
                return null;
        }

        /// <summary>
        /// Enables a device within a given IoT Hub
        /// </summary>
        /// <param name="id">Device Id</param>
        /// <returns>Device entity containing device related information</returns>
        /// <response code="200">Disabled device.</response>
        [HttpPut()]
        [Route("{id}/enable")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Device))]
        public async Task<Device> EnableDevice(string id)
        {
            var data = await _provisioningService.EnableDeviceAsync(id);

            if (data != null)
                return _mapper.Map<Device>(data);
            else
                return null;
        }
        #endregion

        #region DELETE
        /// <summary>
        /// Deletes a device
        /// </summary>
        /// <param name="id">Device Id</param>
        /// <returns>Boolean illustrating if the delete action has been successful or not</returns>
        /// <response code="200">Boolean illustrating if the delete action has been successful or not.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        public async Task<bool> DeleteDevice(string id)
        {
            return await _provisioningService.RemoveDeviceAsync(id);
        }
        #endregion
    }
}
