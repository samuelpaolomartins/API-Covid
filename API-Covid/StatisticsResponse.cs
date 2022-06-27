using System;
using System.Collections.Generic;
using System.Text;

namespace APICovid
{
    public class StatisticsResponse
    {
        public string Continent { get; set; }
        public string Country { get; set; }
        public int? Population { get; set; }//? inteiro ou nulo
        public CaseResponse Cases { get; set; }
        public DeathResponse Deaths { get; set; }
        public TesteResponse Tests { get; set; }
        public DateTime Time { get; set; }
    }
}
