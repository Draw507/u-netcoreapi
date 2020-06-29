using System;
using System.Collections.Generic;
using System.Text;

namespace PruebasAutomaticas
{
    public interface IServicioValidacionesDeTransferencias
    {
        string RealizarValidaciones(Cuenta origen, Cuenta destino, decimal montoATransferir);
    }
}
