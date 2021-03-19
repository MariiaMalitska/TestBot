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
    public class TelegramDialog : ComponentDialog
    {
        HeroCard card;
        public TelegramDialog()
            : base(nameof(TelegramDialog))
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
            var activity = new Activity();

            dynamic data = new JObject();
            data.method = "sendMessage";
            data.parameters = "{ \"chat_id\":\"295753893\", \"text\":\"hello from bot freamework\" }";

            activity.ChannelData = data;

            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        private async Task<DialogTurnResult> HandleReply(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync();
        }

        
    }
}
