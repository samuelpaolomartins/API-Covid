using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace APICovid
{
    public class DeathResponse
    {
        //public string New { get; set; } conversão de string para int para conseguir converter ToString
        public int? New { get; set; }

        [JsonProperty("1M_pop")]//atributo para mapear o "1M_pop" para "M1Pop"
        public double? M1Pop { get; set; }

        public int? Total { get; set; }

        public override string ToString()
        {
            string morteNova = New == null ? "Sem dados para apresentar" : New?.ToString("N0");
            string morteMpes = M1Pop == null ? "Sem dados para apresentar" : M1Pop?.ToString("N0");
            string morteTotal = Total == null ? "Sem dados para apresentar" : Total?.ToString("N0");

            return $"   Novos: {morteNova}" +
                $"\n   1M Pessoas: {morteMpes}" +
                $"\n   Total: {morteTotal}";
        }
    }
}
