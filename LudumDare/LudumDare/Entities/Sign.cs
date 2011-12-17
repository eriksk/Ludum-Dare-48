using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LudumDare.Entities
{
    class Sign
    {
        public string text;
        public int col, row;

        public static Sign Parse(string line)
        {
            Sign s = new Sign();

            string[] values = line.ToLower().Split('&');
            for (int i = 0; i < values.Length; i++)
            {
                string key = values[i].Split(':')[0];
                string value = values[i].Split(':')[1];

                switch (key)
                {
                    case "col":
                        s.col = int.Parse(value);
                        break;
                    case "row":
                        s.row = int.Parse(value);
                        break;
                    case "text":
                        s.text = value.ToUpper().Replace("/L/", "\n");
                        break;
                    default:
                        break;
                }

            }

            return s;
        }
    }
}
