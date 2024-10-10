using WebsupplyBrsupply.Interface.Metodos;

namespace WebsupplyBrsupply.Teste
{
    [TestClass]
    public class Testes
    {
        [TestMethod]
        public void InterfaceIncluirPedidoCatalogo()
        {
            PedidoCatalogoMetodo.CadastraPedido pedidoCatalogo = new PedidoCatalogoMetodo.CadastraPedido();

            pedidoCatalogo.intCodPedCatWebsupply = 7988786;

            Console.WriteLine(pedidoCatalogo.Executar());
            Console.WriteLine(pedidoCatalogo.strMensagem);
            Console.WriteLine(pedidoCatalogo.strRetornoWS);
        }

        [TestMethod]
        public void InterfaceCancelarPedidoCatalogo()
        {
            PedidoCatalogoMetodo.CancelaPedido pedidoCatalogo = new PedidoCatalogoMetodo.CancelaPedido();

            pedidoCatalogo.strCodPedCatWebsupply = "20900340";
            pedidoCatalogo.strMotivoCancelamento = "Cancelar";

            Console.WriteLine(pedidoCatalogo.Executar());
            Console.WriteLine(pedidoCatalogo.strMensagem);
            Console.WriteLine(pedidoCatalogo.strRetornoWS);
        }

        [TestMethod]
        public void InterfaceConsultarPedidoCatalogo()
        {
            PedidoCatalogoMetodo.ConsultaPedido pedidoCatalogo = new PedidoCatalogoMetodo.ConsultaPedido();

            pedidoCatalogo.strCodPedCatWebsupply = "20900340";

            Console.WriteLine(pedidoCatalogo.Executar());
            Console.WriteLine(pedidoCatalogo.strMensagem);
            Console.WriteLine(pedidoCatalogo.strRetornoWS);
        }
    }
}