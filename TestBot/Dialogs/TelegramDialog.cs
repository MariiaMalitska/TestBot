using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TestBot.Dialogs
{
    public class TelegramDialog : ComponentDialog
    {
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
            IMessageActivity activity = Activity.CreateMessageActivity();

            var markup = new

            {
                chat_id = stepContext.Context.Activity.From.Id,
                text = "text",
                reply_markup = new

                {
                    inline_keyboard = new[]
                    {
                       new[]
                       {
                           new{text = "Yes1",callback_data="test1"},
                           new{text = "No1",callback_data="test2"}
                       },

                       new[]
                       {
                           new{text = "Yes2",callback_data="test3"},
                           new{text = "No2",callback_data="test4"},
                           new{text = "No12",callback_data="test5"}
                       }
                    },
                }
            };

            var channelData = new
            {
                method = "sendMessage",
                parameters = markup,
            };

            //activity.Attachments.Clear();
            activity.ChannelData = JObject.FromObject(channelData);

            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            return new DialogTurnResult(DialogTurnStatus.Waiting);
        }

        private async Task<DialogTurnResult> HandleReply(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (stepContext.Context.Activity.Type == "message" && stepContext.Context.Activity.ChannelId == "telegram" && stepContext.Context.Activity.Text.Contains("test"))
            {
                var data = stepContext.Context.Activity.ChannelData;

                IMessageActivity message = Activity.CreateMessageActivity();

                var markup = new
                {
                    chat_id = stepContext.Context.Activity.From.Id,
                    message_id = stepContext.Context.Activity.ReplyToId
                };

                var channelData = new
                {
                    method = "deleteMessage",
                    parameters = markup,
                };

                message.ChannelData = JObject.FromObject(channelData);

                await stepContext.Context.SendActivityAsync(message, cancellationToken);
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
