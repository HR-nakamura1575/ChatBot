// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.12.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace testBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        // コンストラクタで QnA Maker への接続情報と HttpClient を作るための IHttpClientFactory を受け取る
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public EchoBot(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        // ユーザーから話しかけられた時に呼ばれるメソッド。
        // なのでここで QnA Maker を呼び出す
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // QnA Maker に接続するためのクライアントを作る
            var qnaMaker = new Microsoft.Bot.Builder.AI.QnA.QnAMaker(new Microsoft.Bot.Builder.AI.QnA.QnAMakerEndpoint
            {
                    // appsetting.json に書いた設定項目 
                    KnowledgeBaseId = _configuration["QnAKnowledgebaseId"],
                    EndpointKey = _configuration["QnAEndpointKey"],
                    Host = _configuration["QnAEndpointHostName"]
                }, 
                options: null,
                httpClient: _httpClientFactory.CreateClient()
            );

            // QnA Maker から一番マッチした質問の回答を受け取る
            var options = new Microsoft.Bot.Builder.AI.QnA.QnAMakerOptions { Top = 1 };

            // Teams でボットに質問すると @ボット名 が先頭につくので、それを削除
            turnContext.Activity.RemoveRecipientMention();

            // デバッグ用にオウム返し
            /* await turnContext.SendActivityAsync(
                MessageFactory.Text(text: $"`(*ﾟ▽ﾟ* っ)З` 質問は『{turnContext.Activity.Text}』だね！")
            ); */

            var response = await qnaMaker.GetAnswersAsync(turnContext, options);

            // 回答が存在したら応答する
            if (response != null && response.Length > 0)
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text(text: $"`(*ﾟ▽ﾟ* っ)З` これが聞きたいのかな？『{response[0].Questions[0]}』")
                );

                await turnContext.SendActivityAsync(
                        activity: MessageFactory.Text(response[0].Answer),
                        cancellationToken: cancellationToken
                    );

                await turnContext.SendActivityAsync(
                    MessageFactory.Text(text: $"`(*ﾟ▽ﾟ* っ)З` 他の質問もチェックしてね({response[0].Source})")
                );
            }
            else
            {
                await turnContext.SendActivityAsync(
                        activity: MessageFactory.Text("質問に対する回答が見つかりませんでした"),
                        cancellationToken: cancellationToken
                    );
            }
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "`(*ﾟ▽ﾟ* っ)З` こんにちは！何でも聞いてね";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
