using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsupplyHHemo.Interface.Model
{
    public class FornecedorModel
    {
        public string tokenid { get; set; } = "HH@2021!%";
        public string M0_CODIGO { get; set; } = "01";
        public string M0_CODFIL { get; set; }
        public string UUID_WEB { get; set; }
        public string A2_XTPFOR1 { get; set; }
        public string A2_COD { get; set; }
        public string A2_LOJA { get; set; }
        public string A2_TIPO { get; set; }
        public string A2_NOME { get; set; }
        public string A2_NREDUZ { get; set; }
        public string A2_CGC { get; set; }
        public string A2_INSCR { get; set; }
        public string A2_END { get; set; }
        public string A2_COMPLEM { get; set; }
        public string A2_MUN { get; set; }
        public string A2_EST { get; set; }
        public string A2_CEP { get; set; }
        public string A2_BAIRRO { get; set; }
        public string A2_COD_MUN { get; set; }
        public string A2_DDD { get; set; }
        public string A2_TEL { get; set; }
        public string A2_CONTATO { get; set; }
        public string A2_EMAIL { get; set; }
        public string A2_FORMPAG { get; set; }
        public string A2_TIPCTA { get; set; }
        public string A2_BANCO { get; set; }
        public string A2_AGENCIA { get; set; }
        public string A2_DVAGE { get; set; }
        public string A2_NUMCON { get; set; }
        public string A2_DVCTA { get; set; }
        public string A2_XCGCDEP { get; set; }
        public string A2_MSBLQL { get; set; }
        public List<Anexo> ANEXOS { get; set; }

        public class Anexo
        {
            public string ID_DOC { get; set; }
            public string DOC { get; set; }
            public string DOCX64 { get; set; }
        }
    }
}
