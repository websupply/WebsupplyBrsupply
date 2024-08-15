using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SgiConnection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebsupplyHHemo.Interface.Funcoes;
using WebsupplyHHemo.Interface.Model;

namespace WebsupplyHHemo.Interface.Metodos
{
    public class UsuarioMetodo
    {
        static int _intNumTransacao = 0;
        static int _intNumServico = 9;
        string strIdentificador = "Usr" + Mod_Gerais.RetornaIdentificador();

        public string strMensagem = string.Empty;


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
                                 "L", "", "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Parametros para Controle de Paginação
                int totalRegistros = 0;
                int linhaInicial = 0;
                int limiteRegistrosPagina = 100;
                int totalRegistrosPagina = 0;

                // Pega a URL do Serviço
                Class_Servico objServico = new Class_Servico();
                if (objServico.CarregaDados(_intNumServico, "", strIdentificador, intNumTransacao) == false)
                {
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                       1, -1, "", null, "Erro ao recuperar dados do serviço",
                                                       "", "", "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;
                    strMensagem = "Erro ao recuperar dados do serviço";
                    return false;
                }
                else
                { _intNumTransacao -= 1; }

                // Cria um laço para percorrer todas as linhas
                do
                {
                    // Define a Estrutura da Request
                    object requestBody = new
                    {
                        tokenid = "HH@2021!%",
                        M0_CODIGO = "01",
                        ROWINI = linhaInicial,
                        ROWLINES = limiteRegistrosPagina
                    };

                    // Serializa o objeto para JSON
                    string jsonRequestBody = JsonConvert.SerializeObject(requestBody);

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, jsonRequestBody, null, "Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", "", "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Adiciona o JSON como conteúdo da requisição
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                    // Carrega os Dados de Autenticação
                    var base64EncodedAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{objServico.strUsuario}:{objServico.strSenha}"));

                    // Define os parâmetros e cria a chamada
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
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
                                     "L", "", "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    response.EnsureSuccessStatusCode();

                    // Trata o Retorno e aloca no objeto
                    JArray retornoAPI = JArray.Parse(responseBody);

                    // Verifica se tem retorno
                    if (retornoAPI.Count > 0)
                    {
                        // Realiza a Chamada do Banco
                        Conexao conn = new Conexao(Mod_Gerais.ConnectionString());

                        // Percorre Todos os Resultados
                        for (int i = 0; i < retornoAPI.Count; i++)
                        {
                            // Pega a Linha do Retorno
                            JObject linhaRetorno = JObject.Parse(retornoAPI[i].ToString());

                            // Sincroniza o Retorno da API com a Classe de Gerenciamento
                            UsuarioModel usuario = new UsuarioModel
                            {
                                Email = linhaRetorno["USR_EMAIL"].ToString().Trim(),
                                NomeRequisitante = linhaRetorno["USR_NOME"].ToString().Trim(),
                                CodUsuario = linhaRetorno["USR_ID"].ToString().Trim(),
                                Usuario = linhaRetorno["USR_CODIGO"].ToString().Trim(),
                                Status = linhaRetorno["USR_MSBLQL"].ToString().Trim()
                            };

                            // Verifica se o Código do usuário foi enviado
                            // caso não, gera log de despejo 
                            if (usuario.CodUsuario == "" || usuario.CodUsuario == null)
                            {
                                // Gera Log com o retorno da API
                                objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                 0, (int)response.StatusCode, usuario.ToString(), null, "Usuário não inserido por falta de chave primária (USR_ID)",
                                                 "L", "", "", Mod_Gerais.MethodName());
                                objLog.GravaLog();
                                objLog = null;
                            }
                            else
                            {
                                // Cria o Parametro da query do banco
                                ArrayList arrParam = new ArrayList();

                                arrParam.Add(new Parametro("@vEmail", usuario.Email == "" ? null : usuario.Email.ToString(), SqlDbType.VarChar, 70, ParameterDirection.Input));
                                arrParam.Add(new Parametro("@vNomeRequisitante", usuario.NomeRequisitante == "" ? null : usuario.NomeRequisitante.ToString(), SqlDbType.VarChar, 40, ParameterDirection.Input));
                                arrParam.Add(new Parametro("@vCodigo", usuario.CodUsuario == "" ? null : usuario.CodUsuario.ToString(), SqlDbType.VarChar, 30, ParameterDirection.Input));
                                arrParam.Add(new Parametro("@cUsuario", usuario.Usuario == "" ? null : usuario.Usuario.ToString(), SqlDbType.Char, 33, ParameterDirection.Input));
                                arrParam.Add(new Parametro("@cStatus", usuario.Status == "" ? null : usuario.Status.ToString(), SqlDbType.Char, 1, ParameterDirection.Input));

                                ArrayList arrOut = new ArrayList();

                                conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHEMO_WS_USUARIO_INSUPDEXC", arrParam), ref arrOut);
                            }

                        }

                        // Encerra a Conexão com Banco de Dados
                        conn.Dispose();

                        // Registra o Total de Registros da Pagina
                        totalRegistrosPagina = retornoAPI.Count;

                        // Seta o Total de Registros
                        totalRegistros += totalRegistrosPagina;

                        // Atualiza a Paginação
                        linhaInicial = totalRegistros + 1;
                    }
                } while (totalRegistrosPagina == limiteRegistrosPagina);

                // Retorna a Mensagem de Sucesso
                if (totalRegistros > 0)
                {
                    strMensagem = $"{totalRegistros} Usuario(s) cadastrado(s)/atualizado(s) com sucesso";
                }
                else
                {
                    strMensagem = "Requisição concluída com sucesso sem dados retornados";
                }

                // Gera Log
                objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                 0, 0, "", null, strMensagem,
                                 "L", "", "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                return true;
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
                                 "L", "", "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Retorna Falso
                return false;
            }
        }
    }
}
