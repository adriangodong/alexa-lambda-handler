using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;

[assembly: InternalsVisibleTo("AlexaLambdaHandler.Tests")]
namespace AlexaLambdaHandler
{
    public abstract class LambdaHandler
    {

        protected LambdaHandler()
        {
            IntentRequestHandlers = new Dictionary<string, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>>>();
            DefaultHandler = (request, context) => Task.FromResult(ResponseBuilder.Empty());
        }

        public Func<SkillRequest, AudioPlayerRequest, ILambdaContext, Task<SkillResponse>> AudioPlayerRequestHandler { get; protected internal set; }
        public Dictionary<string, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>>> IntentRequestHandlers { get; protected internal set; }
        public Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>> DefaultIntentRequestHandler { get; protected internal set; }
        public Func<SkillRequest, LaunchRequest, ILambdaContext, Task<SkillResponse>> LaunchRequestHandler { get; protected internal set; }
        public Func<SkillRequest, PlaybackControllerRequest, ILambdaContext, Task<SkillResponse>> PlaybackControllerRequestHandler { get; protected internal set; }
        public Func<SkillRequest, SessionEndedRequest, ILambdaContext, Task<SkillResponse>> SessionEndedRequestHandler { get; protected internal set; }
        public Func<SkillRequest, SystemExceptionRequest, ILambdaContext, Task<SkillResponse>> SystemExceptionRequestHandler { get; protected internal set; }
        public Func<SkillRequest, ILambdaContext, Task<SkillResponse>> DefaultHandler { get; protected internal set; }

        public void AddAsyncIntentRequestHandler(string[] intentNames, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>> handler)
        {
            foreach (var intentName in intentNames)
            {
                if (!IntentRequestHandlers.ContainsKey(intentName))
                {
                    IntentRequestHandlers.Add(intentName, null);
                }
                IntentRequestHandlers[intentName] = handler;
            }
        }

        public void AddIntentRequestHandler(string[] intentNames, Func<SkillRequest, IntentRequest, ILambdaContext, SkillResponse> handler)
        {
            AddAsyncIntentRequestHandler(intentNames, (request, intentRequest, context) => Task.FromResult(handler(request, intentRequest, context)));
        }

        public void AddAsyncIntentRequestHandler(string intentName, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>> handler)
        {
            AddAsyncIntentRequestHandler(new[] { intentName }, handler);
        }

        public void AddIntentRequestHandler(string intentName, Func<SkillRequest, IntentRequest, ILambdaContext, SkillResponse> handler)
        {
            AddAsyncIntentRequestHandler(intentName, (request, intentRequest, context) => Task.FromResult(handler(request, intentRequest, context)));
        }

        public async Task<SkillResponse> Handle(SkillRequest request, ILambdaContext context)
        {
            switch (request.Request)
            {
                case AudioPlayerRequest audioPlayerRequest:
                    return AudioPlayerRequestHandler != null ?
                        await AudioPlayerRequestHandler(request, audioPlayerRequest, context) :
                        await DefaultHandler(request, context);

                case IntentRequest intentRequest:
                    var intentName = intentRequest.Intent.Name;
                    return IntentRequestHandlers.ContainsKey(intentName) && IntentRequestHandlers[intentName] != null ?
                        await IntentRequestHandlers[intentName](request, intentRequest, context) :
                        DefaultIntentRequestHandler != null ?
                        await DefaultIntentRequestHandler(request, intentRequest, context) :
                        await DefaultHandler(request, context);

                case LaunchRequest launchRequest:
                    return LaunchRequestHandler != null ?
                        await LaunchRequestHandler(request, launchRequest, context) :
                        await DefaultHandler(request, context);

                case PlaybackControllerRequest playbackControllerRequest:
                    return PlaybackControllerRequestHandler != null ?
                        await PlaybackControllerRequestHandler(request, playbackControllerRequest, context) :
                        await DefaultHandler(request, context);

                case SessionEndedRequest sessionEndedRequest:
                    return SessionEndedRequestHandler != null ?
                        await SessionEndedRequestHandler(request, sessionEndedRequest, context) :
                        await DefaultHandler(request, context);

                case SystemExceptionRequest systemExceptionRequest:
                    return SystemExceptionRequestHandler != null ?
                        await SystemExceptionRequestHandler(request, systemExceptionRequest, context) :
                        await DefaultHandler(request, context);

                default:
                    return await DefaultHandler(request, context);

            }
        }

    }
}
