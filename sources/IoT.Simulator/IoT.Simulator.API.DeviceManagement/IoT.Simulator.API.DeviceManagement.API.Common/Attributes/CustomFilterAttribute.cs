using Microsoft.AspNetCore.Mvc.Filters;

namespace IoT.Simulator.API.DeviceManagement.API.Common.Attributes
{
    public class CustomFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            //TODO: actions to implement
        }
    }
}
