using System;
using System.Xml.Serialization;

namespace WebsupplyBrsupply.Interface.Model
{
    public class PedidoCatalogoModel
    {
        // Gerencia a Request da Interface de Cadastro de Pedido de Catalogo
        public class CadastroPedido
        {
            // XML Completo
            [XmlRoot("arquivo")]
            public class Arquivo
            {
                [XmlElement("info")]
                public Info info { get; set; }

                [XmlElement("det-pedidos")]
                public DetPedidos detPedidos { get; set; }
            }

            // Estrutura de Classes do XML
            public class Info
            {
                [XmlElement("nome-remetente")]
                public string NomeRemetente { get; set; }

                [XmlElement("nome-destinatario")]
                public string NomeDestinatario { get; set; }

                [XmlElement("key")]
                public string Key { get; set; }

                [XmlElement("auth")]
                public string Auth { get; set; }
            }

            public class DetPedidos
            {
                [XmlElement("pedido")]
                public Pedido pedido { get; set; }
            }

            public class Pedido
            {
                [XmlElement("referencia")]
                public string Referencia { get; set; }

                [XmlElement("cnpj")]
                public string Cnpj { get; set; }

                [XmlElement("cod-local", IsNullable = false)]
                public string CodLocal { get; set; }

                [XmlElement("usuario", IsNullable = false)]
                public string Usuario { get; set; }

                [XmlElement("observacao")]
                public string Observacao { get; set; }

                [XmlElement("cod-categoria", IsNullable = false)]
                public string CodCategoria { get; set; }

                [XmlElement("det-itens")]
                public DetItens detItens { get; set; }
            }

            public class DetItens
            {
                [XmlElement("item")]
                public List<Item> Itens { get; set; }
            }

            public class Item
            {
                [XmlElement("cod-brsupply")]
                public string CodBrsupply { get; set; }

                [XmlElement("cod-cliente")]
                public string CodCliente { get; set; }

                [XmlElement("quantidade")]
                public string Quantidade { get; set; }

                [XmlElement("vlr-unitario")]
                public string VlrUnitario { get; set; }

                [XmlElement("ordem-item", IsNullable = false)]
                public string OrdemItem { get; set; }

                [XmlElement("sequencia-item")]
                public string SequenciaItem { get; set; }
            }
        }

        // Gerencia a Request da Interface de Cadastro de Pedido de Catalogo
        public class CancelamentoPedido
        {
            // XML Completo
            [XmlRoot("arquivo")]
            public class Arquivo
            {
                [XmlElement("info")]
                public Info info { get; set; }
            }

            // Estrutura de Classes do XML
            public class Info
            {
                [XmlElement("nome-remetente")]
                public string NomeRemetente { get; set; }

                [XmlElement("nome-destinatario")]
                public string NomeDestinatario { get; set; }

                [XmlElement("key")]
                public string Key { get; set; }

                [XmlElement("auth")]
                public string Auth { get; set; }

                [XmlElement("pedido", IsNullable = false)]
                public string Pedido { get; set; }

                [XmlElement("ordem-compra", IsNullable = false)]
                public string OrdemCompra { get; set; }

                [XmlElement("motivo")]
                public string Motivo { get; set; }
            }
        }

        // Gerencia a Request da Interface de Cadastro de Pedido de Catalogo
        public class ConsultaPedido
        {
            // XML Completo
            [XmlRoot("arquivo")]
            public class Arquivo
            {
                [XmlElement("info")]
                public Info info { get; set; }
            }

            // Estrutura de Classes do XML
            public class Info
            {
                [XmlElement("nome-remetente")]
                public string NomeRemetente { get; set; }

                [XmlElement("nome-destinatario")]
                public string NomeDestinatario { get; set; }

                [XmlElement("key")]
                public string Key { get; set; }

                [XmlElement("auth")]
                public string Auth { get; set; }

                [XmlElement("pedido", IsNullable = false)]
                public string Pedido { get; set; }

                [XmlElement("ordem-compra", IsNullable = false)]
                public string OrdemCompra { get; set; }
            }
        }
    }
}
