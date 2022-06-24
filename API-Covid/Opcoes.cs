using System;
using System.Collections.Generic;
using System.Text;

namespace APICovid
{
    public enum Opcoes
    {
        PaisesDisponiveis = 1,          //Se não atribuir valor começa em 0
        PesquisarPais = 2,              //Se não atribuir valor fica = 1
        VisualizarDadosPais = 3,        //Se não atribuir valor fica = 2
        VisualizarDadosPaisData = 4,    //Se não atribuir valor fica = 3
        VisualizarDadosMundo = 5,       //Se não atribuir valor fica = 4
        VisualizarDadosMundoData = 6,   //Se não atribuir valor fica = 5
        FecharAplicacao = 7,            //Se não atribuir valor fica = 6
        PesquisaContinente = 8
    }
}
