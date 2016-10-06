using NiboTest.Web.Data;
using NiboTest.Web.Exceptions;
using NiboTest.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace NiboTest.Web.FileImporters
{
    internal class OfxImporter : FileImporter
    {
        internal override void ImportarArquivo(string conteudo)
        {
            XDocument documentXml = new XDocument(new XDeclaration("1.0", "utf-16", "true"));
            string linha;

            using (StringReader stReader = new StringReader(conteudo))
            {
                XElement elementoPai = null;
                while ((linha = stReader.ReadLine()) != null)
                {
                    var linhaArray = linha.Split(new string[] { ">" }, StringSplitOptions.RemoveEmptyEntries);
                    var nomeTag = linhaArray[0].Trim().Remove(0, 1);


                    if (nomeTag.IndexOf("/") != -1)
                    {
                        if (elementoPai.Parent != null)
                            elementoPai = elementoPai.Parent;

                        continue;
                    }


                    string valorTag = null;

                    if (linhaArray.Length > 1)
                    {
                        valorTag = linhaArray[1].Trim();

                        XElement elementoNovo = new XElement(nomeTag, valorTag);

                        elementoPai.Add(elementoNovo);
                    }
                    else
                    {
                        XElement elementoNovo = new XElement(nomeTag);

                        if (elementoPai != null)
                            elementoPai.Add(elementoNovo);

                        elementoPai = elementoNovo;

                    }
                }

                documentXml.Add(elementoPai);
            }

            ImportarXmlBanco(documentXml);
        }


        private void ImportarXmlBanco(XDocument xml)
        {
            //Embora todos os arquivos tenham apenas uma tag com dados de cabeçalho,
            //optei por fazer em um loop, já que não possuo manuais ou certeza que sempre tenho uma conta única por extrato

            try
            {
                Context contextDb = DependencyResolver.Current.GetService<Context>();
                var elementosXml = XDocument.Parse(xml.ToString());

                var cabecalhosExtrato = elementosXml.Element("OFX").Elements()
                    .Where(x => x.Name == "BANKMSGSRSV1")
                    .Elements().Where(x => x.Name == "STMTTRNRS").Elements()
                    .Where(x => x.Name == "STMTRS");

                ExtratoBanco extrato = new ExtratoBanco();

                foreach (var cabecalho in cabecalhosExtrato)
                {
                    var dadosBancarios = cabecalho.Element("BANKACCTFROM");

                    if (dadosBancarios != null)
                    {
                        PreencherCabecalho(ref extrato, ref dadosBancarios);
                    }                    

                    var dadosTransacoes = cabecalho.Element("BANKTRANLIST");

                    if (dadosTransacoes != null)
                    {

                        extrato.DataInicio = DateTime.ParseExact(dadosTransacoes.Element("DTSTART").Value.Split('[')[0], "yyyyMMddHHmmss", CultureInfo.GetCultureInfo("pt-BR"));
                        extrato.DataFim = DateTime.ParseExact(dadosTransacoes.Element("DTEND").Value.Split('[')[0], "yyyyMMddHHmmss", CultureInfo.GetCultureInfo("pt-BR"));

                        //validação criada pois não foi mencionado se ao importar o mesmo extrato deve-se 
                        //excluir existente e importar novamente ou se deve validar.

                        ExtratoBanco extratoExistente = contextDb.ExtratoBanco.FirstOrDefault(x => x.CodigoBanco.Equals(extrato.CodigoBanco) && x.CodigoConta.Equals(extrato.CodigoConta) && x.DataInicio == extrato.DataInicio && x.DataFim == extrato.DataFim);
                        if (extratoExistente != null)
                            throw new OfxValidationException("Extrato já cadastrado.");

                        var transacoes = dadosTransacoes.Elements().Where(x => x.Name == "STMTTRN")
                            .Select(x => new Transacao()
                            {
                                TipoTransacao = x.Element("TRNTYPE") != null && x.Element("TRNTYPE").Value != null ? x.Element("TRNTYPE").Value : null,
                                DataTransacao = x.Element("DTPOSTED") != null ? DateTime.ParseExact(x.Element("DTPOSTED").Value.Split('[')[0], "yyyyMMddHHmmss", CultureInfo.GetCultureInfo("pt-BR")) : default(DateTime),
                                ValorTransacao = x.Element("TRNAMT") != null ? decimal.Parse(x.Element("TRNAMT").Value, CultureInfo.GetCultureInfo("en-US")) : default(decimal),
                                NumeroCheck = x.Element("CHECKNUM") != null ? decimal.Parse(x.Element("CHECKNUM").Value, CultureInfo.GetCultureInfo("en-US")) : default(decimal),
                                DescricaoTransacao = x.Element("MEMO") != null ? x.Element("MEMO").Value : null,
                                CodigoConta = extrato.CodigoConta,
                                CodigoBanco = extrato.CodigoBanco
                            }).ToList();

                        extrato.TransacoesExtrato = transacoes;
                    }

                    var dadosFinalExtrato = cabecalho.Element("LEDGERBAL");

                    if (dadosFinalExtrato != null)
                    {
                        extrato.SaldoFinal = dadosFinalExtrato.Element("BALAMT") != null ? decimal.Parse(dadosFinalExtrato.Element("BALAMT").Value, CultureInfo.GetCultureInfo("en-US")) : default(decimal);
                    }
                }                

                contextDb.ExtratoBanco.Add(extrato);
                contextDb.SaveChanges();
            }
            catch(OfxValidationException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new Exception("Erro insperado do servidor.");
            }
        }

        private void PreencherCabecalho(ref ExtratoBanco extrato, ref XElement dadosBancarios)
        {
            extrato.CodigoBanco = dadosBancarios.Element("BANKID").Value;
            extrato.CodigoConta = dadosBancarios.Element("ACCTID").Value;
            extrato.TipoExtrato = dadosBancarios.Element("ACCTTYPE").Value;

        }
    }


}