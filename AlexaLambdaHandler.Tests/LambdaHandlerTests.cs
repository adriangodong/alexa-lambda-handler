using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AlexaLambdaHandler.Tests.Helpers;
using Amazon.Lambda.Core;

namespace AlexaLambdaHandler.Tests
{
    [TestClass]
    public class LambdaHandlerTests
    {

        private MockHandler defaultHandler;
        private LambdaHandler lambdaHandler;
        private Mock<ILambdaContext> mockLambdaContext;

        [TestInitialize]
        public void Initialize()
        {
            defaultHandler = new MockHandler();
            lambdaHandler = new Mock<LambdaHandler>().Object;
            mockLambdaContext = new Mock<ILambdaContext>();
        }

        [TestMethod]
        public void AddAsyncIntentRequestHandler_ShouldSetHandler()
        {
            // Arrange
            var intentName1 = Guid.NewGuid().ToString("N");
            var intentName2 = Guid.NewGuid().ToString("N");
            var intentRequestHandler = new MockHandler();

            lambdaHandler.IntentRequestHandlers = new Dictionary<string, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>>>
            {
                { intentName1, defaultHandler.HandlerAsync }
            };

            // Act
            lambdaHandler.AddAsyncIntentRequestHandler(new[] { intentName1, intentName2 }, intentRequestHandler.HandlerAsync);

            // Assert
            Assert.AreEqual(intentRequestHandler.HandlerAsync, lambdaHandler.IntentRequestHandlers[intentName1]);
            Assert.AreEqual(intentRequestHandler.HandlerAsync, lambdaHandler.IntentRequestHandlers[intentName2]);
        }

        [TestMethod]
        public async Task AddIntentRequestHandler_ShouldSetHandlerAsAsync()
        {
            // Arrange
            var intentName1 = Guid.NewGuid().ToString("N");
            var intentName2 = Guid.NewGuid().ToString("N");
            var intentRequestHandler = new MockHandler();

            lambdaHandler.IntentRequestHandlers = new Dictionary<string, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>>>
            {
                { intentName1, defaultHandler.HandlerAsync }
            };

            // Act
            lambdaHandler.AddIntentRequestHandler(new[] { intentName1, intentName2 }, intentRequestHandler.Handler);

            // Assert
            Assert.IsNotNull(lambdaHandler.IntentRequestHandlers[intentName1]);
            Assert.IsNotNull(lambdaHandler.IntentRequestHandlers[intentName2]);
            Assert.AreEqual(intentRequestHandler.Handler(null, null, null), await lambdaHandler.IntentRequestHandlers[intentName1](null, null, null));
            Assert.AreEqual(intentRequestHandler.Handler(null, null, null), await lambdaHandler.IntentRequestHandlers[intentName2](null, null, null));
        }

        [TestMethod]
        public void AddAsyncIntentRequestHandler_ShouldSetHandlerForSingleName()
        {
            // Arrange
            var intentName = Guid.NewGuid().ToString("N");
            var intentRequestHandler = new MockHandler();

            lambdaHandler.IntentRequestHandlers = new Dictionary<string, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>>>
            {
                { intentName, defaultHandler.HandlerAsync }
            };

            // Act
            lambdaHandler.AddAsyncIntentRequestHandler(intentName, intentRequestHandler.HandlerAsync);

            // Assert
            Assert.AreEqual(intentRequestHandler.HandlerAsync, lambdaHandler.IntentRequestHandlers[intentName]);
        }

        [TestMethod]
        public async Task AddIntentRequestHandler_ShouldSetHandlerAsAsyncForSingleName()
        {
            // Arrange
            var intentName = Guid.NewGuid().ToString("N");
            var intentRequestHandler = new MockHandler();

            lambdaHandler.IntentRequestHandlers = new Dictionary<string, Func<SkillRequest, IntentRequest, ILambdaContext, Task<SkillResponse>>>
            {
                { intentName, defaultHandler.HandlerAsync }
            };

            // Act
            lambdaHandler.AddIntentRequestHandler(intentName, intentRequestHandler.Handler);

            // Assert
            Assert.IsNotNull(lambdaHandler.IntentRequestHandlers[intentName]);
            Assert.AreEqual(intentRequestHandler.Handler(null, null, null), await lambdaHandler.IntentRequestHandlers[intentName](null, null, null));
        }

        [TestMethod]
        public async Task Handle_ShouldReturnEmptyWhenDefaultNotSet()
        {
            // Arrange
            // Act
            var skillResponse = await lambdaHandler.Handle(new SkillRequest(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
        }

        [TestMethod]
        public async Task Handle_AudioPlayerRequest_ShouldCallDefaultWhenHandlerNotSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.AudioPlayer(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsFalse(requestHandler.HandlerCalled);
            Assert.IsTrue(defaultHandler.HandlerCalled);

        }

        [TestMethod]
        public async Task Handle_AudioPlayerRequest_ShouldCallHandlerWhenSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.AudioPlayerRequestHandler = requestHandler.HandlerAsync;
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.AudioPlayer(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(requestHandler.HandlerCalled);
            Assert.IsFalse(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_IntentRequest_ShouldCallIntentHandlerWhenSet()
        {
            // Arrange
            var intentName = Guid.NewGuid().ToString("N");
            var requestHandler = new MockHandler();
            lambdaHandler.AddAsyncIntentRequestHandler(intentName, requestHandler.HandlerAsync);
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.Intent(intentName), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(requestHandler.HandlerCalled);
            Assert.IsFalse(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_IntentRequest_ShouldCallDefaultWhenHandlerSetToNull()
        {
            // Arrange
            var intentName = Guid.NewGuid().ToString("N");
            lambdaHandler.AddAsyncIntentRequestHandler(intentName, null);
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.Intent(intentName), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_IntentRequest_ShouldCallDefaultIntentHandlerWhenIntentHandlerNotSet()
        {
            // Arrange
            var intentName = Guid.NewGuid().ToString("N");
            var requestHandler = new MockHandler();
            lambdaHandler.DefaultIntentRequestHandler = requestHandler.HandlerAsync;
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.Intent(intentName), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(requestHandler.HandlerCalled);
            Assert.IsFalse(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_IntentRequest_ShouldCallDefaultWhenHandlerNotSet()
        {
            // Arrange
            var intentName = Guid.NewGuid().ToString("N");
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.Intent(intentName), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_LaunchRequest_ShouldCallDefaultWhenHandlerNotSet()
        {
            // Arrange
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.Launch(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_LaunchRequest_ShouldCallHandlerWhenSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.LaunchRequestHandler = requestHandler.HandlerAsync;
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.Launch(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(requestHandler.HandlerCalled);
            Assert.IsFalse(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_PlaybackControllerRequest_ShouldCallDefaultWhenHandlerNotSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.PlaybackController(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsFalse(requestHandler.HandlerCalled);
            Assert.IsTrue(defaultHandler.HandlerCalled);

        }

        [TestMethod]
        public async Task Handle_PlaybackControllerRequest_ShouldCallHandlerWhenSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.PlaybackControllerRequestHandler = requestHandler.HandlerAsync;
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.PlaybackController(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(requestHandler.HandlerCalled);
            Assert.IsFalse(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_SessionEndedRequest_ShouldCallDefaultWhenHandlerNotSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.SessionEnded(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsFalse(requestHandler.HandlerCalled);
            Assert.IsTrue(defaultHandler.HandlerCalled);

        }

        [TestMethod]
        public async Task Handle_SessionEndedRequest_ShouldCallHandlerWhenSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.SessionEndedRequestHandler = requestHandler.HandlerAsync;
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.SessionEnded(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(requestHandler.HandlerCalled);
            Assert.IsFalse(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_SystemExceptionRequest_ShouldCallDefaultWhenHandlerNotSet()
        {
            // Arrange
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.SystemException(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(defaultHandler.HandlerCalled);
        }

        [TestMethod]
        public async Task Handle_SystemExceptionRequest_ShouldCallHandlerWhenSet()
        {
            // Arrange
            var requestHandler = new MockHandler();
            lambdaHandler.SystemExceptionRequestHandler = requestHandler.HandlerAsync;
            lambdaHandler.DefaultHandler = defaultHandler.DefaultHandlerAsync;

            // Act
            var skillResponse = await lambdaHandler.Handle(BuiltInIntentBuilder.SystemException(), mockLambdaContext.Object);

            // Assert
            Assert.IsNotNull(skillResponse);
            Assert.IsTrue(requestHandler.HandlerCalled);
            Assert.IsFalse(defaultHandler.HandlerCalled);
        }

    }
}
