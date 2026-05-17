using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

public class FiltroLimiteRequisicoes
{
    private readonly RequestDelegate _proximo;
    private readonly IConnectionMultiplexer _redis;

    public FiltroLimiteRequisicoes(RequestDelegate proximo, IConnectionMultiplexer redis)
    {
        _proximo = proximo;
        _redis = redis;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        // 1. Captura a chave de API enviada nos Headers da requisição
        var chaveApi = contexto.Request.Headers["X-Chave-API"].ToString();
        
        if (string.IsNullOrEmpty(chaveApi))
        {
            contexto.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await contexto.Response.WriteAsJsonAsync(new { mensagem = "Acesso negado. Chave de API ausente nos cabeçalhos." });
            return;
        }

        // 2. Define a conexão com o banco de dados do Redis
        var bancoRedis = _redis.GetDatabase();
        var chaveCliente = $"limite_requisicao:{chaveApi}";

        // 3. Incrementa o valor no Redis de forma atômica
        // Se a chave não existir, o Redis cria com valor 1 automaticamente
        var contagemRequisicoes = await bancoRedis.StringIncrementAsync(chaveCliente);

        // 4. Se for a primeira requisição do ciclo, define o tempo de expiração (janela de 1 minuto)
        if (contagemRequisicoes == 1)
        {
            await bancoRedis.KeyExpireAsync(chaveCliente, TimeSpan.FromMinutes(1));
        }

        // 5. Define o limite máximo permitido (Exemplo: 60 requisições por minuto)
        const int limiteMaximo = 60;

        // Adiciona cabeçalhos informativos na resposta (padrão de mercado para Gateways)
        contexto.Response.Headers.Append("X-Limite-Maximo", limiteMaximo.ToString());
        contexto.Response.Headers.Append("X-Limite-Restante", Math.Max(0, limiteMaximo - contagemRequisicoes).ToString());

        // 6. Verifica se o cliente estourou o limite permitido
        if (contagemRequisicoes > limiteMaximo)
        {
            contexto.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await contexto.Response.WriteAsJsonAsync(new { erro = "Limite de requisições excedido. Tente novamente em breve." });
            return;
        }

        // 7. Se estiver tudo certo, segue o fluxo para o proxy reverso (YARP) fazer o roteamento
        await _proximo(contexto);
    }
}