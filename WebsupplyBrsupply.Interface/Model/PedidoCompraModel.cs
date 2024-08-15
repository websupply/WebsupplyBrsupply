using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsupplyHHemo.Interface.Model
{
    public class PedidoCompraModel
    {
        public string tokenid { get; set; }
        public string M0_CODFIL { get; set; }
        public string M0_CODIGO { get; set; }
        public string C7_COND { get; set; }
        public decimal C7_DESPESA_TOTAL { get; set; }
        public string C7_EMISSAO { get; set; }
        public string C7_FILENT { get; set; }
        public string C7_FORNECE { get; set; }
        public string C7_LOJA { get; set; }
        public string C7_NUM { get; set; }
        public string UUID_WEBSUPPLY { get; set; }
        public decimal C7_SEGURO_TOTAL { get; set; }
        public decimal C7_VALFRE_TOTAL { get; set; }
        public decimal C7_VLDESC_TOTAL { get; set; }
        public string C7_TPFRETE { get; set; }
        public string C7_MSBLQL { get; set; }
        public List<Anexo> ANEXOS { get; set; }
        public List<Item> PEDIDO_ITENS { get; set; }

        public class Anexo
        {
            public string ID_DOC { get; set; }
            public string DOC { get; set; }
            public string DOCX64 { get; set; }
        }

        public class Item
        {
            public string C7_CC { get; set; }
            public string C7_CONTA { get; set; }
            public string C7_DATPRF { get; set; }
            public decimal C7_DESPESA { get; set; }
            public string C7_ITEM { get; set; }
            public string C7_OBS { get; set; }
            public decimal C7_PRECO { get; set; }
            public string C7_PRODUTO { get; set; }
            public decimal C7_QUANT { get; set; }
            public decimal C7_SEGURO { get; set; }
            public decimal C7_TOTAL { get; set; }
            public decimal C7_VALFRE { get; set; }
            public decimal C7_VALICM { get; set; }
            public decimal C7_VALIPI { get; set; }
            public decimal C7_VLDESC { get; set; }
            public string C7_MSBLQL { get; set; }
        }
    }
}
