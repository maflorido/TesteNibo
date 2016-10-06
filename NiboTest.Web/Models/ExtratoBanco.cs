using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NiboTest.Web.Models
{
    public class ExtratoBanco
    {
        public int Id { get; set; }

        public string CodigoBanco { get; set; }

        public string CodigoConta { get; set; }

        public string TipoExtrato { get; set; }

        public DateTime DataInicio { get; set; }

        public DateTime DataFim { get; set; }

        public ICollection<Transacao> TransacoesExtrato { get; set; }

        public decimal SaldoFinal { get; set; }
    }
}