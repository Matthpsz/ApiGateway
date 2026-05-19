using System.Diagnostics;
using System.Threading.Tasks;
using Gateway.Shared.Contexto;
using Gateway.Shared.Modelos;
using Microsoft.AspNetCore.Http;

public class FiltroTelemetria
{
    private readonly RequestDelegate _proximo;

    public FiltroTelemetria(RequestDelegate proximo)
    {
        _proximo = proximo;
    }

    public async Task InvokeAsync(HttpContext contexto, GatewayDbContext bancoDados)
    {
        // 1. Inicia o cronômetro de alta precisão
        var cronometro = Stopwatch.StartNew();

        // Captura a chave de API (caso exista) para atrelar ao log
        var chaveApi = contexto.Request.Headers["X-Chave-API"].ToString();

        try
        {
            // 2. Deixa a requisição seguir para o próximo passo (Rate Limiting e depois YARP)
            await _proximo(contexto);
        }
        finally
        {
            // 3. Para o cronômetro independente se a requisição deu certo ou falhou
            cronometro.Stop();

            // Identifica o projeto de forma inteligente e dinâmica com base na rota
            var caminho = contexto.Request.Path.Value ?? "/";
            var nomeProjeto = "VotoTrack"; // Como o VotoTrack é o fallback raiz, todo tráfego geral pertence a ele

            if (caminho.StartsWith("/api/produtos", StringComparison.OrdinalIgnoreCase))
            {
                nomeProjeto = "TesteAPI";
            }
            else if (caminho.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                var caminhos = caminho.Split('/');
                if (caminhos.Length > 2)
                {
                    var identificador = caminhos[2];
                    if (!string.IsNullOrEmpty(identificador))
                    {
                        nomeProjeto = identificador.ToLower() switch
                        {
                            "vototrack" => "VotoTrack",
                            "produtos" => "TesteAPI",
                            "leilao" => "LeiloesApp",
                            _ => char.ToUpper(identificador[0]) + identificador.Substring(1)
                        };
                    }
                }
            }

            // 4. Monta o objeto de log com os dados coletados
            var log = new LogTelemetria
            {
                Rota = contexto.Request.Path,
                MetodoHttp = contexto.Request.Method,
                StatusCode = contexto.Response.StatusCode,
                TempoExecucaoMs = cronometro.ElapsedMilliseconds,
                DataRequisicao = DateTime.UtcNow,
                ChaveApi = string.IsNullOrEmpty(chaveApi) ? "Anonimo" : chaveApi,
                Projeto = nomeProjeto
            };

            // 5. Salva no Supabase Postgres
            bancoDados.LogsTelemetria.Add(log);
            await bancoDados.SaveChangesAsync();
        }
    }
}