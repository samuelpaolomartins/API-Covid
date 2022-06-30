using System;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;

namespace APICovid
{
    internal class Program
    {
        // por milhão = total * 1000000 / populacao
        //                    total * 1000000
        // por milhão = --------------------------------
        //                      populacao 

        static async Task Main(string[] args)
        {
            /*int[] n1 = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
            int[] n = new int[10];

            //int num = Convert.ToInt32(Console.ReadLine());

            for (int i = 9; i >=0; i--)
            {
                Console.WriteLine(n1[i]);
            }

            Console.ReadKey();
            return;
*/
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
                    case Opcoes.VisualizarRanking:
                        await Ranking();
                        break;
                    default:
                        Console.WriteLine("Opção não existe");
                        break;
                }
            }
        }
        static async Task ShowCountries(string country = null)
        {
            CountryModel info = await MetodosApi.GetCountries(country);

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
        // Exibe as estatisticas de um pais informado pelo parametro country
        static async Task ShowStatistics(string country = null)
        {
            //country = escolha;
            StatisticsModel info = await MetodosApi.GetEstatistica(country);

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
        static async Task ShowHistorico(string country, DateTime? date = null)
        {
            //DateTime data2 = new DateTime(2020, 03, 21);

            //if (date < data2) ou 
            //if (DateTime.Compare(date, data2) < 0)
            //{
            //    Console.WriteLine("Sem dados para apresentar");
            //    return;
            //}

            StatisticsModel info = await MetodosApi.GetHistorico(country, date);

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
            CountryModel info = await MetodosApi.GetCountries(country);

            for (int i = 0; i < info.Response.Length; i++)
            {
                Console.WriteLine($"{i + 1} - {info.Response[i]}");
            }
        }
        static async Task ShowCountryHistory(string country)
        {
            // Buscar pelos países através da entrada do usuário
            CountryModel info = await MetodosApi.GetCountries(country);

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
            StatisticsModel info = await MetodosApi.GetEstatistica();

            var list = new List<string>(); //List<string>(var) list
            int posicao = 1;
            foreach (var item in info.Response)
            {
                // Só vai inserir na list caso o continente não esteja na list
                if (!list.Contains(item.Continent))
                {
                    if (string.IsNullOrEmpty(item.Continent) || item.Continent == "All")
                        continue; //pula pra próx interação do foreach

                    list.Add(item.Continent);
                    Console.WriteLine($"{posicao} - {item.Continent}");
                    posicao++;
                }
            }

            Console.Write("\nEscolha um continente para ver os dados: ");

            int continente;

            while (!int.TryParse(Console.ReadLine(), out continente))
                Console.Write("Digite um valor válido: ");

            if (continente >= list.Capacity - 1 || continente <= 0)
            {
                Console.Write("Digite uma valor válido: ");
                return;
            }

            string continenteEscolhido = list[continente - 1];

            double somaPop = 0;
            int somaNovos = 0;
            int somaAtivo = 0;
            int somaCritico = 0;
            int somaRecuperado = 0;
            double somaCasoTotal = 0;

            int somaMortesNovas = 0;
            double somaMortesTotais = 0;

            double somaTesteTotais = 0;

            foreach (var item in info.Response)
            {
                if (continenteEscolhido == item.Continent && continenteEscolhido != item.Country)
                {
                    double pop = item.Population.HasValue ? item.Population.Value : 0; //hasvalue verifica se possui valor diferente de nulo, se sim pega o valor de population.value, se não pega  valor 0
                    int casonovo = item.Cases.New ?? 0; // se item.case.new for nulo pega o valor 0, se não pega o valor de item.case.new
                    int casoativo = item.Cases.Active.HasValue ? item.Cases.Active.Value : 0;
                    int casocritico = item.Cases.Critical ?? 0;
                    int casorecuperado = item.Cases.Recovered ?? 0;
                    double casototal = item.Cases.Total ?? 0;

                    int mortenova = item.Deaths.New ?? 0;
                    double mortetotais = item.Deaths.Total ?? 0;

                    double testetotal = item.Tests.Total ?? 0;

                    somaPop += pop;
                    somaNovos += casonovo;
                    somaAtivo += casoativo;
                    somaCritico += casocritico;
                    somaRecuperado += casorecuperado;
                    somaCasoTotal += casototal;

                    somaMortesNovas += mortenova;
                    somaMortesTotais += mortetotais;

                    somaTesteTotais += testetotal;
                }
            }

            double caso1Mpes = somaCasoTotal * 1000000 / somaPop;
            double morte1Mpes = somaMortesTotais * 1000000 / somaPop;
            double teste1Mpes = somaTesteTotais * 1000000 / somaPop;

            Console.WriteLine($"\nContinente: {continenteEscolhido}" +
                                      $"\nPopulação total: {somaPop:N0}" +
                                      $"\nCASOS" +
                                      $"\n   Casos novos totais: {somaNovos:N0}" +
                                      $"\n   Casos ativos totais: {somaAtivo:N0}" +
                                      $"\n   Casos criticos totais: {somaCritico:N0}" +
                                      $"\n   Casos recuperados totais: {somaRecuperado:N0}" +
                                      $"\n   Casos por 1M de possoas totais: {caso1Mpes:N0}" +
                                      $"\n   Casos totais: {somaCasoTotal:N0}" +
                                      $"\nMORTES" +
                                      $"\n    Mortes novas totais: {somaMortesNovas:N0}" +
                                      $"\n    Mortes por 1M pessoas totais: {morte1Mpes:N0}" +
                                      $"\n    Mortes totais: {somaMortesTotais:N0}" +
                                      $"\nTESTES" +
                                      $"\n   Testes por 1M pessoas totais: {teste1Mpes:N0}" +
                                      $"\n   Testes totais: {somaTesteTotais:N0}\n");
        }
        static async Task Ranking()
        {
            Console.Write("1 - Casos totais" +
                          "\n2 - Casos recuperados" +
                          "\n3 - Mortes totais" +
                          "\n4 - Testes totais" +
                          "\nEsolha uma opção de ranking: ");

            int escolha = Convert.ToInt32(Console.ReadLine());
            Opcoes2 opcao = (Opcoes2)escolha;
            Console.WriteLine();

            StatisticsModel info = await MetodosApi.GetEstatistica();

            switch (opcao)
            {
                case Opcoes2.CasoTotal:
                    {
                        InfoResponse[] valores = new InfoResponse[10];

                        for (int i = 0; i < valores.Length; i++)
                        {
                            valores[i] = new InfoResponse();

                            for (int j = 0; j < info.Response.Length; j++)
                            {
                                var item = info.Response[j];

                                if (info.Response[j].Continent != info.Response[j].Country)
                                {
                                    if (item.Cases.Total == null)
                                        continue;


                                    if (i == 0 && valores[i].Dados < item.Cases.Total)
                                    {
                                        valores[i].Dados = item.Cases.Total.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }

                                    if (i > 0 && valores[i - 1].Dados > item.Cases.Total && valores[i].Dados < item.Cases.Total)
                                    {
                                        valores[i].Dados = item.Cases.Total.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < valores.Length; i++)
                        {
                            Console.WriteLine($"Casos totais: {valores[i].Dados} do país: {valores[i].Pais}");
                        }
                    }
                    break;
                case Opcoes2.CasoRecuperado:
                    {
                        InfoResponse[] valores = new InfoResponse[10];

                        for (int i = 0; i < valores.Length; i++)
                        {
                            valores[i] = new InfoResponse();

                            for (int j = 0; j < info.Response.Length; j++)
                            {
                                var item = info.Response[j];

                                if (info.Response[j].Continent != info.Response[j].Country)
                                {
                                    if (item.Cases.Recovered == null)
                                        continue;

                                    if (i == 0 && valores[i].Dados < item.Cases.Recovered)
                                    {
                                        valores[i].Dados = item.Cases.Recovered.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }

                                    if (i > 0 && valores[i - 1].Dados > item.Cases.Recovered && valores[i].Dados < item.Cases.Recovered)
                                    {
                                        valores[i].Dados = item.Cases.Recovered.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < valores.Length; i++)
                        {
                            Console.WriteLine($"Casos recuperdaos: {valores[i].Dados} do país: {valores[i].Pais}");
                        }
                    }
                    break;
                case Opcoes2.MorteTotal:
                    {
                        InfoResponse[] valores = new InfoResponse[10];

                        //for pega o maior nº
                        for (int i = 0; i < valores.Length; i++)
                        {
                            valores[i] = new InfoResponse();

                            for (int j = 0; j < info.Response.Length; j++)
                            {
                                var item = info.Response[j];

                                if (info.Response[j].Continent != info.Response[j].Country)
                                {
                                    if (item.Deaths.Total == null)
                                        continue;

                                    if (i == 0 && valores[i].Dados < item.Deaths.Total)
                                    {
                                        valores[i].Dados = item.Deaths.Total.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }

                                    if (i > 0 && valores[i - 1].Dados > item.Deaths.Total && valores[i].Dados < item.Deaths.Total)
                                    {
                                        valores[i].Dados = item.Deaths.Total.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < valores.Length; i++)
                        {
                            Console.WriteLine($"Mortes totais: {valores[i].Dados} do país: {valores[i].Pais}");
                        }
                    }
                    break;
                case Opcoes2.TesteTotal:
                    {
                        InfoResponse[] valores = new InfoResponse[10];

                        //for pega o maior nº
                        for (int i = 0; i < valores.Length; i++)
                        {
                            valores[i] = new InfoResponse();

                            for (int j = 0; j < info.Response.Length; j++)
                            {
                                var item = info.Response[j];

                                if (info.Response[j].Continent != info.Response[j].Country)
                                {
                                    if (item.Tests.Total == null)
                                        continue;

                                    if (i == 0 && valores[i].Dados < item.Tests.Total)
                                    {
                                        valores[i].Dados = item.Tests.Total.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }

                                    if (i > 0 && valores[i - 1].Dados > item.Tests.Total && valores[i].Dados < item.Tests.Total)
                                    {
                                        valores[i].Dados = item.Tests.Total.Value;
                                        valores[i].Pais = info.Response[j].Country;
                                    }
                                }
                            }
                        }
                        for (int i = 0; i < valores.Length; i++)
                        {
                            Console.WriteLine($"Casos recuperdaos: {valores[i].Dados} do país: {valores[i].Pais}");
                        }
                    }
                    break;
                default:
                    Console.WriteLine("Opção não existe");
                    break;
            }
        }
    }
}