using SgiConnection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using WSComuns;

namespace WebsupplyBrsupply.Interface.Funcoes
{
    public class Class_Log_Brsupply : Class_Log
    {
        #region Declaração dos Campos/Variáveis privados

        private int _intCodEmpresa = 34;
        private string strIDTransacao { get; set; }
        private int intNumTransacao { get; set; }
        private int idServico { get; set; }
        private string strDados { get; set; }
        private int intIDErro { get; set; }
        private int intStatus { get; set; }
        private string strLocalOrigem { get; set; }
        private string strMetodoOrigem { get; set; }
        private Class_MensagemRetorno.Class_Erros[] arrDetalhe { get; set; }
        private string strAmbiente { get; set; }

        public string strErroRetorno { get; set; }

        private ArrayList _ArrOut;
        private ArrayList _ArrParam;

        #endregion

        public Class_Log_Brsupply(string _strIdentificador, int _intNumTransacao, int _intIdServico, int _intIDErro, int _Status, object _strConteudoSoap, Class_MensagemRetorno.Class_Erros[] _arrDetalhe, string _strDados,
                        string _strLocalOrigem, string _strChaveLocal, string _strChaveRemoto, string _strMetodoOrigem, string _strAmbiente = null)
        {
            strIDTransacao = _strIdentificador;
            intNumTransacao = _intNumTransacao;
            idServico = _intIdServico;
            intIDErro = _intIDErro;
            intStatus = _Status;
            strConteudoSoap = "";
            if (_strConteudoSoap == null) { strConteudoSoap = ""; }
            else if (_strConteudoSoap != "") { strConteudoSoap = Mod_Gerais.retornaConteudoSoap(_strConteudoSoap); }
            arrDetalhe = _arrDetalhe;
            strDados = _strDados;
            strLocalOrigem = _strLocalOrigem;
            strMetodoOrigem = _strMetodoOrigem;
            strChaveLocal = _strChaveLocal;
            strChaveRemoto = _strChaveRemoto;
            intCodEmpresa = _intCodEmpresa;
            strAmbiente = _strAmbiente;
        }

        public void GravaLog()
        {
            Conexao _Conn = new Conexao(Mod_Gerais.ConnectionString(strAmbiente));
            try
            {
                _ArrParam = new ArrayList();

                _ArrParam.Add(new Parametro("@iIdErro", intIDErro.ToString(), SqlDbType.Int, 4, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@iIdServico", idServico.ToString(), SqlDbType.Int, 4, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@vDados", strDados.ToString(), SqlDbType.VarChar, 800, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@tConteudoSoap", strConteudoSoap == "" ? null : strConteudoSoap, SqlDbType.Text, 0, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@vChaveLocal", strChaveLocal == "" ? null : strChaveLocal.ToString(), SqlDbType.VarChar, 50, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@vChaveRemoto", strChaveRemoto == "" ? null : strChaveRemoto.ToString(), SqlDbType.VarChar, 50, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@dDATA", DateTime.Now.ToString(), SqlDbType.DateTime, 8, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@vIDTransacao", strIDTransacao.ToString(), SqlDbType.VarChar, 50, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@iNumTransacao", intNumTransacao.ToString(), SqlDbType.SmallInt, 2, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@iSTATUS", intStatus.ToString(), SqlDbType.SmallInt, 2, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@cOrigem", strLocalOrigem == "" ? null : strLocalOrigem.ToString(), SqlDbType.Char, 1, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@cMetodoOrigem", strMetodoOrigem == "" ? null : strMetodoOrigem.ToString(), SqlDbType.VarChar, 60, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@iCodEmpresa", intCodEmpresa.ToString(), SqlDbType.Int, 4, ParameterDirection.Input));
                _ArrParam.Add(new Parametro("@iIDLOG", null, SqlDbType.Int, 4, ParameterDirection.Output));

                _ArrOut = new ArrayList();
                _Conn.ExecuteStoredProcedure(new StoredProcedure("SP_GERAL_WEBSERVICE_LOG_INS", _ArrParam), ref _ArrOut);

                int intIdLog = int.Parse(((Parametro)_ArrOut[0]).Valor.ToString());

                if (arrDetalhe != null)
                {
                    for (int i = 0; i < arrDetalhe.Length - 1; i++)
                    {
                        _ArrParam.Clear();
                        _ArrParam.Add(new Parametro("@iIDLog", intIdLog.ToString(), SqlDbType.Int, 4, ParameterDirection.Input));
                        _ArrParam.Add(new Parametro("@iCodErro", arrDetalhe[i].intNumErro.ToString(), SqlDbType.Int, 4, ParameterDirection.Input));
                        _ArrParam.Add(new Parametro("@vCategoria", "", SqlDbType.VarChar, 100, ParameterDirection.Input));
                        _ArrParam.Add(new Parametro("@vItemCategoria", "", SqlDbType.VarChar, 100, ParameterDirection.Input));
                        _ArrParam.Add(new Parametro("@vValor", arrDetalhe[i].strDescErro.ToString(), SqlDbType.VarChar, 800, ParameterDirection.Input));
                        _ArrOut.Clear();

                        _Conn.ExecuteStoredProcedure(new StoredProcedure("SP_WebServices_Log_Detalhe_INS", _ArrParam), ref _ArrOut);
                    }

                }
            }
            catch (Exception ex)
            {
                strErroRetorno = ex.Message.ToString();
            }
            finally
            {
                if (_Conn != null)
                {
                    _Conn.Dispose();
                    _Conn = null;
                }
            }
        }
    }

    public class Class_Erro_Brsupply
    {
        #region  Declaração dos Campos/Variáveis públicos
        public int intIDErro { get; set; }
        public string strDados { get; set; }
        #endregion
    }
}
