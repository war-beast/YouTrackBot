using Microsoft.AspNetCore.Mvc;
using MihaZupan;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using YouTrackBot.Models;

namespace YouTrackBot.Controllers
{
    public class HomeController : Controller
    {
        private string key = "864722583:AAHxMFbwGUSU_8jkdskKNMVa2_THOdQk1pQ";

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("api/[controller]/[action]")]
        public async Task<IActionResult> GetCommand()
        {
            string retVal = "";

            try { 
                var bot = new Telegram.Bot.TelegramBotClient(key, new HttpToSocks5Proxy("127.0.0.1", 9050));
                await bot.SetWebhookAsync("");

                int offset = 0; // отступ по сообщениям
                var updates = await bot.GetUpdatesAsync(offset); // получаем массив обновлений
                foreach (var update in updates) // Перебираем все обновления
                {
                    var message = update.Message;
                    if (message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
                    {
                        retVal += "UP: " + update.Id + " Message: " + message.Text + "<br />";
                    }
                    offset = update.Id + 1;
                }
            }
            catch(Exception ex)
            {
                retVal = ex.Message;
                return BadRequest(retVal);
            }


            return Ok(retVal);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
