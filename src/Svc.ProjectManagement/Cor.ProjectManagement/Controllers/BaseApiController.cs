// Controllers/BaseApiController.cs
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Cor.ProjectManagement.Controllers
{
   [ApiController]
   [Route("api/projectmanagement/v{version:apiVersion}/[controller]")]
   [ApiVersion("1.0")]
   [Authorize]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()
            ?? throw new InvalidOperationException("IMediator service not found");
    }
}