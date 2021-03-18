using AdaptiveCards;
using AdaptiveCards.Templating;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestBot.Dialogs
{
    public class AdaptiveCardDialog : ComponentDialog
    {
        AdaptiveCard card;
        AdaptiveCardTemplate template;
        string id;
        object myData;

        public AdaptiveCardDialog()
            : base(nameof(AdaptiveCardDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowCardStepAsync,
                HandleReply
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> ShowCardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //var templateJson = @"
            //{
            //    ""type"": ""AdaptiveCard"",
            //    ""version"": ""1.2"",
            //    ""body"": [
            //        {
            //            ""type"": ""TextBlock"",
            //            ""text"": ""Hello ${name}!""
            //        }
            //    ]
            //}";

            ////var paths = new[] { ".", "Resources", "toggle.json" };
            ////var templateJson = File.ReadAllText(Path.Combine(paths));

            //// Create a Template instance from the template payload
            //template = new AdaptiveCardTemplate(templateJson);

            //// You can use any serializable object as your data
            //myData = new
            //{
            //    Name = "Matt Hidinger",
            //    BigData = await GetBigData()
            //};

            //// "Expand" the template - this generates the final Adaptive Card payload
            //string cardJson = template.Expand(myData);

            //var adaptiveCardAttachment = new Attachment()
            //{
            //    ContentType = "application/vnd.microsoft.card.adaptive",
            //    Content = JsonConvert.DeserializeObject(cardJson),
            //};


            var choices = new[] { "One", "Two", "Three" };

            card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {

                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock()
                    {
                        Text = "Toggled text",
                        Id = "toggledText",
                        IsVisible = true
                    }
                },
                //Actions = choices.Select(choice => new AdaptiveSubmitAction
                //{
                //    Title = choice,
                //    Data = choice,  // This will be a string
                //}).ToList<AdaptiveAction>(),
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveToggleVisibilityAction
                    {
                        Title = "Toggle visibility",
                        TargetElements = new List<AdaptiveTargetElement>
                        {
                            new AdaptiveTargetElement
                            {
                                ElementId = "toggledText"
                            }
                        }
                    }
                }
            };


            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);
            reply.Attachments.Add(new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                // Convert the AdaptiveCard to a JObject
                Content = JObject.FromObject(card),
            });

            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            id = reply.Id;

            await stepContext.Context.SendActivityAsync(MessageFactory.Text("Type anything to change card."), cancellationToken);

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        private async Task<DialogTurnResult> HandleReply(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //myData = new
            //{
            //    Name = "NewName"
            //};

            //string cardJson = template.Expand(myData);

            //var adaptiveCardAttachment = new Attachment()
            //{
            //    ContentType = "application/vnd.microsoft.card.adaptive",
            //    Content = JsonConvert.DeserializeObject(cardJson),
            //};

            //var attachments = new List<Attachment>();
            //var activity = MessageFactory.Attachment(attachments);
            //activity.Attachments.Add(adaptiveCardAttachment);

            //activity.Id = id;

            //await stepContext.Context.UpdateActivityAsync(activity, cancellationToken);

            //await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            return await stepContext.EndDialogAsync();
        }

        private async Task<string> GetBigData()
        {
            await Task.Delay(5000);
            return "Hello data!";
        }
    }
}
