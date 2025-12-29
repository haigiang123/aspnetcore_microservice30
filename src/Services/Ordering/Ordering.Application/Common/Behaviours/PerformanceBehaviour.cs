using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
        
    {
        private readonly ILogger<TRequest> _logger;
        private readonly Stopwatch _stopWatch;

        public PerformanceBehaviour(ILogger<TRequest> logger)
        {
            _logger = logger;
            _stopWatch = new Stopwatch();
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _stopWatch.Start();
            var response = await next();
            _stopWatch.Stop();

            var elaspTimer = _stopWatch.ElapsedMilliseconds;

            if (elaspTimer <= 500) 
                return response;

            var requestName = typeof(TRequest).FullName;
            _logger.LogWarning("Application Long Running Request {0} ({1} milliseconds {2})", requestName, elaspTimer, request);


            return response;
        }
    }
}
