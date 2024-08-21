using WebsupplyBrsupply.Interface.Metodos;

namespace WebsupplyBrsupply.Teste
{
    [TestClass]
    public class Testes
    {
        [TestMethod]
        public void InterfaceIncluirPedidoCatalogo()
        {
            PedidoCatalogoMetodo pedidoCatalogo = new PedidoCatalogoMetodo();

            pedidoCatalogo.intCodPedCatWebsupply = 20900340;

            Console.WriteLine(pedidoCatalogo.IncluirPedido());
            Console.WriteLine(pedidoCatalogo.strMensagem);
        }
    }
}