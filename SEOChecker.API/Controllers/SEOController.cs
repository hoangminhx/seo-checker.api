using MediatR;
using Microsoft.AspNetCore.Mvc;
using SEOChecker.Application.DTOs.SEO;
using SEOChecker.Application.Queries.SEO.GetResultIndexes;

namespace SEOChecker.API.Controllers
{
    [Route("api/seo")]
    [ApiController]
    public class SEOController : ControllerBase
    {
        private readonly IMediator mediator;

        public SEOController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("result-indexes")]
        public async Task<SEOResponseDto> GetResultIndexes([FromQuery]SEORequestDto requestDto)
        {
            GetResultIndexesQuery query = new GetResultIndexesQuery(requestDto);
            SEOResponseDto response = await mediator.Send(query, HttpContext.RequestAborted);

            return response;
        }
    }
}
