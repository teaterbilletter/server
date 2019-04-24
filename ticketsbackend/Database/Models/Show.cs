using System;
using System.Collections.Generic;

namespace Database.Models
{
    public class Show
    {
        public string title { get; set; }
        public List<DateTime> dates { get; set; }
        public Theater theater { get; set; }

        public Show(string title, List<DateTime> dates, Theater theater)
        {
            this.title = title;
            this.dates = dates;
            this.theater = theater;
        }

        public Show()
        {
            
            
        }
    }
}