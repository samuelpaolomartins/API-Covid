using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Globalization;
using System.Collections.Generic;

namespace APICovid
{
    // Métodos da API em classe separada

    internal class Program
    {
        private static string Url = "https://covid-193.p.rapidapi.com";
        private static string Key = "d7a0543448mshd4c0461dfd90571p149c59jsn063447df72e7";
        static async Task Main(string[] args)
        {
            while (true)
            {
                Console.Write("Você deseja" +
                "\n1 - Listar todos os países disponíveis da API" +
                "\n2 - Pesquisar por países disponíveis na API" +
                "\n3 - Visualizar os dados de um país específico" +
                "\n4 - Visualizar os dados de um país específico em uma data específica" +
                "\n5 - Visualizar os dados do mundo todo e" +
                "\n6 - Visualizar os dados do munddo em uma data específica" +
                "\n7 - Visualizar dados dos continentes" +
                "\n8 - Ranking dos 10 países que mais houveram casos totais, casos recuperados, mortes e testes totais" +
                "\n9 - Parar aplicação" +
                "\nDigite uma opção:");

                int escolher;

                while (!int.TryParse(Console.ReadLine(), out escolher))
                    Console.Write("Insira uma opção válida: ");

                Opcoes opcao = (Opcoes)escolher;

                if (opcao == Opcoes.FecharAplicacao)
                    break;

                switch (opcao)
                {
                    case Opcoes.PaisesDisponiveis:
                        Console.WriteLine("\nPaíses disponíveis da API:");
                        await MostrarPais();
                        Console.WriteLine();
                        break;
                    case Opcoes.PesquisarPais:
                        {
                            while (true)
                            {
                                Console.Write("Digite um país: ");
                                var nomePais = Console.ReadLine();

                                if (!string.IsNullOrEmpty(nomePais))
                                {
                                    Console.WriteLine($"Países disponíveis com '{nomePais}'");
                                    await MostrarPais(nomePais);
                                    Console.WriteLine();
                                    break;
                                }
                            }
                            break;
                        }
                    case Opcoes.VisualizarDadosPais:
                        {
                            while (true)
                            {
                                Console.Write("Digite um país: ");
                                var nomePais = Console.ReadLine();

                                if (!string.IsNullOrEmpty(nomePais))
                                {
                                    Console.WriteLine($"Estatísticas encontrada do país '{nomePais}': ");
                                    await ShowCountries(nomePais);

                                    break;
                                }
                            }
                            break;
                        }
                    case Opcoes.VisualizarDadosPaisData:
                        {
                            while (true)
                            {
                                Console.Write("Digite um país: ");
                                string nomePais = Console.ReadLine();

                                if (!string.IsNullOrEmpty(nomePais))
                                {
                                    await ShowCountryHistory(nomePais);
                                    break;
                                }
                            }
                            break;
                        }
                    case Opcoes.VisualizarDadosMundo:
                        {
                            Console.WriteLine("Lista das estatísticas do mundo:");

                            await ShowStatistics("all");
                            Console.WriteLine();
                            break;
                        }
                    case Opcoes.VisualizarDadosMundoData:
                        {
                            while (true)
                            {
                                Console.Write("Digite uma data (formato dd/MM/yyyy): ");
                                DateTime date;

                                while (true)
                                {
                                    string entrada = Console.ReadLine();

                                    if (string.IsNullOrEmpty(entrada))
                                    {
                                        date = DateTime.Now;
                                        break;
                                    }
                                    else if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        Console.Write("Insira uma data válida: ");
                                    }
                                }

                                Console.WriteLine($"Histórico do mundo na data '{date:dd/MM/yyyy}':"); //date:dd/MM/yyyy maneira simplificada do date.ToString("dd/MM/yyyy")
                                await ShowHistorico("all", date);
                                Console.WriteLine();
                                break;
                            }
                            break;
                        }
                    case Opcoes.PesquisaContinente:
                        await GetContinents();
                        break;
                    default:
                        Console.WriteLine("Opção não existe");
                        break;
                }
            }
        }
        static async Task<CountryModel> GetCountries(string country = null)
        {
            var client = new HttpClient();

            var url = Url + "/countries";

            if (!string.IsNullOrEmpty(country))
                url += $"?search={country}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-RapidAPI-Key", Key },
                    { "X-RapidAPI-Host", "covid-193.p.rapidapi.com" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                CountryModel info = JsonConvert.DeserializeObject<CountryModel>(body);
                for (int i = 0; i < info.Response.Length; i++)
                {
                    info.Response[i] = HttpUtility.HtmlDecode(info.Response[i]);
                }
                return info;
            }
        }
        static async Task ShowCountries(string country = null)
        {
            CountryModel info = await GetCountries(country);

            if (info.Response.Length == 1)
            {
                await ShowStatistics(info.Response[0]);
            }
            else
            {
                for (int i = 0; i < info.Response.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {info.Response[i]}");
                }
                if (info.Response.Length > 1)
                {
                    Console.Write("Qual país deseja ver os dados?: ");
                    int escolha;

                    while (!int.TryParse(Console.ReadLine(), out escolha))
                    {
                        Console.Write("Digite um valor válido: ");
                    }

                    //escolha = escolha - 1;
                    escolha--;

                    if (escolha >= info.Response.Length || escolha < 0)
                    {
                        Console.WriteLine("Número inválido");
                        return;
                    }
                    else
                        await ShowStatistics(info.Response[escolha]);
                }
                else
                {
                    Console.WriteLine("Sem estatísitica do país pesquisado");
                    return;
                }
            }
        }
        static async Task<StatisticsModel> GetEstatistica(string country = null)
        {
            var client = new HttpClient();

            var url = Url + "/statistics";

            if (!string.IsNullOrEmpty(country))
                url += $"?country={country}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-RapidAPI-Key", Key },
                    { "X-RapidAPI-Host", "covid-193.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(body);
                StatisticsModel info = JsonConvert.DeserializeObject<StatisticsModel>(body);
                return info;
            }
        }

        // Exibe as estatisticas de um pais informado pelo parametro country
        static async Task ShowStatistics(string country = null)
        {
            //country = escolha;
            StatisticsModel info = await GetEstatistica(country);

            foreach (var item in info.Response)
            {
                string continente = item.Continent == null ? "Sem dados para apresentar" : item.Continent;
                //string continente = item.Continent ?? "Sem dados para apresentar";
                string populacao = item.Population == null ? "Sem dados para apresentar" : item.Population?.ToString("N0");
                //string populacao = item.Population?.ToString("N0") ?? "Sem dados para apresentar";
                string casosNovos = item.Cases.New == null ? "Sem dados para apresentar" : item.Cases.New?.ToString("N0");
                string casosAtivos = item.Cases.Active == null ? "Sem dados para apresentar" : item.Cases.Active?.ToString("N0");
                string casosCriticos = item.Cases.Critical == null ? "Sem dados para apresentar" : item.Cases.Critical?.ToString("N0");
                string casosRecuperados = item.Cases.Recovered == null ? "Sem dados para apresentar" : item.Cases.Recovered?.ToString("N0");
                string casosMpess = item.Cases.M1Pop == null ? "Sem dados para apresentar" : item.Cases.M1Pop?.ToString("N0");
                string casosTotal = item.Cases.Total == null ? "Sem dados para apresentar" : item.Cases.Total?.ToString("N0");

                Console.WriteLine($"Continente: {continente}" +
                                  $"\nPaís: {item.Country}" +
                                  $"\npopulação: {populacao}" + //'?' se a população for nula o ToString não executa
                                  $"\nCasos:" +
                                  $"\n   Novos: {casosNovos}" +
                                  $"\n   Ativos: {casosAtivos}" +
                                  $"\n   Crítico: {casosCriticos}" +
                                  $"\n   Recuperado: {casosRecuperados}" +
                                  $"\n   a cada 1M Pessoas: {casosMpess}" +
                                  $"\n   Total: {casosTotal}" +
                                  $"\nMortes:\n{item.Deaths}" +
                                  $"\nTestes:\n{item.Tests}" +
                                  $"\nData e hora: {item.Time.ToString("yyyy/MM/dd HH:mm:ss")}\n");
            }

        }
        static async Task<StatisticsModel> GetHistorico(string country, DateTime? date = null)
        {
            var client = new HttpClient();

            var url = Url + "/history";

            if (!string.IsNullOrEmpty(country))
            {
                url += $"?country={country}";

                if (date != null)
                {
                    url += $"&day={date?.ToString("yyyy-MM-dd")}";
                }
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "X-RapidAPI-Key", Key },
                    { "X-RapidAPI-Host", "covid-193.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(body);
                StatisticsModel info = JsonConvert.DeserializeObject<StatisticsModel>(body);
                return info;
            }
        }
        static async Task ShowHistorico(string country, DateTime? date = null)
        {
            //DateTime data2 = new DateTime(2020, 03, 21);

            //if (date < data2) ou 
            //if (DateTime.Compare(date, data2) < 0)
            //{
            //    Console.WriteLine("Sem dados para apresentar");
            //    return;
            //}

            StatisticsModel info = await GetHistorico(country, date);

            if (info.Response.Length == 0)
            {
                Console.WriteLine("Sem dados para apresentar");
                return;
            }

            string continente = info.Response[0].Continent == null ? "Sem dados para apresentar" : info.Response[0].Continent;
            string populacao = info.Response[0].Population == null ? "Sem dados para apresentar" : info.Response[0].Population?.ToString("N0");
            string casosNovos = info.Response[0].Cases.New == null ? "Sem dados para apresentar" : info.Response[0].Cases.New?.ToString("N0");
            string casosAtivos = info.Response[0].Cases.Active == null ? "Sem dados para apresentar" : info.Response[0].Cases.Active?.ToString("N0");
            string casosCriticos = info.Response[0].Cases.Critical == null ? "Sem dados para apresentar" : info.Response[0].Cases.Critical?.ToString("N0");
            string casosRecuperados = info.Response[0].Cases.Recovered == null ? "Sem dados para apresentar" : info.Response[0].Cases.Recovered?.ToString("N0");
            string casosMpess = info.Response[0].Cases.M1Pop == null ? "Sem dados para apresentar" : info.Response[0].Cases.M1Pop?.ToString("N0");
            string casosTotal = info.Response[0].Cases.Total == null ? "Sem dados para apresentar" : info.Response[0].Cases.Total?.ToString("N0");

            Console.WriteLine($"Continente: {continente}" +
                              $"\nPaís: {info.Response[0].Country}" +
                              $"\nPopulação: {populacao}" +
                              $"\nCasos:" +
                              $"\n   Novos: {casosNovos}" + // abreviação
                              $"\n   Ativos: {casosAtivos}" +
                              $"\n   Crítico: {casosCriticos}" +
                              $"\n   Recuperado: {casosRecuperados}" +
                              $"\n   a cada 1M Pessoas: {casosMpess}" +
                              $"\n   Total: {casosTotal}" +
                              $"\nMortes:\n{info.Response[0].Deaths}" +
                              $"\nTestes:\n{info.Response[0].Tests}" +
                              $"\nData e hora: {info.Response[0].Time:yyyy/MM/dd HH:mm:ss}\n"); // abreviação

        }
        static async Task MostrarPais(string country = null)
        {
            CountryModel info = await GetCountries(country);

            for (int i = 0; i < info.Response.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {info.Response[i]}");
            }
        }
        static async Task ShowCountryHistory(string country)
        {
            // Buscar pelos países através da entrada do usuário

            CountryModel info = await GetCountries(country);

            // Busca pelos países. Sem resultado, exibir mensagem
            if (info.Response.Length == 0)
            {
                Console.WriteLine("Sem histórico do país pesquisado");
                return;
            }

            string paisEscolhido;

            // Mais de um resultado, pede para o usuário escolher
            if (info.Response.Length > 1)
            {
                for (int i = 0; i < info.Response.Length; i++)
                {
                    Console.WriteLine($"{i + 1} - {info.Response[i]}");
                }

                Console.Write("Qual país deseja ver os dados?: ");
                int escolha;

                while (!int.TryParse(Console.ReadLine(), out escolha))
                {
                    Console.Write("Digite um valor válido: ");
                }

                //escolha = escolha - 1;
                escolha--;

                if (escolha >= info.Response.Length || escolha < 0)
                {
                    Console.WriteLine("Número inválido");
                    return;
                }

                paisEscolhido = info.Response[escolha];

            }
            else
            {
                // Só um resultado, não precisa escolher
                paisEscolhido = info.Response[0];
            }

            // Pede para inserir a data
            Console.Write("Digite uma data (formato dd/MM/yyyy): ");

            DateTime date;

            while (true)
            {
                string entrada = Console.ReadLine(); //digitar a data

                if (string.IsNullOrEmpty(entrada)) //se for nula
                {
                    date = DateTime.Now; //aplica a data ataul
                    break;
                }
                else if (DateTime.TryParseExact(entrada, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) //conversão de string para DateTime
                {
                    break;
                }
                else
                    Console.Write("Digite uma valor válido: ");
            }

            Console.WriteLine($"Histórico do país '{paisEscolhido}' na data '{date.ToString("dd/MM/yyyy")}':");
            await ShowHistorico(paisEscolhido, date);
            return;

        }
        static async Task GetContinents()
        {
            StatisticsModel info = await GetEstatistica();

            var list = new List<string>(); //List<string>(var) list verificar Task<List<string>>
            int posicao = 1;
            foreach (var item in info.Response)
            {
                // Só vai inserir na list caso o continente não esteja na list
                if (!list.Contains(item.Continent))
                {
                    if (string.IsNullOrEmpty(item.Continent) || item.Continent == "All")
                    {
                        continue; //pula pra próx interação do foreach
                    }

                    list.Add(item.Continent);
                    Console.WriteLine($"{posicao} - {item.Continent}");
                    posicao++;
                }
            }

            Console.Write("\nEscolha um continente para ver os dados: ");

            int continente;

            while (!int.TryParse(Console.ReadLine(), out continente))
                Console.Write("Digite um valor válido: ");

            if (continente >= list.Capacity - 1 || continente < 0)
            {
                Console.Write("Digite uma valor válido: ");
                return;
            }

            string continenteEscolhido = list[continente - 1];
            await ShowContinentValues(continenteEscolhido);
        }
        static async Task ShowContinentValues(string continent)
        {
            //pegar a informação do continente selecionado
            //somar as informações necessárias
            //printar as informações

            StatisticsModel info = await GetEstatistica();

            int somapop = 0;
            int somanovos = 0;
            int somaativo = 0;
            int somacritico = 0;
            int somarecuperado = 0;
            double somacaso1M = 0;
            int somacasototal = 0;

            int somamortesnovas = 0;
            double somamortes1M = 0;
            int somamortestotais = 0;

            double somatestes1M = 0;
            int somatestetotais = 0;

            foreach (var item in info.Response)
            {
                if (continent == item.Continent && continent != item.Country)
                {
                    int pop = item.Population.HasValue ? item.Population.Value : 0; //hasvalue verifica se possui valor diferente de nulo, se sim pega o valor de population.value, se não pega  valor 0
                    int casonovo = item.Cases.New ?? 0; // se item.case.new for nulo pega o valor 0, se não pega o valor de item.case.new
                    int casoativo = item.Cases.Active.HasValue ? item.Cases.Active.Value : 0;
                    int casocritico = item.Cases.Critical ?? 0;
                    int casorecuperado = item.Cases.Recovered ?? 0;
                    double caso1Mpes = item.Cases.M1Pop ?? 0;
                    int casototal = item.Cases.Total ?? 0;

                    int mortenova = item.Deaths.New ?? 0;
                    double morte1Mpes = item.Deaths.M1Pop ?? 0;
                    int mortetotais = item.Deaths.Total ?? 0;

                    double teste1Mpes = item.Tests.M_pop ?? 0;
                    int testetotal = item.Tests.Total ?? 0;

                    somapop += pop;
                    somanovos += casonovo;
                    somaativo += casoativo;
                    somacritico += casocritico;
                    somarecuperado += casorecuperado;
                    somacaso1M += caso1Mpes;
                    somacasototal += casototal;

                    somamortesnovas += mortenova;
                    somamortes1M += morte1Mpes;
                    somamortestotais += mortetotais;

                    somatestes1M += teste1Mpes;
                    somatestetotais += testetotal;
                }

            }
            Console.WriteLine($"\nContinente: {continent}" +
                                      $"\nPopulação total: {somapop.ToString("N0")}" +
                                      $"\nCASOS" +
                                      $"\n   Casos novos totais: {somanovos.ToString("N0")}" +
                                      $"\n   Casos ativos totais: {somaativo.ToString("N0")}" +
                                      $"\n   Casos criticos totais: {somacritico.ToString("N0")}" +
                                      $"\n   Casos recuperados totais: {somarecuperado.ToString("N0")}" +
                                      $"\n   Casos por 1M de possoas totais: {somacaso1M.ToString("N0")}" +
                                      $"\n   Casos totais: {somacasototal.ToString("N0")}" +
                                      $"\nMORTES" +
                                      $"\n    Mortes novas totais: {somamortesnovas.ToString("N0")}" +
                                      $"\n    Mortes por 1M pessoas totais: {somamortes1M.ToString("N0")}" +
                                      $"\n    Mortes totais: {somamortestotais.ToString("N0")}" +
                                      $"\nTESTES" +
                                      $"\n   Testes por 1M pessoas totais: {somatestes1M.ToString("N0")}" +
                                      $"\n   Testes totais: {somatestetotais.ToString("N0")}\n");
        }
    }
}