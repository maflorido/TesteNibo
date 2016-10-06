using NiboTest.Web.Data;
using NiboTest.Web.FileImporters;
using NiboTest.Web.Models;
using NiboTest.Web.ViewModels.Extrato;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;

namespace NiboTest.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportarArquivos(HttpPostedFileBase file1)
        {
            //Criei uma pequena fábrica abstrata para demonstrar conhecimento de padrões de projeto
            //não que houvesse real necessidade disso neste caso.
            try
            {
                using (StreamReader st = new StreamReader(file1.InputStream, Encoding.ASCII))
                {
                    var arquivoStr = st.ReadToEnd();
                    var arquivoTratadoXml = arquivoStr.Substring(arquivoStr.IndexOf("<OFX>"));

                    FileImporter fileImporter = null;

                    switch (Path.GetExtension(file1.FileName))
                    {
                        case ".ofx":
                            fileImporter = new OfxImporter();
                            break;
                        default:
                            break;
                    }

                    fileImporter.ImportarArquivo(arquivoTratadoXml);

                    ViewBag.Mensagem = "Arquivo Importado Com sucesso!";
                }
                
            }
            catch(Exception ex)
            {
                ViewBag.Erro = string.Concat("ERRO INESPERADO: ", ex.Message);
            }

            return View("Index");
        }

        [HttpGet]
        public ActionResult ListarTransacao()
        {
            Context dbContext = DependencyResolver.Current.GetService<Context>();

            var extratos = (from e in dbContext.ExtratoBanco.Include("TransacoesExtrato")
                            group e by new { e.CodigoBanco, e.CodigoConta } into grouped
                            select new
                            {
                                banco = grouped.Key.CodigoBanco,
                                conta = grouped.Key.CodigoConta,
                                dataInicial = grouped.Min(x => x.DataInicio),
                                dataFim = grouped.Max(x => x.DataFim),
                                saldoFinal = grouped.Sum(x => x.SaldoFinal),
                                transacoes = grouped.SelectMany(x => x.TransacoesExtrato).ToList(),
                            }).ToList().Select(x => new ExtratoBanco()
                            {
                                CodigoBanco = x.banco,
                                CodigoConta = x.conta,
                                DataInicio = x.dataInicial,
                                DataFim = x.dataFim,
                                SaldoFinal = x.saldoFinal,
                                TransacoesExtrato = x.transacoes
                            });
            
            var viewModel = extratos.Select(x => new ListaExtratoTransacao(x));
            return View("ListaExtratos", viewModel);
        }        
    }
}