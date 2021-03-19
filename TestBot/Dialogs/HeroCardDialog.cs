using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TestBot.Dialogs
{
    public class HeroCardDialog : ComponentDialog
    {
        HeroCard card;
        private const string TestPrompt = nameof(TestPrompt);
        public HeroCardDialog()
            : base(nameof(HeroCardDialog))
        {
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowCardStepAsync,
                HandleReply
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);

            AddDialog(new ChoicePrompt(TestPrompt, ValidatePrompt));
        }

        private async Task<DialogTurnResult> ShowCardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var attachments = new List<Attachment>();

            var reply = MessageFactory.Attachment(attachments);

            var initialValue = new JObject { { "count", 0 } };

            card = new HeroCard
            {
                Title = "BotFramework Hero Card",
                Subtitle = "Microsoft Bot Framework",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are," +
                       " from text/sms to Skype, Slack, Office 365 mail and other popular services.",
                Buttons = new List<CardAction>
                {
                    new CardAction
                    {
                        Type = ActionTypes.MessageBack,
                        Title = "Update card",
                        Text = "Update"
                    },
                    new CardAction
                    {
                        Type = ActionTypes.MessageBack,
                        Title = "Who am I?",
                        Text = "whoami"
                    },
                    new CardAction
                    {
                        Type = ActionTypes.MessageBack,
                        Title = "Delete card",
                        Text = "Delete"
                    }
                },
            };

            reply.Attachments.Add(card.ToAttachment());

            // Send the card(s) to the user as an attachment to the activity
            await stepContext.Context.SendActivityAsync(reply, cancellationToken);

            //return new DialogTurnResult(DialogTurnStatus.Waiting);
            //return await stepContext.NextAsync(cancellationToken: cancellationToken);
            //return await stepContext.EndDialogAsync();

            var choices = new List<string>
            {
                "One", "Two", "Three"
            };

            return await stepContext.PromptAsync(
                TestPrompt,
                new PromptOptions
                {
                    Choices = ChoiceFactory.ToChoices(choices)
                },
                cancellationToken);
        }

        private async Task<bool> ValidatePrompt(PromptValidatorContext<FoundChoice> promptContext, CancellationToken cancellationToken)
        {
            if (!promptContext.Recognized.Succeeded || promptContext.Recognized.Value == null)
            {
                return false;
            }

            return true;
        }

        private async Task<DialogTurnResult> HandleReply(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Result != null)
            {
                var res = stepContext.Result.ToString();
                await stepContext.Context.SendActivityAsync("Prompt choice recognized");
            }

            return await stepContext.EndDialogAsync();
        }

        private static async Task SendUpdatedCard(ITurnContext turnContext, HeroCard card, CancellationToken cancellationToken)
        {
            card.Title = "I've been updated";

            //var data = turnContext.Activity.Value as JObject;
            //data = JObject.FromObject(data);
            //data["count"] = data["count"].Value<int>() + 1;
            //card.Text = $"Update count - {data["count"].Value<int>()}";

            card.Buttons.Add(new CardAction
            {
                Type = ActionTypes.MessageBack,
                Title = "Newwww option!!",
                Text = "New",
                //Value = data
            });

            var activity = MessageFactory.Attachment(card.ToAttachment());
            activity.Id = turnContext.Activity.Id;

            await turnContext.UpdateActivityAsync(activity, cancellationToken);
        }
    }
}
