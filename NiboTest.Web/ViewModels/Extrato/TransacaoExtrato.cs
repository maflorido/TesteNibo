using NiboTest.Web.Models;

namespace NiboTest.Web.ViewModels.Extrato
{
    public class TransacaoExtrato
    {
        public TransacaoExtrato(Transacao transacao)
        {
            this.TipoTransacao = transacao.TipoTransacao;
            this.DataTransacao = transacao.DataTransacao.ToString("dd/MM/yyyy HH:mm:ss");
            this.DescricaoTransacao = transacao.DescricaoTransacao;
            this.ValorTransacao = transacao.ValorTransacao.ToString("C");
        }

        public string TipoTransacao { get; private set; }

        public string DataTransacao { get; private set; }

        public string ValorTransacao { get; private set; }

        public string DescricaoTransacao { get; private set; }
    }
}