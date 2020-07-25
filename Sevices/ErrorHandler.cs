using Microsoft.EntityFrameworkCore;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TelegramShopAsp.DB;

namespace TelegramShopAsp
{

    class TelegramErrorHandler : IErrorHandler
    {
        public void Handler(string message)
        {
            TelegramRequest.SendSimpleMessage(message, 1042630860);
        }
    }

    class ConsoleErrorHandler : IErrorHandler
    {
        public void Handler(string message)
        {
            Console.WriteLine(message);
        }
    }

    class DBErrorHandler : IErrorHandler
    {     
       // private static DB_A566BC_xenichContext _context = new DB_A566BC_xenichContext();
        public void Handler(string message)
        {
            try
            {
                using (DB_A566BC_xenichContext _context = new DB_A566BC_xenichContext())
                {

                    _context.LoadStoredProc("prSetLog")
                           .AddParam("@Message", message)
                           .ExecNonQuery();
                    //Log log = new Log() { Date = DateTime.Now, Message = message };
                    //_context.Add(log);
                    //_context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                TelegramErrorHandler h = new TelegramErrorHandler();
                h.Handler(ex.Message);
            }
        }
    }

    public class ErrorHandler
    {
        private static IErrorHandler handler = new DBErrorHandler();
        public static void Handle(string message)
        {
            handler.Handler(message);
        }
    }
}
