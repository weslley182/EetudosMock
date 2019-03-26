using mock.dominio;
using mock.infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.servico
{
    public class EncerradorDeLeilao
    {
        public int total { get; private set; }
        private IRepositorioDeLeiloes dao;
        private Carteiro carteiro;


        public EncerradorDeLeilao(IRepositorioDeLeiloes dao, Carteiro carteiro)
        {
            this.dao = dao;
            this.carteiro = carteiro;
            total = 0;
        }

        public virtual void encerra()
        {            
            List<Leilao> todosLeiloesCorrentes = dao.correntes();

            foreach (var l in todosLeiloesCorrentes)
            {
                if (comecouSemanaPassada(l))
                {
                    try
                    {
                        l.encerra();
                        total++;
                        dao.atualiza(l);
                        // agora enviamos por email !
                        this.carteiro.envia(l);
                    }
                    catch (Exception e)
                    {
                        //salva um log
                    }
                }
            }
        }


        private bool comecouSemanaPassada(Leilao leilao)
        {
            return diasEntre(leilao.data, DateTime.Now) >= 7;
        }

        private int diasEntre(DateTime inicio, DateTime fim)
        {
            int dias = (int)(fim - inicio).TotalDays;

            return dias;
        }

    }
}
