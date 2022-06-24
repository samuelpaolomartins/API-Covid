//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Text;
//using System.Threading.Tasks;

//namespace APICovid
//{
//    internal class Program2
//    {
//        static async Task ShowCountryHistory(string country)
//        {
//            // Busca pelos países
//            CountryModel info = await GetCountries(country);

//            if (info.Response.Length == 0)
//            {
//                Console.WriteLine("Sem historico do país pesquisado");
//                return;
//            }

//            string paisEscolhido;

//            if (info.Response.Length > 1)
//            {
//                for (int i = 0; i < info.Response.Length; i++)
//                {
//                    Console.WriteLine($"{i + 1} - {info.Response[i]}");
//                }

//                Console.Write("Qual país deseja ver os dados?: ");
//                int escolha;

//                while (!int.TryParse(Console.ReadLine(), out escolha))
//                {
//                    Console.Write("Digite um valor válido: ");
//                }

//                //escolha = escolha - 1;
//                escolha--;

//                if (escolha >= info.Response.Length || escolha < 0)
//                {
//                    Console.WriteLine("Número inválido");
//                    return;
//                }

//                paisEscolhido = info.Response[escolha];
//            }
//            else
//            {
//                paisEscolhido = info.Response[0];
//            }

//            // Pedir uma data
//            Console.Write("Digite uma data (formato dd/MM/yyyy): ");

//            DateTime date;

//            while (true)
//            {
//                string entrada = Console.ReadLine(); //digitar a data

//                if (string.IsNullOrEmpty(entrada))
//                {
//                    date = DateTime.Now;
//                    break;
//                }
//                else if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) //conversão de string para DateTime
//                {
//                    break;
//                }
//                else
//                    Console.Write("Digite uma valor válido: ");
//            }



//            Console.WriteLine($"Histórico do país '{paisEscolhido}' na data '{date.ToString("dd/MM/yyyy")}':");
//            await ShowHistorico(paisEscolhido, date);
//        }
//    }
//}
