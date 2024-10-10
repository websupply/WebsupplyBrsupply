![Logo do Projeto](logo.png)

# Interface de Integração Websupply x BR Supply

Interface de Integração BRSupply

## Nuget de Referências do Projeto - WebsupplyBrsupply.Interface

- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/13.0.3) - Json.NET is a popular high-performance JSON framework for .NET
- [Microsoft.System.Configuration.ConfigurationManager](https://www.nuget.org/packages/System.Configuration.ConfigurationManager/8.0.0) - Provides types that support using XML configuration files (app.config). This package exists only to support migrating existing .NET Framework code that already uses System.Configuration. When writing new code, use another configuration system instead, such as Microsoft.Extensions.Configuration.
- [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/2.1.4) - Provides the data provider for SQL Server. These classes provide access to versions of SQL Server and encapsulate database-specific protocols, including tabular data stream (TDS)

## Componentes COM

- **WSComuns.dll**: Componente responsável por funções comuns do Sistema.
- **SgiConnection.dll**: Componente responsável pela conexão com o banco de dados.

## Gateway Utilizado

- [API Rest XML - BRSupply](https://wbsvc.brsupply.com.br/webserviceimp/manual/) - API REST BR Supply - API onde será feito todo o consumo e integração

## Como Usar

Será necessário atualizar as dll's dentro do servidor em questão ou utilizar o projeto de Testes (Próximo Tópico).

### Instalação

1. Clone o repositório:
    ```bash
    git clone https://github.com/websupply/WebsupplyBrsupply.git
    ```
2. Navegue até o diretório do projeto:
    ```bash
    cd [repositório da solution]
    ```
3. Restaure os pacotes NuGet:
    ```bash
    dotnet restore
    ```

### Exemplo de Uso

```asp
<!-- Cadastro de Pedido -->
<%
    Dim wsBrSupply
    set wsBrSupply = Server.CreateObject("WebsupplyBrsupply.Interface.Metodos.PedidoCatalogoMetodo.CadastraPedido")
    wsBrSupply.intCodPedCatWebsupply = 0
    if wsBrSupply.Executar() = True then
        response.write wsBrSupply.strCodPedCatBrSupply
        response.write wsBrSupply.strRetornoWS 'Retorna o XML convertido em JSON da API
        response.write "Deu bom"
    else
        response.write wsBrSupply.strMensagem
    end if
%>

<!-- Cancelamento de Pedido -->
<%
    Dim wsBrSupply
    set wsBrSupply = Server.CreateObject("WebsupplyBrsupply.Interface.Metodos.PedidoCatalogoMetodo.CancelaPedido")
    'Aqui pode usar somente um dos codigos sendo o strCodPedCatWebsupply(Websupply) ou o strCodPedCatBrSupply(BR Supply)
    wsBrSupply.strCodPedCatWebsupply = 0
    wsBrSupply.strCodPedCatBrSupply = 0
    wsBrSupply.strMotivoCancelamento = "Cancelar porque foi errado"
    if wsBrSupply.Executar() = True then
        response.write "Deu bom"
        response.write wsBrSupply.strRetornoWS 'Retorna o XML convertido em JSON da API
    else
        response.write wsBrSupply.strMensagem
    end if
%>

<!-- Consulta de Pedido -->
<%
    Dim wsBrSupply
    set wsBrSupply = Server.CreateObject("WebsupplyBrsupply.Interface.Metodos.PedidoCatalogoMetodo.ConsultaPedido")
    'Aqui pode usar somente um dos codigos sendo o strCodPedCatWebsupply(Websupply) ou o strCodPedCatBrSupply(BR Supply)
    wsBrSupply.strCodPedCatWebsupply = 0
    wsBrSupply.strCodPedCatBrSupply = 0
    if wsBrSupply.Executar() = True then
        response.write wsBrSupply.strRetornoWS 'Retorna o XML convertido em JSON da API
        response.write "Deu bom"
    else
        response.write wsBrSupply.strMensagem
    end if
%>

