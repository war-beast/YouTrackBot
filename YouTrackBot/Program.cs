using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MihaZupan;
using System;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

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
            Settings = new CustomSettings { TelegramKey = configuration["CustomSettings:TelegramKey"], SocksProxy = configuration["CustomSettings:SocksProxy"], ProxyPort = Int32.Parse(configuration["CustomSettings:ProxyPort"]) };

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

            string result = GetResult(inlineQueryEventArgs);

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

        private static string GetResult(InlineQueryEventArgs inlineQueryEventArgs)
        {
            var query = inlineQueryEventArgs.InlineQuery.Query;
            var command = query.Substring(0, query.IndexOf("///")).Split(' ');
            var projectName = command[0];
            var taskType = command[1].ToLower();
            taskType = taskType.Contains("task") || taskType.Contains("feature") || taskType.Contains("bug") ? taskType : "";
            var taskName = command[2];
            var taskDesc = BuildDescription(command);

            var result = $"Проект: {projectName}\r\nТип задачи: {taskType}\r\nНаменование: {taskName}\r\nОписание: {taskDesc}\r\nРезультат: Ok";
            return result;
        }

        private static string BuildDescription(string[] commandParts)
        {
            string retVal = "";

            for (int idx = 3; idx < commandParts.Length; idx++)
            {
                retVal += commandParts[idx] + " ";
            }

            return retVal.Trim();
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
