using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Threading;
using TelegramShopAsp.Responses;

namespace TelegramShopAsp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        Thread botThrd;

        private readonly DB.DB_A566BC_xenichContext _context;
        //private readonly IBot _bot;
        //private readonly TelegramRequest _telegram;

        public ValuesController(DB.DB_A566BC_xenichContext _context)//, TelegramRequest _telegram)//, IBot _bot)
        {
            this._context = _context;
            //this._bot = _bot;
            //this._telegram = _telegram;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet("checkConnectionToDB")]
        public string CheckConnectionToDB(string s)
        {
            //DB.DB_A566BC_xenichContext _context = new DB.DB_A566BC_xenichContext();
            int id = 100;
            SqlParameter[] @params =
                {
                    new SqlParameter("@ret", System.Data.SqlDbType.Int) {Direction = System.Data.ParameterDirection.Output},      //выходной параметр
                    new SqlParameter("@value", id) {Direction = System.Data.ParameterDirection.Input},                         // входной параметр в хранимке @id
                };
            string query = $"EXEC	 [dbo].[prCheckConnectionToDB] @value, @ret OUTPUT";
            _context.Database.ExecuteSqlCommand(query, @params);      // не получаем выходной результирующий набор

            try
            {
                if ((int)@params[0].Value == 101)
                {
                    ErrorHandler.Handle("Соединение установлено");
                    return "Соединение установлено";                   
                }
                else
                    return "Соединение НЕ установлено";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        [HttpGet("startBot")]
        public string StartBot(string id)
        {         
            try
            {
                //_telegram.GetUpdates();
                //if (id == "3132304771")
                //{
                //    if (!BOT.initialized)
                //    {
                //        BOT.Init();
                //        return "Bot started";
                //    }
                //}
                //else 
                //{
                //    return "Wrong password";
                //}
            }
            catch (Exception ex)
            {
                return ex.Message;

            }
            return "Bot not started";
        }

        [HttpGet("stoptBot")]
        public string StopBot()
        {          
            return "Bot not stoped";
        }

                                                            //  token = "1120463837:AAHEvmnejgfiH7CvnEts9M5TliR-SQdigpc";       //   @MarketTele_bot
        [HttpPost("getUpdate")]                             // webhook for  https://api.telegram.org:443/bot1120463837:AAHEvmnejgfiH7CvnEts9M5TliR-SQdigpc/setWebhook?url=https://www.xenich.pp.ua/values/getUpdate      
        public void GetUpdate([FromBody] Update update)
        {

            try
            {
                string resp = Newtonsoft.Json.JsonConvert.SerializeObject(update);
                ErrorHandler.Handle(resp);
                TelegramRequest.UpdateFromWebHook(update);
            }
            catch (Exception ex)
            {
                ErrorHandler.Handle(ex.Message);
            }
        }
    }
}
