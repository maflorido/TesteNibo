using NiboTest.Web.Models;
using System.Collections.Generic;
using System.Linq;

namespace NiboTest.Web.ViewModels.Extrato
{
    public class ListaExtratoTransacao
    {

        public ListaExtratoTransacao(ExtratoBanco extrato)
        {
            this.Banco = extrato.CodigoBanco;
            this.Conta = extrato.CodigoConta;
            this.DataInicioExtrato = extrato.DataInicio.ToString("dd/MM/yyyy HH:mm:ss");
            this.DataFimExtrato = extrato.DataFim.ToString("dd/MM/yyyy HH:mm:ss");
            this.SaldoFinal = extrato.SaldoFinal.ToString("C");

            this.TransacoesExtrato = extrato.TransacoesExtrato.Select(x => new TransacaoExtrato(x));
        }

        public string Banco { get; private set; }

        public string Conta { get; private set; }

        public string TipoExtrato { get; private set; }

        public string DataInicioExtrato { get; private set; }

        public string DataFimExtrato { get; private set; }

        public string SaldoFinal { get; private set; }

        public IEnumerable<TransacaoExtrato> TransacoesExtrato { get; private set; }
        
    }
}