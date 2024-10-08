using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebsupplyBrsupply.Interface.Model
{
    public class RetornoAPIModel
    {
        // Gerencia o Retorno da Interface de Pedido de Catalogo
        public class CadastraPedidoCatalogo
        {
            // XML Completo
            [XmlRoot("arquivo")]
            public class Arquivo
            {
                [XmlElement("info")]
                public Info info { get; set; }

                [XmlElement("processamento")]
                public Processamento processamento { get; set; } = new Processamento();

                [XmlElement("data")]
                public string Data { get; set; }

                [XmlElement("qtd-pedidos")]
                public string QtdPedidos { get; set; }
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

            public class Processamento
            {
                [XmlElement("pedido")]
                public Pedido pedido { get; set; } = new Pedido();
            }

            public class Pedido
            {
                [XmlElement("status")]
                public string Status { get; set; }

                [XmlElement("referencia")]
                public string Referencia { get; set; }

                [XmlElement("cnpj")]
                public string Cnpj { get; set; }

                [XmlElement("mensagem")]
                public string Mensagem { get; set; }

                [XmlElement("transacao")]
                public string Transacao { get; set; }
            }
        }

        // Gerencia o Retorno da Interface de Pedido de Catalogo
        public class CancelamentoPedidoCatalogo
        {
            // XML Completo
            [XmlRoot("arquivo")]
            public class Arquivo
            {
                [XmlElement("data")]
                public string Data { get; set; }

                [XmlElement("status")]
                public string Status { get; set; }

                [XmlElement("mensagem")]
                public string Mensagem { get; set; }
            }
        }
    }
}
