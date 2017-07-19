using Alexa.NET.Request;
using Alexa.NET.Request.Type;

namespace AlexaLambdaHandler.Tests.Helpers
{
    public static class BuiltInIntentBuilder
    {

        public static SkillRequest AudioPlayer()
        {
            return new SkillRequest()
            {
                Request = new AudioPlayerRequest()
            };
        }

        public static SkillRequest Intent(string intentName)
        {
            return new SkillRequest()
            {
                Request = new IntentRequest()
                {
                    Intent = new Intent()
                    {
                        Name = intentName
                    }
                }
            };
        }

        public static SkillRequest Launch()
        {
            return new SkillRequest()
            {
                Request = new LaunchRequest()
            };
        }

        public static SkillRequest PlaybackController()
        {
            return new SkillRequest()
            {
                Request = new PlaybackControllerRequest()
            };
        }

        public static SkillRequest SessionEnded()
        {
            return new SkillRequest()
            {
                Request = new SessionEndedRequest()
            };
        }

        public static SkillRequest SystemException()
        {
            return new SkillRequest()
            {
                Request = new SystemExceptionRequest()
            };
        }

    }
}