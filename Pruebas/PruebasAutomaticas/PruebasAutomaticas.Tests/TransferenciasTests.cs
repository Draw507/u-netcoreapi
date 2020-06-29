using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace PruebasAutomaticas.Tests
{
    [TestClass]
    public class TransferenciasTests
    {
        [TestMethod]
        public void TransferenciaInvalidaArrojaException()
        {
            // Preparaci�n
            Exception expectedException = null;
            Cuenta origen = new Cuenta() { Fondos = 0 };
            Cuenta destino = new Cuenta() { Fondos = 0 };
            decimal montoATransferir = 5m;
            var mock = new Mock<IServicioValidacionesDeTransferencias>();
            string mensajeDeError = "mensaje de error";
            mock.Setup(x => x.RealizarValidaciones(origen, destino, montoATransferir)).Returns(mensajeDeError);
            var servicio = new ServicioDeTransferencias(mock.Object);

            // Prueba
            try
            {
                servicio.TransferirEntreCuentas(origen, destino, montoATransferir);
                Assert.Fail("Un error debi� ser arrojado");
            }
            catch (Exception ex)
            {
                expectedException = ex;
            }

            // Verificaci�n
            Assert.IsTrue(expectedException is ApplicationException);
            Assert.AreEqual(mensajeDeError, expectedException.Message);
        }
    }
}
