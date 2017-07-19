using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

namespace AlexaLambdaHandler.Tests.Helpers
{
    internal class MockHandler
    {
        private readonly SkillResponse skillResponse = ResponseBuilder.Empty();

        public bool HandlerCalled { get; private set; }

        public Task<SkillResponse> HandlerAsync(SkillRequest request, Request anyRequest, ILambdaContext context)
        {
            return Task.FromResult(Handler(request, anyRequest, context));
        }

        public SkillResponse Handler(SkillRequest request, Request anyRequest, ILambdaContext context)
        {
            return DefaultHandler(request, context);
        }

        public Task<SkillResponse> DefaultHandlerAsync(SkillRequest request, ILambdaContext context)
        {
            return Task.FromResult(DefaultHandler(request, context));
        }

        public SkillResponse DefaultHandler(SkillRequest request, ILambdaContext context)
        {
            HandlerCalled = true;
            return skillResponse;
        }
    }
}
