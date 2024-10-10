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
        // Gerencia o Retorno da Interface de Cadastro de Pedido de Catalogo
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

        // Gerencia o Retorno da Interface de Cancelamento de Pedido de Catalogo
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
        
        // Gerencia o Retorno da Interface de Consulta de Pedido de Catalogo
        public class ConsultaPedidoCatalogo
        {
            // XML Completo
            [XmlRoot("arquivo")]
            public class Arquivo
            {
                [XmlElement("info", IsNullable = false)]
                public Info info { get; set; }

                [XmlElement("dados", IsNullable = false)]
                public Dados dados { get; set; } = new Dados();

                [XmlElement("data", IsNullable = false)]
                public string Data { get; set; }

                [XmlElement("status", IsNullable = false)]
                public string Status { get; set; }

                [XmlElement("mensagem", IsNullable = false)]
                public string Mensagem { get; set; }
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

            public class Dados
            {
                [XmlElement("pedido")]
                public List<Pedido> pedido { get; set; } = new List<Pedido>();
            }

            public class Pedido
            {
                [XmlElement("numero")]
                public string Numero { get; set; }
                
                [XmlElement("ordem")]
                public string Ordem { get; set; }

                [XmlElement("cod-status")]
                public string CodStatus { get; set; }

                [XmlElement("status")]
                public string Status { get; set; }

                [XmlElement("observacao")]
                public string Observacao { get; set; }

                [XmlElement("notas-fiscais")]
                public NotasFiscais notasFiscais { get; set; } = new NotasFiscais();
            }

            public class NotasFiscais
            {
                [XmlElement("nfe")]
                public List<NotaFiscal> NFE { get; set; } = new List<NotaFiscal>();
            }

            public class NotaFiscal
            {
                [XmlElement("numero-nfe")]
                public string NumNFE { get; set; }

                [XmlElement("serie")]
                public string Serie { get; set; }

                [XmlElement("data-emissao")]
                public string DataEmissao { get; set; }

                [XmlElement("danfe")]
                public string Danfe { get; set; }
            }
        }
    }
}
