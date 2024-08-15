using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SgiConnection;
using System;
using System.Collections;
using System.Data;
using System.Text;
using WebsupplyHHemo.Interface.Funcoes;
using WebsupplyHHemo.Interface.Model;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebsupplyHHemo.Interface.Metodos
{
    public class ProdutosMetodo
    {
        static int _intNumTransacao = 0;
        static int _intNumServico = 15;
        string strIdentificador = "Prd" + Mod_Gerais.RetornaIdentificador();

        public string strMensagem = string.Empty;

        // Paramêtros de Controle da Classe
        public string strCodPrdWebsupply = string.Empty;
        public string strCodPrdProtheus = string.Empty;

        private static int intNumTransacao
        {
            get
            {
                _intNumTransacao += 1;
                return _intNumTransacao;
            }
            set
            {
                _intNumTransacao = value;
            }
        }

        public bool ConsomeWS()
        {
            bool retorno = false;
            Class_Log_Brsupply objLog;

            try
            {
                // Cria o Cliente Http
                HttpClient cliente = new HttpClient();

                // Gera Log
                objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                 0, 0, "", null, "Inicio do Método " + Mod_Gerais.MethodName(),
                                 "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Pega a URL do Serviço
                Class_Servico objServico = new Class_Servico();
                if (objServico.CarregaDados(_intNumServico, "", strIdentificador, intNumTransacao) == false)
                {
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                       1, -1, "", null, "Erro ao recuperar dados do serviço",
                                                       "", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;
                    strMensagem = "Erro ao recuperar dados do serviço";
                    return false;
                }
                else
                { _intNumTransacao -= 1; }

                // Realiza a Chamada do Banco
                Conexao conn = new Conexao(Mod_Gerais.ConnectionString());

                // Cria o Parametro da query do banco
                ArrayList arrParam = new ArrayList();

                arrParam.Add(new Parametro("@cCodProd", strCodPrdWebsupply, SqlDbType.Char, 10, ParameterDirection.Input));

                ArrayList arrOut = new ArrayList();
                DataTable DadosGrupoItens = conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHEMO_WS_Produtos_Sel", arrParam), ref arrOut).Tables[0];

                // Encerra a Conexão com Banco de Dados
                conn.Dispose();

                if (DadosGrupoItens.Rows.Count > 0)
                {
                    // Estrutura a Model
                    ProdutosModel gruposItens = new ProdutosModel
                    {
                        tokenid = DadosGrupoItens.Rows[0]["tokenid"].ToString().Trim(),
                        M0_CODIGO = DadosGrupoItens.Rows[0]["M0_CODIGO"].ToString().Trim(),
                        M0_CODFIL = DadosGrupoItens.Rows[0]["M0_CODFIL"].ToString().Trim(),
                        B1_COD = DadosGrupoItens.Rows[0]["B1_COD"].ToString().Trim(),
                        B1_DESC = DadosGrupoItens.Rows[0]["B1_DESC"].ToString().Trim(),
                        B1_DESC2 = DadosGrupoItens.Rows[0]["B1_DESC2"].ToString().Trim(),
                        B1_UM = DadosGrupoItens.Rows[0]["B1_UM"].ToString().Trim(),
                        B1_TIPO = DadosGrupoItens.Rows[0]["B1_TIPO"].ToString().Trim(),
                        B1_POSIPI = DadosGrupoItens.Rows[0]["B1_POSIPI"].ToString().Trim(),
                        B1_CONTA = DadosGrupoItens.Rows[0]["B1_CONTA"].ToString().Trim(),
                        B1_GRUPO = DadosGrupoItens.Rows[0]["B1_GRUPO"].ToString().Trim(),
                        B1_MSBLQL = DadosGrupoItens.Rows[0]["B1_MSBLQL"].ToString().Trim(),
                        UUID_WEBSUPPLY = DadosGrupoItens.Rows[0]["UUID_WEBSUPPLY"].ToString().Trim(),
                    };

                    // Realiza a Chamada do Banco
                    conn = new Conexao(Mod_Gerais.ConnectionString());

                    // Serializa o objeto para JSON
                    string jsonRequestBody = JsonConvert.SerializeObject(gruposItens);

                    // Atualiza o Identificador
                    strIdentificador = (gruposItens.B1_COD != String.Empty ? "Alt" : "Cad") + strIdentificador;

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, jsonRequestBody, null, "Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Adiciona o JSON como conteúdo da requisição
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                    // Carrega os Dados de Autenticação
                    var base64EncodedAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{objServico.strUsuario}:{objServico.strSenha}"));

                    // Define os parâmetros e cria a chamada
                    var request = new HttpRequestMessage
                    {
                        Method = gruposItens.B1_COD != String.Empty ? HttpMethod.Put : HttpMethod.Post,
                        RequestUri = new Uri(objServico.strURL),
                        Content = content,
                        Headers =
                                {
                                    Authorization = new AuthenticationHeaderValue($"{objServico.strTipoAutenticao}", base64EncodedAuth)
                                }
                    };

                    // Envia a requisição
                    var response = cliente.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

                    // Trata o Retorno da API
                    var responseBody = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                    // Gera Log com o retorno da API
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, (int)response.StatusCode, responseBody, null, "Retorno da Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();

                        // Trata o Retorno e aloca no objeto
                        JArray retornoAPI = JArray.Parse(responseBody);

                        // Verifica se tem retorno
                        if (retornoAPI.Count > 0)
                        {
                            // Percorre Todos os Resultados
                            for (int i = 0; i < retornoAPI.Count; i++)
                            {
                                // Pega a Linha do Retorno
                                JObject linhaRetorno = JObject.Parse(retornoAPI[i].ToString());

                                // Instância a model de controle do retorno da API
                                RetornoAPIModel retornoAPIModel = new RetornoAPIModel
                                {
                                    C_STATUS = linhaRetorno["C_STATUS"].ToString().Trim(),
                                    N_STATUS = (int)linhaRetorno["N_STATUS"]
                                };

                                // Verifica se retornou erro do protheus
                                // N_STATUS": 1 = Sucesso / 0 = Erro
                                if (retornoAPIModel.N_STATUS != 1)
                                {
                                    strMensagem = retornoAPIModel.C_STATUS;

                                    // Gera Log com o retorno da API
                                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                     0, (int)response.StatusCode, retornoAPIModel, null, "Erro no Retorno da Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                                     "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                                    objLog.GravaLog();
                                    objLog = null;

                                    return false;
                                }

                                // Sincroniza o Retorno da API com os Parametros
                                strCodPrdProtheus = linhaRetorno["B1_COD"].ToString().Trim();

                                // Valida se algum dos códigos retornou vázio
                                // caso sim, devolve erro
                                if (strCodPrdProtheus == String.Empty)
                                {
                                    strMensagem = $"Ocorreu um erro na chamada da aplicação - [{linhaRetorno["C_STATUS"].ToString().Trim()}] - B1_COD [{strCodPrdProtheus}]";

                                    return false;
                                }

                                // Armazena o Codigo do Produtos na base de dados
                                // caso não tenha recebido na consulta do banco.
                                if (gruposItens.B1_COD == String.Empty)
                                {
                                    // Realiza a Chamada do Banco
                                    conn = new Conexao(Mod_Gerais.ConnectionString());

                                    // Cria o Parametro da query do banco
                                    ArrayList arrParam2 = new ArrayList();

                                    arrParam2.Add(new Parametro("@cCodProd", strCodPrdWebsupply, SqlDbType.Char, 10, ParameterDirection.Input));
                                    arrParam2.Add(new Parametro("@cCodProdutoCompleto", strCodPrdProtheus, SqlDbType.Char, 20, ParameterDirection.Input));

                                    ArrayList arrOut2 = new ArrayList();

                                    conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHemo_WS_Produtos_CodProdutoCompleto_UPD", arrParam2), ref arrOut2);

                                    // Encerra a Conexão com Banco de Dados
                                    conn.Dispose();
                                }
                            }
                        }

                        // Define a mensagem de sucesso
                        strMensagem = $"Produto {gruposItens.B1_DESC2} do codigo [{(strCodPrdProtheus != String.Empty ? strCodPrdProtheus : gruposItens.B1_COD)}] {(gruposItens.B1_COD != String.Empty ? "atualizado(a)" : "cadastrado(a)")} com sucesso.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return true;
                    }
                    else
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Ocorreu o erro [{response.StatusCode}] ao processar a solicitação, verifique nos logs e tente novamente.";


                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, (int)response.StatusCode, "", null, strMensagem,
                                         "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }
                }
                else
                {
                    // Define a mensagem de erro
                    strMensagem = $"Não foi possível realizar a operação, pois não foi retornando nenhum dado referente ao Produto {strCodPrdWebsupply}";

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, "", null, strMensagem,
                                     "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    return false;
                }
            }
            catch (Exception ex)
            {
                // Inicializa a Model de Excepetion
                ExcepetionModel excepetionEstruturada = new ExcepetionModel(ex, true);

                // Estrutura o Erro
                strMensagem = excepetionEstruturada.Mensagem;

                // Gera Log
                objLog = new Class_Log_Hhemo(strIdentificador, intNumTransacao, _intNumServico,
                                 1, -1, JsonConvert.SerializeObject(excepetionEstruturada), null, strMensagem,
                                 "L", strCodPrdWebsupply.ToString(), "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Retorna Falso
                return false;
            }
        }
    }
}
