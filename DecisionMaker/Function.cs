using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Requests.RequestTypes;
using Slight.Alexa.Framework.Models.Responses;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace DecisionMaker
{
    public class Function
    {
        private readonly Random _random = new Random((int)DateTime.UtcNow.Ticks);

        private readonly List<string> _answers = new List<string>
        {
            "It is certain",
            "It is decidedly so",
            "Without a doubt",
            "Heck yes",
            "Yes definitely",
            "You may rely on it",
            "As I see it, yes",
            "Most likely",
            "Outlook good",
            "Yes",
            "Signs point to yes",
            "Reply hazy try again",
            "Ask again later",
            "Better not tell you now",
            "Cannot predict now",
            "Concentrate and ask again",
            "Don't count on it",
            "My reply is no",
            "My sources say no",
            "Outlook not so good",
            "Very doubtful",
            "I don't think so, buddy",
            "Aye, me hearty! "
        };

        

        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            IOutputSpeech innerResponse = null;
            var log = context.Logger;
            var shouldEndSession = false;

            log.LogLine(JsonConvert.SerializeObject(input));

            if (input.GetRequestType() == typeof(ILaunchRequest))
            {
                // default launch request, let's just let them know what you can do
                log.LogLine("Default LaunchRequest made");

                innerResponse = Help;
            }
            else if (input.GetRequestType() == typeof(IIntentRequest))
            {
                // intent request, process the intent
                log.LogLine($"Intent Requested {input.Request.Intent.Name}");

                switch (input.Request.Intent.Name)
                {
                    case "QuestionIntent":
                        innerResponse = RandomAnswer;
                        shouldEndSession = true;
                        break;
                    case "AMAZON.HelpIntent":
                        innerResponse = Help;
                        break;
                    case "AMAZON.StopIntent":
                    case "AMAZON.CancelIntent":
                        innerResponse = End;
                        shouldEndSession = true;
                        break;
                    default:
                        innerResponse = Error;
                        break;
                }
            }

            var response = new Response
            {
                ShouldEndSession = shouldEndSession,
                OutputSpeech = innerResponse
            };

            var skillResponse = new SkillResponse
            {
                Response = response,
                Version = "1.0"
            };

            return skillResponse;
        }

        public PlainTextOutputSpeech RandomAnswer => Response(_answers[_random.Next(0, _answers.Count)]);

        public PlainTextOutputSpeech Help => Response("Ask me a question that requires a yes or no answer and I'll decide what the answer should be.");

        public PlainTextOutputSpeech End => Response("Goodbye.");

        public PlainTextOutputSpeech Error => Response("Hmm, something went wrong. Try again");

        private static PlainTextOutputSpeech Response(string stringAnswer)
        {
            return new PlainTextOutputSpeech
            {
                Text = stringAnswer
            };
        }
    }
}
