using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MihaZupan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using YouTrackSharp.Projects;

namespace YouTrackBot
{
    public static class Program
    {
        private static TelegramBotClient Bot { get; set; }
        private static CustomSettings Settings { get; set; }
        private static int QueryId { get; set; } = 1;

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var configuration = builder.Build();
            Settings = new CustomSettings {
                TelegramKey = configuration["CustomSettings:TelegramKey"],
                SocksProxy = configuration["CustomSettings:SocksProxy"],
                ProxyPort = Int32.Parse(configuration["CustomSettings:ProxyPort"]),
                YouTrackToken = configuration["CustomSettings:YouTrackToken"]
            };

            Bot = new TelegramBotClient(Settings.TelegramKey, new HttpToSocks5Proxy(Settings.SocksProxy, Settings.ProxyPort));
            var me = Bot.GetMeAsync().Result;
            Console.Title = me.Username;

            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");
            if (!inlineQueryEventArgs.InlineQuery.Query.EndsWith("///"))
                return;

            string result = new CommandParser(Settings).GetResult(inlineQueryEventArgs);

            InlineQueryResultBase[] results = {
                new InlineQueryResultArticle (id: QueryId.ToString(), title: "Результат", inputMessageContent: new InputTextMessageContent(result))
            };

            try { 
            await Bot.AnswerInlineQueryAsync(
                inlineQueryEventArgs.InlineQuery.Id,
                results,
                isPersonal: true,
                cacheTime: 0);
            }
            catch (Exception)
            {

            }

            QueryId++;
        }        

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message);
        }
    }
}
