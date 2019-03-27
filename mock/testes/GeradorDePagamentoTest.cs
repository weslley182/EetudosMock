using mock.dominio;
using mock.servico;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using mock.infra;

namespace mock.testes
{
    [TestFixture]
    class GeradorDePagamentoTeste
    {
        [Test]
        public void DeveGerarPagamentoParaLeilaoEncerrado()
        {

            var leilaoDao = new Mock<LeilaoDaoFalso>();
            var avaliador = new Mock<Avaliador>();
            var pagamentoDao = new Mock<PagamentoDao>();

            Leilao leilao1 = new Leilao("Playstation");
            leilao1.propoe(new Lance(new Usuario("Jorge"), 2000));
            leilao1.propoe(new Lance(new Usuario("Maria"), 2500));
            leilao1.naData(new DateTime(1999, 5, 1));

            List<Leilao> leiloes = new List<Leilao>();
            leiloes.Add(leilao1);

            leilaoDao.Setup(m => m.encerrados()).Returns(leiloes);
            avaliador.Setup(a => a.maiorValor).Returns(2500);

            Pagamento retorno = null;

            pagamentoDao.Setup(p => p.salva(It.IsAny<Pagamento>())).Callback<Pagamento>(r => retorno = r);

            //GeradorDePagamento gerador = new GeradorDePagamento(leilaoDao.Object, avaliador.Object, pagamentoDao.Object);
            GeradorDePagamento gerador = new GeradorDePagamento(leilaoDao.Object, new Avaliador(), pagamentoDao.Object);
            gerador.gera();

            Assert.AreEqual(2500, retorno.valor);
        }

        [Test]
        public void deveEmpurrarParaOProximoDiaUtil()
        {
            var leilaoDao = new Mock<LeilaoDaoFalso>();
            var pagamentoDao = new Mock<PagamentoDao>();
            var relogio = new Mock<IRelogio>();
            // estamos falando para o mock que hoje é sábado.
            relogio.Setup(r => r.hoje()).Returns(new DateTime(2019, 03, 23));

            Leilao leilao1 = new Leilao("Playstation");
            leilao1.propoe(new Lance(new Usuario("Renan"), 500));
            leilao1.propoe(new Lance(new Usuario("Felipe"), 1500));

            List<Leilao> leiloes = new List<Leilao>();
            leiloes.Add(leilao1);

            leilaoDao.Setup(l => l.encerrados()).Returns(leiloes);

            Pagamento pagamento = null;

            pagamentoDao.Setup(p => p.salva(It.IsAny<Pagamento>())).Callback<Pagamento>(r => pagamento = r);

            GeradorDePagamento gerador = new GeradorDePagamento(leilaoDao.Object, new Avaliador(), pagamentoDao.Object, relogio.Object);
            gerador.gera();

            Assert.AreEqual(DayOfWeek.Monday, pagamento.data.DayOfWeek);

        }
    }
}
