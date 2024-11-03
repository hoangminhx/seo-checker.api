using MediatR;
using SEOChecker.Application.DTOs.SEO;

namespace SEOChecker.Application.Queries.SEO.GetResultIndexes
{
    public record GetResultIndexesQuery(SEORequestDto request) : IRequest<SEOResponseDto>;
}
