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

            // 4. Monta o objeto de log com os dados coletados
            var log = new LogTelemetria
            {
                Rota = contexto.Request.Path,
                MetodoHttp = contexto.Request.Method,
                StatusCode = contexto.Response.StatusCode,
                TempoExecucaoMs = cronometro.ElapsedMilliseconds,
                DataRequisicao = DateTime.UtcNow,
                ChaveApi = string.IsNullOrEmpty(chaveApi) ? "Anonimo" : chaveApi
            };

            // 5. Salva no SQL Server (Fire-and-Forget controlado ou async tradicional)
            bancoDados.LogsTelemetria.Add(log);
            await bancoDados.SaveChangesAsync();
        }
    }
}