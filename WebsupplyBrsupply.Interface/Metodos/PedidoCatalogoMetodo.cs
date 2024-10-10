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
using System.Xml.Serialization;
using System.Xml;

namespace WebsupplyBrsupply.Interface.Metodos
{
    public class PedidoCatalogoMetodo
    {
        public class CadastraPedido
        {
            static int _intNumTransacao = 0;
            static int _intNumServico = 1;
            string strIdentificador = "CadPedCat" + Mod_Gerais.RetornaIdentificador();

            public string strMensagem = string.Empty;

            // Paramêtros de Controle da Classe
            public int intCodPedCatWebsupply = 0;
            public string strCodPedCatBrSupply = string.Empty;

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

            public bool Executar()
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
                                     "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Pega a URL do Serviço
                    Class_Servico objServico = new Class_Servico();
                    if (objServico.CarregaDados(_intNumServico, "", strIdentificador, intNumTransacao) == false)
                    {
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                           1, -1, "", null, "Erro ao recuperar dados do serviço",
                                                           "", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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

                    arrParam.Add(new Parametro("@iCDGPED", intCodPedCatWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));

                    ArrayList arrOut = new ArrayList();
                    DataTable DadosPedidoCatalogo = conn.ExecuteStoredProcedure(new StoredProcedure("SP_WS_BRSUPPLY_PEDIDO_CATALOGO_SEL", arrParam), ref arrOut).Tables[0];

                    // Encerra a Conexão com Banco de Dados
                    conn.Dispose();

                    if (DadosPedidoCatalogo.Rows.Count > 0)
                    {
                        // Estrutura a Model
                        PedidoCatalogoModel.CadastroPedido.Arquivo pedidoCatalogo = new PedidoCatalogoModel.CadastroPedido.Arquivo
                        {
                            info = new PedidoCatalogoModel.CadastroPedido.Info
                            {
                                NomeRemetente = "GrupoPulsa",
                                NomeDestinatario = "BR SUPPLY",
                                Key = objServico.strSenha,
                                Auth = objServico.strUsuario
                            },
                            detPedidos = new PedidoCatalogoModel.CadastroPedido.DetPedidos
                            {
                                pedido = new PedidoCatalogoModel.CadastroPedido.Pedido
                                {
                                    Referencia = DadosPedidoCatalogo.Rows[0]["referencia"].ToString().Trim(),
                                    Cnpj = DadosPedidoCatalogo.Rows[0]["cnpj"].ToString().Trim(),
                                    CodLocal = string.IsNullOrWhiteSpace(DadosPedidoCatalogo.Rows[0]["cod-local"].ToString().Trim()) ? null : DadosPedidoCatalogo.Rows[0]["cod-local"].ToString().Trim(),
                                    Usuario = string.IsNullOrWhiteSpace(DadosPedidoCatalogo.Rows[0]["usuario"].ToString().Trim()) ? null : DadosPedidoCatalogo.Rows[0]["usuario"].ToString().Trim(),
                                    Observacao = DadosPedidoCatalogo.Rows[0]["observacao"].ToString().Trim(),
                                    CodCategoria = string.IsNullOrWhiteSpace(DadosPedidoCatalogo.Rows[0]["cod-categoria"].ToString().Trim()) ? null : DadosPedidoCatalogo.Rows[0]["cod-categoria"].ToString().Trim(),
                                    detItens = new PedidoCatalogoModel.CadastroPedido.DetItens
                                    {
                                        Itens = new List<PedidoCatalogoModel.CadastroPedido.Item>()
                                    }
                                }
                            }
                        };

                        // Realiza a Chamada do Banco
                        conn = new Conexao(Mod_Gerais.ConnectionString());

                        // Cria o Parametro da query do banco
                        arrParam = new ArrayList();

                        arrParam.Add(new Parametro("@iCDGPED", intCodPedCatWebsupply, SqlDbType.Int, 4, ParameterDirection.Input));

                        arrOut = new ArrayList();
                        DataTable DadosItens = conn.ExecuteStoredProcedure(new StoredProcedure("SP_WS_BRSUPPLY_PEDIDO_CATALOGO_ITENS_SEL", arrParam), ref arrOut).Tables[0];

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
                                PedidoCatalogoModel.CadastroPedido.Item item = new PedidoCatalogoModel.CadastroPedido.Item
                                {
                                    CodBrsupply = registro["cod-brsupply"].ToString().Trim(),
                                    CodCliente = registro["cod-cliente"].ToString().Trim(),
                                    Quantidade = registro["quantidade"].ToString().Trim(),
                                    VlrUnitario = registro["vlr-unitario"].ToString().Trim(),
                                    OrdemItem = string.IsNullOrWhiteSpace(registro["ordem-item"].ToString().Trim()) ? null : registro["ordem-item"].ToString().Trim(),
                                    SequenciaItem = registro["sequencia-item"].ToString().Trim(),
                                };

                                // Adiciona a Array de Itens
                                pedidoCatalogo.detPedidos.pedido.detItens.Itens.Add(item);
                            }
                        }
                        else
                        {
                            // Define a mensagem de erro
                            strMensagem = $"Não foi possível realizar a operação, pois não foi retornando nenhum item associado ao Pedido Nº {intCodPedCatWebsupply}";

                            // Gera Log
                            objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                             0, 0, "", null, strMensagem,
                                             "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                            objLog.GravaLog();
                            objLog = null;

                            return false;
                        }

                        // Serializa o objeto para XML
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(PedidoCatalogoModel.CadastroPedido.Arquivo));
                        string xmlRequestBody;

                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", ""); // Remove os namespaces

                        using (var memoryStream = new MemoryStream())
                        {
                            using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                            {
                                XmlWriterSettings settings = new XmlWriterSettings
                                {
                                    Indent = true,
                                    Encoding = Encoding.UTF8,
                                    OmitXmlDeclaration = true // Remove a declaração XML
                                };

                                using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, settings))
                                {
                                    xmlSerializer.Serialize(xmlWriter, pedidoCatalogo, ns);
                                }
                            }

                            xmlRequestBody = Encoding.UTF8.GetString(memoryStream.ToArray());
                        }

                        // Adiciona manualmente a declaração XML
                        string xmlDeclaration = "<?xml version=\"1.0\"?>\r\n";
                        xmlRequestBody = xmlDeclaration + xmlRequestBody;

                        // Atualiza o Identificador
                        strIdentificador = "Cad" + strIdentificador;

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, xmlRequestBody, null, "Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                         "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                         "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        if (response.IsSuccessStatusCode)
                        {
                            response.EnsureSuccessStatusCode();

                            // Desserializa a resposta XML
                            XmlSerializer serializer = new XmlSerializer(typeof(RetornoAPIModel.CadastraPedidoCatalogo.Arquivo));
                            RetornoAPIModel.CadastraPedidoCatalogo.Arquivo retornoAPI;

                            using (StringReader reader = new StringReader(responseBody))
                            {
                                retornoAPI = (RetornoAPIModel.CadastraPedidoCatalogo.Arquivo)serializer.Deserialize(reader);
                            }

                            // Verifica se retornou erro
                            if (retornoAPI.processamento.pedido.Status.ToUpper() == "ERRO")
                            {
                                // Define a mensagem de erro
                                strMensagem = retornoAPI.processamento.pedido.Mensagem;

                                // Gera Log
                                objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                 0, (int)response.StatusCode, "", null, strMensagem,
                                                 "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                                objLog.GravaLog();
                                objLog = null;

                                return false;
                            }

                            // Seta os Parametros recebidos pela Interface
                            strMensagem = retornoAPI.processamento.pedido.Mensagem;
                            strCodPedCatBrSupply = retornoAPI.processamento.pedido.Transacao;

                            // Define a mensagem de sucesso
                            strMensagem = $"Pedido Nº {intCodPedCatWebsupply} do codigo [{strCodPedCatBrSupply}] cadastrado(a) com sucesso.";

                            // Gera Log
                            objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                             0, 0, "", null, strMensagem,
                                             "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                             "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                            objLog.GravaLog();
                            objLog = null;

                            return false;
                        }
                    }
                    else
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois não foi retornando nenhum dado referente ao Pedido Nº {intCodPedCatWebsupply}";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     1, -1, JsonConvert.SerializeObject(excepetionEstruturada), null, strMensagem,
                                     "L", intCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Retorna Falso
                    return false;
                }
            }
        }
        
        public class CancelaPedido
        {
            static int _intNumTransacao = 0;
            static int _intNumServico = 2;
            string strIdentificador = "CancPedCat" + Mod_Gerais.RetornaIdentificador();

            public string strMensagem = string.Empty;

            // Paramêtros de Controle da Classe
            public string strCodPedCatWebsupply = string.Empty;
            public string strCodPedCatBrSupply = string.Empty;
            public string strMotivoCancelamento = string.Empty;

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

            public bool Executar()
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
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Pega a URL do Serviço
                    Class_Servico objServico = new Class_Servico();
                    if (objServico.CarregaDados(_intNumServico, "", strIdentificador, intNumTransacao) == false)
                    {
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                           1, -1, "", null, "Erro ao recuperar dados do serviço",
                                                           "", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;
                        strMensagem = "Erro ao recuperar dados do serviço";
                        return false;
                    }
                    else
                    { _intNumTransacao -= 1; }

                    // Estrutura a Model
                    PedidoCatalogoModel.CancelamentoPedido.Arquivo pedidoCatalogo = new PedidoCatalogoModel.CancelamentoPedido.Arquivo
                    {
                        info = new PedidoCatalogoModel.CancelamentoPedido.Info 
                        { 
                            NomeRemetente = "GrupoPulsa",
                            NomeDestinatario = "BR SUPPLY",
                            Key = objServico.strSenha,
                            Auth = objServico.strUsuario,
                            OrdemCompra = (strCodPedCatWebsupply == string.Empty || strCodPedCatBrSupply != string.Empty) ? null : strCodPedCatWebsupply,
                            Pedido = (strCodPedCatBrSupply == string.Empty || strCodPedCatWebsupply != string.Empty) ? null : strCodPedCatBrSupply,
                            Motivo = strMotivoCancelamento
                        }
                    };

                    // Verifica se ambos os campos de codigos estão vázios
                    if(pedidoCatalogo.info.OrdemCompra == null && pedidoCatalogo.info.Pedido == null)
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois tanto o código BRSupply quanto WebSupply estão vázios, preencha somente um para prosseguir.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Verifica se ambos os campos de codigos estão preenchidos
                    if(pedidoCatalogo.info.OrdemCompra != null && pedidoCatalogo.info.Pedido != null)
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois tanto o código BRSupply quanto WebSupply foram preenchidos, preencha somente um para prosseguir.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Verifica se o motivo foi preenchido
                    if(pedidoCatalogo.info.Motivo == null || pedidoCatalogo.info.Motivo == string.Empty)
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois não foi especificado o Motivo do cancelamento.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Serializa o objeto para XML
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(PedidoCatalogoModel.CancelamentoPedido.Arquivo));
                    string xmlRequestBody;

                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", ""); // Remove os namespaces

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                        {
                            XmlWriterSettings settings = new XmlWriterSettings
                            {
                                Indent = true,
                                Encoding = Encoding.UTF8,
                                OmitXmlDeclaration = true // Remove a declaração XML
                            };

                            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, settings))
                            {
                                xmlSerializer.Serialize(xmlWriter, pedidoCatalogo, ns);
                            }
                        }

                        xmlRequestBody = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }

                    // Adiciona manualmente a declaração XML
                    string xmlDeclaration = "<?xml version=\"1.0\"?>\r\n";
                    xmlRequestBody = xmlDeclaration + xmlRequestBody;

                    // Atualiza o Identificador
                    strIdentificador = "Canc" + strIdentificador;

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, xmlRequestBody, null, "Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();

                        // Desserializa a resposta XML
                        XmlSerializer serializer = new XmlSerializer(typeof(RetornoAPIModel.CancelamentoPedidoCatalogo.Arquivo));
                        RetornoAPIModel.CancelamentoPedidoCatalogo.Arquivo retornoAPI;

                        using (StringReader reader = new StringReader(responseBody))
                        {
                            retornoAPI = (RetornoAPIModel.CancelamentoPedidoCatalogo.Arquivo)serializer.Deserialize(reader);
                        }

                        // Verifica se retornou erro
                        if (retornoAPI.Status.ToUpper() == "ERRO")
                        {
                            // Define a mensagem de erro
                            strMensagem = retornoAPI.Mensagem;

                            // Gera Log
                            objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                             0, (int)response.StatusCode, "", null, strMensagem,
                                             "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                            objLog.GravaLog();
                            objLog = null;

                            return false;
                        }

                        // Seta os Parametros recebidos pela Interface
                        strMensagem = retornoAPI.Mensagem;

                        // Define a mensagem de sucesso
                        strMensagem = $"Pedido Nº {(strCodPedCatWebsupply == null ? "BR Supply" : "WebSupply")} do codigo [{(strCodPedCatWebsupply == null ? strCodPedCatBrSupply : strCodPedCatWebsupply)}] cancelado com sucesso.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     1, -1, JsonConvert.SerializeObject(excepetionEstruturada), null, strMensagem,
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Retorna Falso
                    return false;
                }
            }
        }

        public class ConsultaPedido
        {
            static int _intNumTransacao = 0;
            static int _intNumServico = 3;
            string strIdentificador = "ConsPedCat" + Mod_Gerais.RetornaIdentificador();

            public string strMensagem = string.Empty;

            // Paramêtros de Controle da Classe
            public string strCodPedCatWebsupply = string.Empty;
            public string strCodPedCatBrSupply = string.Empty;
            public RetornoAPIModel.ConsultaPedidoCatalogo.Arquivo objRetornoWS = new RetornoAPIModel.ConsultaPedidoCatalogo.Arquivo();

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

            public bool Executar()
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
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Pega a URL do Serviço
                    Class_Servico objServico = new Class_Servico();
                    if (objServico.CarregaDados(_intNumServico, "", strIdentificador, intNumTransacao) == false)
                    {
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                                           1, -1, "", null, "Erro ao recuperar dados do serviço",
                                                           "", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;
                        strMensagem = "Erro ao recuperar dados do serviço";
                        return false;
                    }
                    else
                    { _intNumTransacao -= 1; }

                    // Estrutura a Model
                    PedidoCatalogoModel.CancelamentoPedido.Arquivo pedidoCatalogo = new PedidoCatalogoModel.CancelamentoPedido.Arquivo
                    {
                        info = new PedidoCatalogoModel.CancelamentoPedido.Info
                        {
                            NomeRemetente = "GrupoPulsa",
                            NomeDestinatario = "BR SUPPLY",
                            Key = objServico.strSenha,
                            Auth = objServico.strUsuario,
                            OrdemCompra = (strCodPedCatWebsupply == string.Empty || strCodPedCatBrSupply != string.Empty) ? null : strCodPedCatWebsupply,
                            Pedido = (strCodPedCatBrSupply == string.Empty || strCodPedCatWebsupply != string.Empty) ? null : strCodPedCatBrSupply
                        }
                    };

                    // Verifica se ambos os campos de codigos estão vázios
                    if (pedidoCatalogo.info.OrdemCompra == null && pedidoCatalogo.info.Pedido == null)
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois tanto o código BRSupply quanto WebSupply estão vázios, preencha somente um para prosseguir.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Verifica se ambos os campos de codigos estão preenchidos
                    if (pedidoCatalogo.info.OrdemCompra != null && pedidoCatalogo.info.Pedido != null)
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois tanto o código BRSupply quanto WebSupply foram preenchidos, preencha somente um para prosseguir.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Verifica se o motivo foi preenchido
                    if (pedidoCatalogo.info.Motivo == null || pedidoCatalogo.info.Motivo == string.Empty)
                    {
                        // Define a mensagem de erro
                        strMensagem = $"Não foi possível realizar a operação, pois não foi especificado o Motivo do cancelamento.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                        objLog.GravaLog();
                        objLog = null;

                        return false;
                    }

                    // Serializa o objeto para XML
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(PedidoCatalogoModel.CancelamentoPedido.Arquivo));
                    string xmlRequestBody;

                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", ""); // Remove os namespaces

                    using (var memoryStream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(false)))
                        {
                            XmlWriterSettings settings = new XmlWriterSettings
                            {
                                Indent = true,
                                Encoding = Encoding.UTF8,
                                OmitXmlDeclaration = true // Remove a declaração XML
                            };

                            using (XmlWriter xmlWriter = XmlWriter.Create(streamWriter, settings))
                            {
                                xmlSerializer.Serialize(xmlWriter, pedidoCatalogo, ns);
                            }
                        }

                        xmlRequestBody = Encoding.UTF8.GetString(memoryStream.ToArray());
                    }

                    // Adiciona manualmente a declaração XML
                    string xmlDeclaration = "<?xml version=\"1.0\"?>\r\n";
                    xmlRequestBody = xmlDeclaration + xmlRequestBody;

                    // Atualiza o Identificador
                    strIdentificador = "Canc" + strIdentificador;

                    // Gera Log
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     0, 0, xmlRequestBody, null, "Chamada a API Rest - Método " + Mod_Gerais.MethodName(),
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    if (response.IsSuccessStatusCode)
                    {
                        response.EnsureSuccessStatusCode();

                        // Desserializa a resposta XML
                        XmlSerializer serializer = new XmlSerializer(typeof(RetornoAPIModel.ConsultaPedidoCatalogo.Arquivo));
                        RetornoAPIModel.ConsultaPedidoCatalogo.Arquivo retornoAPI;

                        using (StringReader reader = new StringReader(responseBody))
                        {
                            retornoAPI = (RetornoAPIModel.ConsultaPedidoCatalogo.Arquivo)serializer.Deserialize(reader);
                        }

                        // Aplica o Retorno da API no objeto publico
                        objRetornoWS = retornoAPI;

                        // Verifica se retornou erro
                        if (retornoAPI.Status.ToUpper() == "ERRO")
                        {
                            // Define a mensagem de erro
                            strMensagem = retornoAPI.Mensagem;

                            // Gera Log
                            objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                             0, (int)response.StatusCode, "", null, strMensagem,
                                             "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                            objLog.GravaLog();
                            objLog = null;

                            return false;
                        }

                        // Seta os Parametros recebidos pela Interface
                        strMensagem = retornoAPI.Mensagem;

                        // Define a mensagem de sucesso
                        strMensagem = $"Pedido Nº {(strCodPedCatWebsupply == null ? "BR Supply" : "WebSupply")} do codigo [{(strCodPedCatWebsupply == null ? strCodPedCatBrSupply : strCodPedCatWebsupply)}] cancelado com sucesso.";

                        // Gera Log
                        objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                         0, 0, "", null, strMensagem,
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                                         "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
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
                    objLog = new Class_Log_Brsupply(strIdentificador, intNumTransacao, _intNumServico,
                                     1, -1, JsonConvert.SerializeObject(excepetionEstruturada), null, strMensagem,
                                     "L", strCodPedCatWebsupply.ToString(), "", Mod_Gerais.MethodName());
                    objLog.GravaLog();
                    objLog = null;

                    // Retorna Falso
                    return false;
                }
            }
        }
    }
}
