﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mock.dominio
{
    class RelogioDoSistema : IRelogio
    {
        public DateTime hoje()
        {
            return DateTime.Today;
        }
    }
}
