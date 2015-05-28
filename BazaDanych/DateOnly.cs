using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class DateOnly
    {
        public DateTime Date { get; set; }

        public override string ToString()
        {
            string day, month;
            if (Date.Day < 10)
                day = "0" + Date.Day;
            else
                day = Date.Day.ToString();
            if (Date.Month < 10)
                month = "0" + Date.Month;
            else
                month = Date.Month.ToString();
            return day + "-" + month + "-" + Date.Year;
        }
    }
}
