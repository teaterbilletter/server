using System;
using System.Collections.Generic;
using ticketsbackend.Models;

namespace Database.Models
{
    public class Show
    {
        public int ID { get; set; }
        public string title { get; set; }
        public List<DateTime> dates { get; set; }
        public Hall hall { get; set; }
        public string imgUrl { get; set; }
        public decimal basePrice { get; set; }
        public bool active { get; set; } = true;

        public Show(int id, string title, List<DateTime> dates, Hall hall, string imgUrl, decimal basePrice)
        {
            ID = id;
            this.title = title;
            this.dates = dates;
            this.hall = hall;
            this.imgUrl = imgUrl;
            this.basePrice = basePrice;
        }

        public Show()
        {
            
        }
    }
}