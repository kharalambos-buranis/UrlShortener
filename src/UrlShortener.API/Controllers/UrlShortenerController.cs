﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.API.Models.Requests;
using UrlShortener.API.Services;
using UrlShortener.API.Services.Exceptions;

namespace UrlShortener.API.Controllers
{
    [ApiController]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlShortenerService _service;

        public UrlShortenerController(IUrlShortenerService service)
        {
            _service = service;
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginalUrl([FromRoute] string shortCode, CancellationToken cancellationToken)
        {
            var result = await _service.GetUrlDetails(shortCode, cancellationToken);

            if (result.ExpiresAt.HasValue && result.ExpiresAt < DateTime.UtcNow)
            {
               await _service.DeactivateUrlAsync(shortCode);
            }

            await _service.RecordClickAsync(shortCode, Request.Headers["User-Agent"].ToString(), HttpContext.Connection.RemoteIpAddress?.ToString());

            return Redirect(result.OriginalUrl);
        }

        [HttpPost("api/urls")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenUrlRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.ShortenUrl(request, cancellationToken);

            return Ok(result);
        }

        [HttpGet("api/urls/{shortCode}")]
        public async Task<IActionResult> GetUrlDetails([FromRoute] string shortCode, CancellationToken cancellationToken)
        {
            var result = await _service.GetUrlDetails(shortCode, cancellationToken);

            return Ok(result);
        }

        [HttpPut("api/urls/{shortCode}")]
        public async Task<IActionResult> UpdateUrl(
            [FromRoute] string shortCode,
            [FromBody] UpdateUrlRequest request,
            CancellationToken cancellationToken)
        {
            await _service.UpdateUrl(shortCode,request, cancellationToken);

            return NoContent();
        }

        [HttpDelete("api/urls/{shortCode}")]
        public async Task<IActionResult> DeleteUrl([FromRoute] string shortCode, CancellationToken cancellationToken)
        {
            await _service.DeleteUrl(shortCode, cancellationToken);

            return NoContent();
        }
    }
}
