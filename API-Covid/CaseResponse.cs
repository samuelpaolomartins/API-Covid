using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace APICovid
{
    public class CaseResponse
    {
        //public string New { get; set; } conversão de string para int para conseguir converter ToString
        public int? New { get; set; }
        public int? Active { get; set; }
        public int? Critical { get; set; }
        public int? Recovered { get; set; }

        [JsonProperty("1M_pop")]//atributo para mapear o "1M_pop" para "M1Pop"
        public double? M1Pop { get; set; }

        public int? Total { get; set; }
    }
}
