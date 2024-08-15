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
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Diagnostics;
using static WebsupplyHHemo.Interface.Model.RecebimentoModel;

namespace WebsupplyHHemo.Interface.Metodos
{
    public class RecebimentoMetodo
    {
        static int _intNumTransacao = 0;
        static int _intNumServico = 13;
        string strIdentificador = "Rec" + Mod_Gerais.RetornaIdentificador();

        public string strMensagem = string.Empty;

        // Paramêtros de Controle da Classe
        public int intCodRecComWebsupply = 0;
        public int intCodNFWebsupply = 0;
        public string strFuncao = string.Empty; // Enviar - Cancelar

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
                // Verifica se a Funcao enviada está dentro do esperado
                if(strFuncao.ToString().Trim() != "Enviar" && strFuncao.ToString().Trim() != "Cancelar")
                {

                    // Define a mensagem de erro
                    strMensagem = $"Função não permitida ou não parametrizada, verifique e tente novamente. Função Solicitada [{strFuncao}]";

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, "", null, strMensagem,
                                     "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    return false;
                }

                // Cria o Cliente Http
                HttpClient cliente = new HttpClient();

                // Gera Log
                objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                 0, 0, "", null, "Inicio do Método " + Mod_Gerais.MethodName(),
                                 "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Pega a URL do Serviço
                Class_Servico objServico = new Class_Servico();
                if (objServico.CarregaDados(_intNumServico, "", strIdentificador, intNumTransacao) == false)
                {
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                       1, -1, "", null, "Erro ao recuperar dados do serviço",
                                                       "", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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

                arrParam.Add(new Parametro("@iCdgPed", intCodRecComWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));
                arrParam.Add(new Parametro("@iCodNF", intCodNFWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));

                ArrayList arrOut = new ArrayList();
                DataTable DadosRecebimento = conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHemo_WS_Recebimento_Sel", arrParam), ref arrOut).Tables[0];

                // Encerra a Conexão com Banco de Dados
                conn.Dispose();

                if (DadosRecebimento.Rows.Count > 0)
                {
                    // Estrutura a Model
                    RecebimentoModel recebimento = new RecebimentoModel
                    {
                        tokenid = DadosRecebimento.Rows[0]["tokenid"].ToString().Trim(),
                        M0_CODFIL = DadosRecebimento.Rows[0]["M0_CODFIL"].ToString().Trim(),
                        M0_CODIGO = DadosRecebimento.Rows[0]["M0_CODIGO"].ToString().Trim(),
                        F1_CHVNFE = DadosRecebimento.Rows[0]["F1_CHVNFE"].ToString().Trim(),
                        F1_COND = DadosRecebimento.Rows[0]["F1_COND"].ToString().Trim(),
                        F1_DOC = DadosRecebimento.Rows[0]["F1_DOC"].ToString().Trim(),
                        F1_DTDIGIT = DadosRecebimento.Rows[0]["F1_DTDIGIT"].ToString().Trim(),
                        F1_EMISSAO = DadosRecebimento.Rows[0]["F1_EMISSAO"].ToString().Trim(),
                        F1_ESPECIE = DadosRecebimento.Rows[0]["F1_ESPECIE"].ToString().Trim(),
                        F1_FORMUL = DadosRecebimento.Rows[0]["F1_FORMUL"].ToString().Trim(),
                        F1_FORNECE = DadosRecebimento.Rows[0]["F1_FORNECE"].ToString().Trim(),
                        F1_LOJA = DadosRecebimento.Rows[0]["F1_LOJA"].ToString().Trim(),
                        F1_SERIE = DadosRecebimento.Rows[0]["F1_SERIE"].ToString().Trim(),
                        F1_TIPO = DadosRecebimento.Rows[0]["F1_TIPO"].ToString().Trim(),
                        F1_XML = DadosRecebimento.Rows[0]["F1_XML"].ToString().Trim(),
                        UUID_WEBSUPPLY = DadosRecebimento.Rows[0]["UUID_WEBSUPPLY"].ToString().Trim(),
                        ANEXOS = new List<RecebimentoModel.Anexo>(),
                        PRENOTA_ITENS = new List<RecebimentoModel.Item>()
                    };

                    // Realiza a Chamada do Banco
                    conn = new Conexao(Mod_Gerais.ConnectionString());

                    // Cria o Parametro da query do banco
                    arrParam = new ArrayList();

                    arrParam.Add(new Parametro("@iCdgPed", intCodRecComWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));
                    arrParam.Add(new Parametro("@iCodNF", intCodNFWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));

                    arrOut = new ArrayList();
                    DataTable DadosItens = conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHemo_WS_Recebimento_Itens_Sel", arrParam), ref arrOut).Tables[0];

                    // Encerra a Conexão com Banco de Dados
                    conn.Dispose();

                    // Verifica se Existe itens para o pedido e caso sim, traz
                    // os itens e caso não, retorna erro
                    if (DadosItens.Rows.Count > 0)
                    {
                        for (int i = 0; i < DadosItens.Rows.Count; i++)
                        {
                            // Pega a Linha do Registro
                            var registro = DadosItens.Rows[i];

                            // Carrega os Dados do item
                            RecebimentoModel.Item item = new RecebimentoModel.Item
                            {
                                D1_CC = registro["D1_CC"].ToString().Trim(),
                                D1_CONTA = registro["D1_CONTA"].ToString().Trim(),
                                D1_DATPRF = registro["D1_DATPRF"].ToString().Trim(),
                                D1_DESPESA = Decimal.Parse(registro["D1_DESPESA"].ToString().Trim()),
                                D1_ITEM = registro["D1_ITEM"].ToString().Trim(),
                                D1_ITEMPC = registro["D1_ITEMPC"].ToString().Trim(),
                                D1_OBS = registro["D1_OBS"].ToString().Trim(),
                                D1_PRECO = Decimal.Parse(registro["D1_PRECO"].ToString().Trim()),
                                D1_COD = registro["D1_COD"].ToString().Trim(),
                                D1_PEDIDO = registro["D1_PEDIDO"].ToString().Trim(),
                                D1_OPER = registro["D1_OPER"].ToString().Trim(),
                                D1_VALDESC = Decimal.Parse(registro["D1_VALDESC"].ToString().Trim()),
                                D1_VUNIT = Decimal.Parse(registro["D1_VUNIT"].ToString().Trim()),
                                D1_QUANT = Decimal.Parse(registro["D1_QUANT"].ToString().Trim()),
                                D1_SEGURO = Decimal.Parse(registro["D1_SEGURO"].ToString().Trim()),
                                D1_TOTAL = Decimal.Parse(registro["D1_TOTAL"].ToString().Trim()),
                                D1_VALFRE = Decimal.Parse(registro["D1_VALFRE"].ToString().Trim()),
                                D1_VALICM = Decimal.Parse(registro["D1_VALICM"].ToString().Trim()),
                                D1_VALIPI = Decimal.Parse(registro["D1_VALIPI"].ToString().Trim()),
                                D1_VLDESC = Decimal.Parse(registro["D1_VLDESC"].ToString().Trim()),
                                D1_MSBLQL = registro["D1_MSBLQL"].ToString().Trim(),
                                ID_ITEMP = int.Parse(registro["ID_ITEMP"].ToString().Trim()),
                                Tipo = registro["Tipo"].ToString().Trim(),
                                LOTES = new List<Lote>()
                            };

                            // Realiza a Chamada do Banco
                            conn = new Conexao(Mod_Gerais.ConnectionString());

                            // Consulta os Lotes do Item
                            arrParam = new ArrayList();

                            arrParam.Add(new Parametro("@CDGPED", intCodRecComWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));
                            arrParam.Add(new Parametro("@iCodNF", intCodNFWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));
                            arrParam.Add(new Parametro("@ID_ITEMP", item.ID_ITEMP, SqlDbType.Int, 4, ParameterDirection.Input));
                            arrParam.Add(new Parametro("@cTipo", item.Tipo, SqlDbType.Char, 2, ParameterDirection.Input));

                            arrOut = new ArrayList();
                            DataTable DadosLote = conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHEMO_WS_RECEBIMENTO_CONTROLE_SEL", arrParam), ref arrOut).Tables[0];

                            // Verifica se Existe Lotes do item para o pedido e caso sim, traz
                            // os lotes do item
                            if (DadosLote.Rows.Count > 0)
                            {
                                for (int j = 0; j < DadosLote.Rows.Count; j++)
                                {
                                    // Pega a Linha do Registro
                                    var registro2 = DadosLote.Rows[j];

                                    // Instância o Lote
                                    RecebimentoModel.Lote lote = new RecebimentoModel.Lote
                                    {
                                        LOTE_SERIE = registro2["LOTE_SERIE"].ToString().Trim(),
                                        QUANT = Decimal.Parse(registro2["QUANT"].ToString().Trim()),
                                        DATA_VALIDADE = registro2["DATA_VALIDADE"].ToString().Trim(),
                                    };

                                    // Adiciona o Lote a listagem de lotes do item
                                    item.LOTES.Add(lote);
                                }
                            }

                            // Adiciona a Array de Itens
                            recebimento.PRENOTA_ITENS.Add(item);
                        }
                    }
                    else
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois não foi retornando nenhum item associado ao Pedido Nº {intCodRecComWebsupply}";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Realiza a Chamada do Banco
                    conn = new Conexao(Mod_Gerais.ConnectionString());

                    // Cria o Parametro da query do banco
                    arrParam = new ArrayList();

                    arrParam.Add(new Parametro("@iCdgPed", intCodRecComWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));
                    arrParam.Add(new Parametro("@iCodNF", intCodNFWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));

                    arrOut = new ArrayList();
                    DataTable DadosAnexos = conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHemo_WS_Recebimento_Anexos_Sel", arrParam), ref arrOut).Tables[0];

                    // Encerra a Conexão com Banco de Dados
                    conn.Dispose();

                    // Verifica se Existe itens para o pedido e caso sim, traz
                    // os itens e caso não, retorna erro
                    if (DadosAnexos.Rows.Count > 0)
                    {
                        for (int i = 0; i < DadosAnexos.Rows.Count; i++)
                        {
                            // Pega a Linha do Registro
                            var registro = DadosAnexos.Rows[i];

                            // Carrega os Dados do Anexo
                            RecebimentoModel.Anexo anexo = new RecebimentoModel.Anexo
                            {
                                ID_DOC = registro["ID_DOC"].ToString().Trim(),
                                DOC = registro["DOC"].ToString().Trim()
                            };

                            // Consulta o Caminho Base dos Arquivos
                            string pathBase = Mod_Gerais.ConsultaParametroConfig("DriveFisicoArquivos");

                            // Define o caminho completo do arquivo
                            string pathCompleto = pathBase + $@"\Arquivos\recebimentos\{intCodRecComWebsupply}\{anexo.DOC}";

                            // Adiciona o Base64 ao objeto Anexo
                            anexo.DOCX64 = Mod_Gerais.ArquivoParaBase64(pathCompleto);

                            // Adiciona a Array de Itens
                            recebimento.ANEXOS.Add(anexo);
                        }
                    }

                    // Remove o Tipo da estrutura de Itens do Pré Nota
                    if(recebimento.PRENOTA_ITENS.Count > 0)
                    {
                        for(var i = 0; i < recebimento.PRENOTA_ITENS.Count; i++)
                        {
                            recebimento.PRENOTA_ITENS[i].Tipo = string.Empty;
                        }
                    }

                    // Serializa o objeto para JSON
                    string jsonRequestBody = JsonConvert.SerializeObject(recebimento);

                    // Atualiza o Identificador
                    strIdentificador = (strFuncao.ToString().Trim() == "Enviar" ? "Cad" : "Exc") + strIdentificador;

                    // Remove todos os Base64 para gerar o Log
                    for (int i = 0; i < recebimento.ANEXOS.Count; i++)
                    {
                        // Pega o Registro
                        RecebimentoModel.Anexo anexo = recebimento.ANEXOS[i];

                        // Retira o Base64
                        anexo.DOCX64 = string.Empty;
                    }

                    // Gera Log
                    objLog = new Class_Log_Hhemo(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, JsonConvert.SerializeObject(recebimento), null, "Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Adiciona o JSON como conteúdo da requisição
                    var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

                    // Carrega os Dados de Autenticação
                    var base64EncodedAuth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{objServico.strUsuario}:{objServico.strSenha}"));

                    // Define os parâmetros e cria a chamada
                    var request = new HttpRequestMessage
                    {
                        Method = strFuncao.ToString().Trim() == "Enviar" ? HttpMethod.Post : HttpMethod.Put,
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
                                     "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                                     "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                                    objLog.GravaLog();
                                    objLog = null;

                                    return false;
                                }
                            }
                        }

                        // Define a mensagem de sucesso
                        strMensagem = $"Recebimento Nº {intCodRecComWebsupply} {(strFuncao.ToString().Trim() == "Enviar" ? "cadastrado(a)" : "atualizado(a)")} com sucesso.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                         "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }
                }
                else
                {
                    // Define a mensagem de erro
                    strMensagem = $"Não foi possível realizar a operação, pois não foi retornando nenhum dado referente ao Pedido Nº {intCodRecComWebsupply}";

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, "", null, strMensagem,
                                     "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                 "L", intCodRecComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Retorna Falso
                return false;
            }
        }
    }
}
