using AdaptiveCards;
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
    public class AdaptiveCardDialog : ComponentDialog
    {
        AdaptiveCard card;
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

            AddDialog(new ChoicePrompt(nameof(ShowCardStepAsync)));
        }


        private async Task<DialogTurnResult> ShowCardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var choices = new[] { "One", "Two", "Three" };

            card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 0))
            {  
                
                Body = new List<AdaptiveElement>()
                {
                    new AdaptiveTextBlock("Example card"),
                },
                Actions = choices.Select(choice => new AdaptiveSubmitAction
                {
                    Title = choice,
                    Data = choice,  // This will be a string
                }).ToList<AdaptiveAction>(),
            };

            return await stepContext.PromptAsync(
            nameof(ShowCardStepAsync),
            new PromptOptions
            {
                Prompt = (Activity)MessageFactory.Attachment(new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    // Convert the AdaptiveCard to a JObject
                    Content = JObject.FromObject(card),
                }),
                Choices = ChoiceFactory.ToChoices(choices),
                // Don't render the choices outside the card
                Style = ListStyle.None,
            },
            cancellationToken);
        }

        private async Task<DialogTurnResult> HandleReply(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync();
        }
    }
}
