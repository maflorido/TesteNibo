using System;

namespace NiboTest.Web.Models
{
    public class Transacao
    {
        public string CodigoBanco { get; set; }

        public string CodigoConta { get; set; }

        public string TipoTransacao { get; set; }

        public DateTime DataTransacao { get; set; }

        public Decimal ValorTransacao { get; set; }

        public string DescricaoTransacao { get; set; }

        public decimal NumeroCheck { get; set; }
    }
}