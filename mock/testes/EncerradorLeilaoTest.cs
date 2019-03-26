using mock.dominio;
using mock.servico;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using mock.infra;

namespace mock.testes
{
    [TestFixture]
    class EncerradorLeilaoTest
    {
        [Test]
        public void deveEncerrarLeiloesQueComecaramUmaSemanaAnte()
        {
            DateTime data = new DateTime(2014, 05, 05);

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);
            Leilao leilao2 = new Leilao("Play 2");
            leilao2.naData(data);

            List<Leilao> leiloesAntigos = new List<Leilao>();
            leiloesAntigos.Add(leilao1);
            leiloesAntigos.Add(leilao2);
            // criando o mock
            var dao = new Mock<IRepositorioDeLeiloes>();
            // ensinando a retornar os leiloes antigos quando chamar o correntes
            dao.Setup(m => m.correntes()).Returns(leiloesAntigos);

            var carteiro = new Mock<Carteiro>();

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();


            Assert.AreEqual(2, leiloesAntigos.Count);
            Assert.IsTrue(leiloesAntigos[0].encerrado);
            Assert.IsTrue(leiloesAntigos[1].encerrado);
        }

        [Test]
        public void NaodeveEncerrarLeiloesQueComecaramOntem()
        {
            DateTime data = DateTime.Today;

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);
            Leilao leilao2 = new Leilao("Play 2");
            leilao2.naData(data);

            List<Leilao> listaRetorno = new List<Leilao>();
            listaRetorno.Add(leilao1);
            listaRetorno.Add(leilao2);

            var dao = new Mock<IRepositorioDeLeiloes>();
            dao.Setup(m => m.correntes()).Returns(listaRetorno);

            var carteiro = new Mock<Carteiro>();

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            Assert.AreEqual(0, encerrador.total);
            Assert.IsFalse(leilao1.encerrado);
            Assert.IsFalse(leilao2.encerrado);
        }

        [Test]
        public void naoDeveEncerarLeiloesSeNaoHouverNenhum()
        {
            var dao = new Mock<IRepositorioDeLeiloes>();
            dao.Setup(m => m.correntes()).Returns(new List<Leilao>());

            var carteiro = new Mock<Carteiro>();

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            Assert.AreEqual(0, encerrador.total);
        }

        [Test]
        public void deveEncerrarLeiloesQueComecaramUmaSemanaAntes()
        {
            DateTime data = new DateTime(2014, 05, 05);

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);
            Leilao leilao2 = new Leilao("Play 2");
            leilao2.naData(data);

            List<Leilao> leiloesAntigos = new List<Leilao>();
            leiloesAntigos.Add(leilao1);
            leiloesAntigos.Add(leilao2);
            // criando o mock
            var dao = new Mock<LeilaoDaoFalso>();
            // ensinando a retornar os leiloes antigos quando chamar o correntes
            dao.Setup(m => m.correntes()).Returns(leiloesAntigos);

            var carteiro = new Mock<Carteiro>();

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();


            Assert.AreEqual(2, leiloesAntigos.Count);
            Assert.IsTrue(leiloesAntigos[0].encerrado);
            Assert.IsTrue(leiloesAntigos[1].encerrado);
            dao.Verify(m => m.atualiza(leilao1), Times.Once());
            dao.Verify(m => m.atualiza(leilao2), Times.Once());
        }

        [Test]
        public void deveAtualizaOsLeiloesEncerrados()
        {
            DateTime data = new DateTime(2014, 05, 05);

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);

            List<Leilao> listaRetorno = new List<Leilao>();
            listaRetorno.Add(leilao1);

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(m => m.correntes()).Returns(listaRetorno);

            var carteiro = new Mock<Carteiro>();

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(m => m.atualiza(leilao1), Times.Once());
        }

        [Test]
        public void NaoDeveAtualizaOsLeiloesEncerrados()
        {
            DateTime data = DateTime.Now;

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);

            List<Leilao> listaRetorno = new List<Leilao>();
            listaRetorno.Add(leilao1);

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(m => m.correntes()).Returns(listaRetorno);
            var carteiro = new Mock<Carteiro>();

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            // verify aqui !
            dao.Verify(m => m.atualiza(leilao1), Times.Never());
        }

        [Test]
        public void deveContinuarAExecutarMesmoQuandoODaoFalha()
        {
            DateTime data = new DateTime(2014, 05, 05);

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);
            Leilao leilao2 = new Leilao("Play 2");
            leilao2.naData(data);

            List<Leilao> listaRetorno = new List<Leilao>();
            listaRetorno.Add(leilao1);
            listaRetorno.Add(leilao2);

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(m => m.correntes()).Returns(listaRetorno);

            var carteiro = new Mock<Carteiro>();

            dao.Setup(m => m.atualiza(leilao1)).Throws(new Exception());

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(m => m.atualiza(leilao2));
            carteiro.Verify(c => c.envia(leilao2));
        }

        [Test]
        public void deveContinuarAExecutarMesmoQuandoOCarteiroFalha()
        {
            DateTime data = new DateTime(2014, 05, 05);

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);
            Leilao leilao2 = new Leilao("Play 2");
            leilao2.naData(data);

            List<Leilao> listaRetorno = new List<Leilao>();
            listaRetorno.Add(leilao1);
            listaRetorno.Add(leilao2);

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(m => m.correntes()).Returns(listaRetorno);

            var carteiro = new Mock<Carteiro>();

            carteiro.Setup(m => m.envia(leilao1)).Throws(new Exception());

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            dao.Verify(m => m.atualiza(leilao2));
            carteiro.Verify(c => c.envia(leilao2));
        }

        [Test]
        public void deveDesistirQuandoODaoFalhaSempre()
        {
            DateTime data = new DateTime(2014, 05, 05);

            Leilao leilao1 = new Leilao("Tv 20 polegadas");
            leilao1.naData(data);
            Leilao leilao2 = new Leilao("Play 2");
            leilao2.naData(data);

            List<Leilao> listaRetorno = new List<Leilao>();
            listaRetorno.Add(leilao1);
            listaRetorno.Add(leilao2);

            var dao = new Mock<LeilaoDaoFalso>();
            dao.Setup(m => m.correntes()).Returns(listaRetorno);

            var carteiro = new Mock<Carteiro>();

            dao.Setup(m => m.atualiza(leilao1)).Throws(new Exception());
            dao.Setup(m => m.atualiza(leilao2)).Throws(new Exception());

            EncerradorDeLeilao encerrador = new EncerradorDeLeilao(dao.Object, carteiro.Object);
            encerrador.encerra();

            carteiro.Verify(c => c.envia(leilao1), Times.Never());
            carteiro.Verify(c => c.envia(leilao2), Times.Never());
            //usar 2 linha ou...
            carteiro.Verify(c => c.envia(It.IsAny<Leilao>()), Times.Never());
        }
    }
}

