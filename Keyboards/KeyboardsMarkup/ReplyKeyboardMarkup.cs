using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramShopAsp
{
    class ReplyKeyboardMarkup : IKeyboard
    {
        public bool resize_keyboard = false;
        public bool one_time_keyboard = true;
        public bool selective = false;
        public bool ReplyKeyboardRemove = true;

        List<List<KbrdBtn>> rows = new List<List<KbrdBtn>>();

        public ReplyKeyboardMarkup() { }
        public ReplyKeyboardMarkup(List<List<KbrdBtn>> rows) 
        {
            this.rows = rows;
        }

        public void Addrow(List<KbrdBtn> row)
        {
            rows.Add(row);
        }

        public string Generate()
        {
            StringBuilder sb = new StringBuilder("{\"keyboard\":[");
            foreach (List<KbrdBtn> row in rows)
            {
                sb.Append("[");
                foreach (KbrdBtn btn in row)
                {
                    sb.Append("{\"text\":\"" + btn.text + "\"");
                    if (btn.request_contact)
                        sb.Append(",\"request_contact\":true");
                    if (btn.request_location)
                        sb.Append(",\"request_location\":true");
                    sb.Append("},");
                }
                sb.Remove(sb.Length-1, 1);          // удаляем ненужную запятую
                sb.Append("],");
            }
            sb.Remove(sb.Length-1, 1);              // удаляем ненужную запятую
            sb.Append("]");
            if (resize_keyboard)
                sb.Append(",\"resize_keyboard\":true");
            if (one_time_keyboard)
                sb.Append(",\"one_time_keyboard\":true}");      // закрыли фигурную скобку

            return sb.ToString(); ;
        }
    }

    class KbrdBtn
    {
        public string text;
        public bool request_contact = false;
        public bool request_location = false;
        // KeyboardButtonPollType request_location
        public KbrdBtn(string text)
        {
            this.text = text;
        }

        public KbrdBtn(string text, bool request_contact)
        {
            this.request_contact = request_contact;
            this.text = text;
        }

        public KbrdBtn(string text, bool request_contact, bool request_location)
        {
            this.request_contact = request_contact;
            this.text = text;
            this.request_location = request_location;
        }
    }
}
