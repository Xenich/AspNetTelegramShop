using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramShopAsp
{
    public class InlineKeyboardMarkup : IKeyboard
    {
        readonly List<List<InlineKbrdBtn>> rows = new List<List<InlineKbrdBtn>>();
        public InlineKeyboardMarkup(List<List<InlineKbrdBtn>> rows)
        {
            this.rows = rows;
        }
        public InlineKeyboardMarkup()
        {
        }
        public void Addrow(List<InlineKbrdBtn> row)
        {
            rows.Add(row);
        }

        public string Generate()
        {
            StringBuilder sb = new StringBuilder("{\"inline_keyboard\":[");
            foreach (List<InlineKbrdBtn> row in rows)
            {
                sb.Append("[");
                foreach (InlineKbrdBtn btn in row)
                {
                    sb.Append("{\"text\":\"" + btn.text + "\", \"callback_data\":\"" + btn.callback_data + "\"");
                    if (!string.IsNullOrEmpty(btn.url))
                        sb.Append(",\"url\":\"" + btn.url + "\"");
                    sb.Append("},");
                }
                sb.Remove(sb.Length - 1, 1);          // удаляем ненужную запятую
                sb.Append("],");
            }
            sb.Remove(sb.Length - 1, 1);              // удаляем ненужную запятую
            sb.Append("]}");
            return sb.ToString(); ;
        }
    }

    public class InlineKbrdBtn
    {
        public string text;
        public string callback_data;
        public string url;
        public string switch_inline_query;
        public string switch_inline_query_current_chat;

        // KeyboardButtonPollType request_location
        public InlineKbrdBtn(string text, string callback_data)
        {
            this.text = text;
            this.callback_data = callback_data;
        }
    }
}
