using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsupplyHHemo.Interface.Model
{
    public class RecebimentoModel
    {
        public string tokenid { get; set; }
        public string M0_CODFIL { get; set; } 
        public string M0_CODIGO { get; set; } 
        public string F1_CHVNFE { get; set; } 
        public string F1_COND { get; set; } 
        public string F1_DOC { get; set; } 
        public string F1_DTDIGIT { get; set; } 
        public string F1_EMISSAO { get; set; } 
        public string F1_ESPECIE { get; set; } 
        public string F1_FORMUL { get; set; } 
        public string F1_FORNECE { get; set; } 
        public string F1_LOJA { get; set; } 
        public string F1_SERIE { get; set; } 
        public string F1_TIPO { get; set; } 
        public string F1_XML { get; set; } 
        public string UUID_WEBSUPPLY { get; set; }
        public List<Anexo> ANEXOS { get; set; }
        public List<Item> PRENOTA_ITENS { get; set; }

        public class Anexo
        {
            public string ID_DOC { get; set; }
            public string DOC { get; set; }
            public string DOCX64 { get; set; }
        }

        public class Lote
        {
            public string LOTE_SERIE { get; set; }
            public decimal QUANT { get; set; }
            public string DATA_VALIDADE { get; set; }
        }

        public class Item
        {
            public string D1_CC { get; set; }
            public string D1_CONTA { get; set; }
            public string D1_DATPRF { get; set; }
            public decimal D1_DESPESA { get; set; }
            public string D1_ITEM { get; set; }
            public string D1_ITEMPC { get; set; }
            public string D1_OBS { get; set; }
            public decimal D1_PRECO { get; set; }
            public string D1_COD { get; set; }
            public string D1_PEDIDO { get; set; }
            public string D1_OPER { get; set; }
            public decimal D1_VALDESC { get; set; }
            public decimal D1_VUNIT { get; set; }
            public decimal D1_QUANT { get; set; }
            public decimal D1_SEGURO { get; set; }
            public decimal D1_TOTAL { get; set; }
            public decimal D1_VALFRE { get; set; }
            public decimal D1_VALICM { get; set; }
            public decimal D1_VALIPI { get; set; }
            public decimal D1_VLDESC { get; set; }
            public string D1_MSBLQL { get; set; }
            public string Tipo { get; set; }
            public int ID_ITEMP { get; set; }
            public List<Lote> LOTES { get; set; }
        }
    }
}
