using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WSComuns;

namespace WebsupplyBrsupply.Interface.Funcoes
{
    public class Mod_Gerais
    {
        public static string strUsuario { get { return ""; } }
        public static string strSenha { get { return ""; } }
        public static string strCGC { get { return "03746938000143"; } }

        public const string strErrorMessage = "O campo [{0}] deve conter entre {2} e {1} caracteres.";
        public const string strDateTimeMessage = "O campo [{0}] não é uma data válida.";
        public const string strRequiredMessage = "O campo [{0}] deve ser preenchido.";
        public const string strMaxErrorMessage = "O campo [{0}] pode ter no máximo {1} caracteres.";

        public static string ConnectionString()
        {
            AssemblySettings settings = new AssemblySettings();
#if DEBUG
            return settings["appConexaoBrsupplyHom"].ToString();
#else
            return settings["appConexaoBrsupplyProd"].ToString();
#endif
        }

        public static string MethodName()
        {
            StackTrace _stack = new StackTrace();
            StackFrame _Frame = _stack.GetFrame(1);
            MethodBase _Method = _Frame.GetMethod();
            return _Method.DeclaringType.FullName + "." + _Method.Name + "()";
        }

        public static string PreencheCampo(string strValor, int intTamanho)
        {
            string strRetorno = "";
            if (strValor != null)
            {
                if (strValor.Trim().Length > 0)
                {
                    for (int intCarac = 0; intCarac < intTamanho - strValor.Length; intCarac++)
                    {
                        strRetorno = strRetorno + "0";
                    }
                }
                strRetorno = strRetorno + strValor;
            }
            return strRetorno;
        }

        public static string TrataNullTiraEspaco(object objValor)
        {
            string objRecebe = string.Empty;
            if (objValor != DBNull.Value)
            {
                try
                {
                    objRecebe = objValor.ToString().Trim();
                }
                catch (Exception)
                {
                    objRecebe = (string)objValor;
                }
            }
            return objRecebe;
        }

        public static string RetornaIdentificador()
        {
            try
            {
                string strRetorno;
                DateTime datDataAtual = DateTime.Now;
                strRetorno = PreencheCampo(datDataAtual.Year.ToString(), 4) +
                             PreencheCampo(datDataAtual.Month.ToString(), 2) +
                             PreencheCampo(datDataAtual.Day.ToString(), 2) +
                             PreencheCampo(datDataAtual.Hour.ToString(), 2) +
                             PreencheCampo(datDataAtual.Minute.ToString(), 2) +
                             PreencheCampo(datDataAtual.Second.ToString(), 2) +
                             PreencheCampo(datDataAtual.Millisecond.ToString(), 4);

                return strRetorno;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string retornaConteudoSoap(object objSerializer)
        {
            string strSOAP;
            MemoryStream MemStream;
            StreamWriter TextoWriter;
            StringBuilder StrBuilder;
            System.Xml.Serialization.XmlSerializer Serializer;
            StreamReader strReader;
            try
            {
                MemStream = new MemoryStream();
                TextoWriter = new StreamWriter(MemStream, Encoding.UTF8);
                StrBuilder = new StringBuilder();
                Serializer = new System.Xml.Serialization.XmlSerializer(objSerializer.GetType());
                Serializer.Serialize(TextoWriter, objSerializer);
                MemStream.Position = 0;
                strReader = new StreamReader(MemStream);
                StrBuilder.Append(strReader.ReadToEnd());
                strSOAP = StrBuilder.ToString();
                StrBuilder = null;
                strReader = null;
                Serializer = null;
                TextoWriter = null;
                MemStream = null;
            }
            catch (Exception)
            {
                strSOAP = "";
            }
            return strSOAP;
        }
        public static bool ValidaCampos(object _tpTipo, ref ArrayList _arrErros, bool _bllClass = true)
        {
            bool _objRetorno = true;
            _objRetorno = TrataCampos(_tpTipo, ref _arrErros);

            Type myType = _tpTipo.GetType();
            PropertyInfo[] myPropInfo = myType.GetProperties();

            foreach (PropertyInfo propriedade in myPropInfo)
            {
                if (propriedade.PropertyType.Namespace.ToUpper() != "SYSTEM")
                {
                    var _arrObject = propriedade.GetGetMethod().Invoke(_tpTipo, null);
                    if (_arrObject != null && _bllClass == true)
                    {
                        if (propriedade.PropertyType.IsArray)
                        {
                            foreach (object element in (Array)_arrObject)
                            {
                                if (!TrataCampos(element, ref _arrErros) && _objRetorno)
                                { _objRetorno = false; }
                            }
                        }
                        else
                        {
                            if (!TrataCampos(_arrObject, ref _arrErros) && _objRetorno)
                            { _objRetorno = false; }
                        }
                    }
                }
            }
            return _objRetorno;
        }

        private static bool TrataCampos(object _objValidar, ref ArrayList _arrErros)
        {
            ValidationContext _objContext = new ValidationContext(_objValidar, null, null);
            List<ValidationResult> _objResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(_objValidar, _objContext, _objResults, true);

            if (!isValid)
            {
                Class_MensagemRetorno.Class_Erros _objMsgErro;
                foreach (var validationResult in _objResults)
                {
                    _objMsgErro = new Class_MensagemRetorno.Class_Erros();
                    _objMsgErro.intNumErro = _arrErros.Count + 1;
                    _objMsgErro.strDescErro = validationResult.ErrorMessage;
                    _arrErros.Add(_objMsgErro);
                }
            }
            return isValid;
        }

        public static string ConsultaParametroConfig(string parametro)
        {
            AssemblySettings settings = new AssemblySettings();

            return settings[parametro].ToString();
        }

        public static string ArquivoParaBase64(string pathArquivo)
        {
            // Realiza a leitura do Arquivo
            Byte[] bytes = File.ReadAllBytes(pathArquivo);

            // Devole o base64
            return Convert.ToBase64String(bytes);
        }
    }
}
