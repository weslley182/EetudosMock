using mock.dominio;
using mock.infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.servico
{
    public class GeradorDePagamento
    {
        private LeilaoDaoFalso leilaoDao;
        private PagamentoDao pagamentoDao;
        private Avaliador avaliador;
        private IRelogio relogio;

        public GeradorDePagamento(LeilaoDaoFalso leilaoDao, Avaliador avaliador, PagamentoDao pagamentoDao)
        {
            this.leilaoDao = leilaoDao;
            this.avaliador = avaliador;
            this.pagamentoDao = pagamentoDao;
            this.relogio = new RelogioDoSistema();
        }

        public GeradorDePagamento(LeilaoDaoFalso leilaoDao, Avaliador avaliador, PagamentoDao pagamentoDao, IRelogio relogio)
        {
            this.leilaoDao = leilaoDao;
            this.avaliador = avaliador;
            this.pagamentoDao = pagamentoDao;
            this.relogio = relogio;
        }


        public virtual void gera()
        {
            List<Leilao> encerrados = leilaoDao.encerrados();
            foreach (var l in encerrados)
            {
                this.avaliador.avalia(l);

                Pagamento pagamento = new Pagamento(this.avaliador.maiorValor, proximoDiaUtil());

                this.pagamentoDao.salva(pagamento);
            }
        }

        public virtual DateTime proximoDiaUtil()
        {
            DateTime data = this.relogio.hoje();

            DayOfWeek diaDaSemana = data.DayOfWeek;

            if (diaDaSemana == DayOfWeek.Saturday)
            {
                data = data.AddDays(2);
            }
            else if (diaDaSemana == DayOfWeek.Sunday)
            {
                data = data.AddDays(1);
            }

            return data;
        }
    }
}
