# Alexa Lambda Handler

[![Build status](https://ci.appveyor.com/api/projects/status/50x6ft24fl1pv8ae/branch/master?svg=true)](https://ci.appveyor.com/project/adriangodong/alexa-lambda-handler/branch/master)

[![NuGet](https://img.shields.io/nuget/v/AlexaLambdaHandler.svg)](https://www.nuget.org/packages/AlexaLambdaHandler/)

Alexa Lambda Handler is a helper class that defines injection points for routing Alexa Skill requests hosted on AWS Lambda. Depends on [Alexa.NET](https://www.nuget.org/packages/Alexa.NET/) Nuget package.

## Usage ##

1. Add a reference to AlexaLambdaHandler using Nuget.
2. Make your AWS Lambda handler class derive from `AlexaLambdaHandler.LambdaHandler` abstract class.
3. Add handlers inside the constructor.
4. Call `await Handle(input, context)` in your AWS Lambda handler method and return the output.

## Code Sample ##

```
// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
namespace MyAlexaSkillLambda
{
    public class Function : LambdaHandler
    {

        public Function()
        {
            AddAsyncIntentRequestHandler("SomeCustomIntent", HandleCustomIntent);

            SystemExceptionRequestHandler = (request, exceptionRequest, context) =>
             {
                 context.Logger.Log($"ERROR: {exceptionRequest.Error.Message}");
                 return Task.FromResult(ResponseBuilder.Empty());
             };
        }

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            return await Handle(input, context);
        }

        internal async Task<SkillResponse> HandleLaunchRequest(SkillRequest request, LaunchRequest launchRequest, ILambdaContext context)
        {
            ...
        }

        internal async Task<SkillResponse> HandleCustomIntent(SkillRequest request, IntentRequest intentRequest, ILambdaContext context)
        {
            ...
        }

    }
}
```

AWS Lambda Handler string for the above class is "`MyAlexaSkillLambda::MyAlexaSkillLambda.Function::FunctionHandler`".

## API Definition ##

### Property `LambdaHandler.AudioPlayerRequestHandler` ###

Get/set handler for audio player requests.

### Property `LambdaHandler.IntentRequestHandlers` ###

Get/set handler for intent requests, both built-in and custom.

### Property `LambdaHandler.DefaultIntentRequestHandler` ###

Get/set handler for default intent requests. This handler will be used when an intent name does not match any known intents. Register known intents using `LambdaHandler.AddIntentRequestHandler()` method.

### Property `LambdaHandler.LaunchRequestHandler` ###

Get/set handler for launch requests.

### Property `LambdaHandler.PlaybackControllerRequestHandler` ###

Get/set handler for playback controller requests.

### Property `LambdaHandler.SessionEndedRequestHandler` ###

Get/set handler for session end requests.

### Property `LambdaHandler.SystemExceptionRequestHandler` ###

Get/set handler for system exception requests.

### Property `LambdaHandler.DefaultHandler` ###

Get/set default handler. Default handler will be used when no handler is set for a request. For intent requests, default handler will be used when handling unknown intent and `LambdaHandler.DefaultIntentRequestHandler` is not set.

### Method `LambdaHandler.AddIntentRequestHandler()` and `LambdaHandler.AddAsyncIntentRequestHandler()` ###

Add a handler for a specific intent name. Built-in intents are prefixed with "`AMAZON.`", e.g. "`AMAZON.HelpIntent`".

### Method `LambdaHandler.Handle()` ###

Perform routing of request into one of the configured handlers. Call this and return the output on your AWS Lambda handler function.

Code sample:
```
public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
{
    return await Handle(input, context);
}
```