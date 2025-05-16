using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyBasket.Contracts.Votes;
using SurveyBasket.Extensions;
using SurveyBasket.Services.Questions;
using SurveyBasket.Services.Votes;

namespace SurveyBasket.Controllers;

public class VotesController(IQuestionService questionService, IVoteService voteService, ILogger<VotesController> logger) : ControllerBase
{
    private readonly IQuestionService _questionService = questionService;
    private readonly IVoteService _voteService = voteService;
    private readonly ILogger<VotesController> _logger = logger;

    [HttpGet("")]
    public async Task<IActionResult> Start([FromRoute] int pollId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging starting Vote :pollId = {pollId} ", pollId);
        var userId = User.GetUserId();

        var result = await _questionService.GetAvailableAsync(pollId, userId!, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

    }
    [AllowAnonymous]
    [HttpPost("")]
    public async Task<IActionResult> Vote([FromRoute] int pollId, [FromBody] VoteRequest request, CancellationToken cancellationToken)
    {
        var result = await _voteService.AddAsync(pollId, User.GetUserId()!, request, cancellationToken);

        return result.IsSuccess ? Created() : result.ToProblem();
    }
}