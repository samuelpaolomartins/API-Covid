using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace APICovid
{
    public class TesteResponse
    {
        [JsonProperty("1M_pop")]//atributo para mapear o "1M_pop" para "M1Pop"
        //public string M_pop { get; set; } conversão de string para int para conseguir converter ToString
        public double? M_pop { get; set; }

        public int? Total { get; set; }

        public override string ToString()
        {
            string testeMpes = M_pop == null ? "Sem dados para apresentar" : M_pop?.ToString("N0");
            string testeTotal = Total == null ? "Sem dados para apresentar" : Total?.ToString("N0");

            return $"   a cada 1M Pessoas: {testeMpes}" +
                $"\n   Total: {testeTotal}";
        }
    }
}
