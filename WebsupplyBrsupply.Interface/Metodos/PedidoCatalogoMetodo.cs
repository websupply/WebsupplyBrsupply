using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SgiConnection;
using System;
using System.Collections;
using System.Data;
using System.Text;
using WebsupplyBrsupply.Interface.Funcoes;
using WebsupplyBrsupply.Interface.Model;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Headers;
using static WebsupplyBrsupply.Interface.Model.PedidoCatalogoModel;
using System.Xml.Serialization;

namespace WebsupplyBrsupply.Interface.Metodos
{
    public class PedidoCatalogoMetodo
    {
        static int _intNumTransacao = 0;
        static int _intNumServico = 1;
        string strIdentificador = "PedCat" + Mod_Gerais.RetornaIdentificador();

        public string strMensagem = string.Empty;

        // Paramêtros de Controle da Classe
        public int intCodPedComWebsupply = 0;
        public string strCodPedComProtheus = string.Empty;

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

        public bool IncluirPedido()
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
                                 "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Pega a URL do Serviço
                Class_Servico objServico = new Class_Servico();
                if (objServico.CarregaDados(_intNumServico, "", strIdentificador, intNumTransacao) == false)
                {
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                       1, -1, "", null, "Erro ao recuperar dados do serviço",
                                                       "", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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

                arrParam.Add(new Parametro("@iCdgPed", intCodPedComWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));

                ArrayList arrOut = new ArrayList();
                DataTable DadosPedidoCatalogo = conn.ExecuteStoredProcedure(new StoredProcedure("[Procedure para Pegar o Cabeçalho]", arrParam), ref arrOut).Tables[0];

                // Encerra a Conexão com Banco de Dados
                conn.Dispose();

                if (DadosPedidoCatalogo.Rows.Count > 0)
                {
                    // Estrutura a Model
                    PedidoCatalogoModel.PedidoCatalogo pedidoCatalogo = new PedidoCatalogoModel.PedidoCatalogo
                    {
                        info = new PedidoCatalogoModel.Info
                        {
                            NomeRemetente = DadosPedidoCatalogo.Rows[0]["nome-rementente"].ToString().Trim(),
                            NomeDestinatario = DadosPedidoCatalogo.Rows[0]["nome-destinatario"].ToString().Trim(),
                            Key = DadosPedidoCatalogo.Rows[0]["key"].ToString().Trim(),
                            Auth = DadosPedidoCatalogo.Rows[0]["auth"].ToString().Trim()
                        },
                        detPedidos = new PedidoCatalogoModel.DetPedidos
                        {
                            pedido = new PedidoCatalogoModel.Pedido
                            {
                                Referencia = DadosPedidoCatalogo.Rows[0]["referencia"].ToString().Trim(),
                                Cnpj = DadosPedidoCatalogo.Rows[0]["cnpj"].ToString().Trim(),
                                CodLocal = DadosPedidoCatalogo.Rows[0]["cod-local"].ToString().Trim(),
                                Usuario = DadosPedidoCatalogo.Rows[0]["usuario"].ToString().Trim(),
                                Observacao = DadosPedidoCatalogo.Rows[0]["observacao"].ToString().Trim(),
                                CodCategoria = DadosPedidoCatalogo.Rows[0]["cod-categoria"].ToString().Trim(),
                                detItens = new PedidoCatalogoModel.DetItens
                                {
                                    Itens = new List<PedidoCatalogoModel.Item>()
                                }
                            }
                        }
                    };

                    // Realiza a Chamada do Banco
                    conn = new Conexao(Mod_Gerais.ConnectionString());

                    // Cria o Parametro da query do banco
                    arrParam = new ArrayList();

                    arrParam.Add(new Parametro("@iCdgPed", intCodPedComWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));

                    arrOut = new ArrayList();
                    DataTable DadosItens = conn.ExecuteStoredProcedure(new StoredProcedure("[Procedure para pegar os itens do Pedido]", arrParam), ref arrOut).Tables[0];

                    // Encerra a Conexão com Banco de Dados
                    conn.Dispose();

                    // Verifica se Existe itens para o pedido e caso sim, traz
                    // os itens e caso não, retorna erro
                    if (DadosItens.Rows.Count > 0)
                    {
                        for(int i = 0; i < DadosItens.Rows.Count; i++)
                        {
                            // Pega a Linha do Registro
                            var registro = DadosItens.Rows[i];

                            // Carrega os Dados do item
                            PedidoCatalogoModel.Item item = new PedidoCatalogoModel.Item
                            {
                                CodBrsupply = registro["cod-brsupply"].ToString().Trim(),
                                CodCliente = registro["cod-cliente"].ToString().Trim(),
                                Quantidade = registro["quantidade"].ToString().Trim(),
                                VlrUnitario = registro["vlr-unitario"].ToString().Trim(),
                                OrdemItem = registro["ordem-item"].ToString().Trim(),
                                SequenciaItem = registro["sequencia-item"].ToString().Trim(),
                            };

                            // Adiciona a Array de Itens
                            pedidoCatalogo.detPedidos.pedido.detItens.Itens.Add(item);
                        }
                    }
                    else
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois não foi retornando nenhum item associado ao Pedido Nº {intCodPedComWebsupply}";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Serializa o objeto para XML
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(PedidoCatalogoModel.PedidoCatalogo));
                    string xmlRequestBody;
                    using (var stringWriter = new StringWriter())
                    {
                        xmlSerializer.Serialize(stringWriter, pedidoCatalogo);
                        xmlRequestBody = stringWriter.ToString();
                    }

                    // Atualiza o Identificador
                    strIdentificador = "Cad" + strIdentificador;

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, xmlRequestBody, null, "Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Adiciona o JSON como conteúdo da requisição
                    var content = new StringContent(xmlRequestBody, Encoding.UTF8, "application/xml");

                    // Define os parâmetros e cria a chamada
                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Post,
                        RequestUri = new Uri(objServico.strURL),
                        Content = content
                    };

                    // Envia a requisição
                    var response = cliente.SendAsync(request).ConfigureAwait(false).GetAwaiter().GetResult();

                    // Trata o Retorno da API
                    var responseBody = response.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

                    // Gera Log com o retorno da API
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, (int)response.StatusCode, responseBody, null, "Retorno da Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                                     "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                                    objLog.GravaLog();
                                    objLog = null;

                                    return false;
                                }

                                // Sincroniza o Retorno da API com os Parametros
                                strCodPedComProtheus = linhaRetorno["C7_NUM"].ToString().Trim();

                                // Valida se algum dos códigos retornou vázio
                                // caso sim, devolve erro
                                if (strCodPedComProtheus == String.Empty)
                                {
                                    strMensagem = $"Ocorreu um erro na chamada da aplicação - [{linhaRetorno["C_STATUS"].ToString().Trim()}] - C7_NUM [{strCodPedComProtheus}]";

                                    return false;
                                }

                                // Caso o Pedido não tenha código do protheus, armazena essa informação no banco
                                if (pedidoCompra.C7_NUM == String.Empty)
                                {
                                    // Realiza a Chamada do Banco
                                    conn = new Conexao(Mod_Gerais.ConnectionString());

                                    // Cria o Parametro da query do banco
                                    ArrayList arrParam2 = new ArrayList();

                                    arrParam2.Add(new Parametro("@iCdgPed", intCodPedComWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));
                                    arrParam2.Add(new Parametro("@vNumPedInt", strCodPedComProtheus, SqlDbType.Char, 15, ParameterDirection.Input));

                                    ArrayList arrOut2 = new ArrayList();

                                    conn.ExecuteStoredProcedure(new StoredProcedure("SP_HHemo_WS_Pedido_Compras_NumPedInt_UPD", arrParam2), ref arrOut2);

                                    // Encerra a Conexão com Banco de Dados
                                    conn.Dispose();
                                }
                            }
                        }

                        // Define a mensagem de sucesso
                        strMensagem = $"Pedido Nº {intCodPedComWebsupply} do codigo [{(strCodPedComProtheus != String.Empty ? strCodPedComProtheus : pedidoCompra.C7_NUM)}] {(pedidoCompra.C7_NUM != String.Empty ? "atualizado(a)" : "cadastrado(a)")} com sucesso.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                         "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }
                }
                else
                {
                    // Define a mensagem de erro
                    strMensagem = $"Não foi possível realizar a operação, pois não foi retornando nenhum dado referente ao Pedido Nº {intCodPedComWebsupply}";

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, "", null, strMensagem,
                                     "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                 "L", intCodPedComWebsupply.ToString(), "", Mod_Gerais.MethodName());
                objLog.GravaLog();
                objLog = null;

                // Retorna Falso
                return false;
            }
        }
    }
}
