using System;

namespace Gateway.Shared.Modelos;

public class LogTelemetria
{
    public long Id { get; set; }
    public string Rota { get; set; } = string.Empty;
    public string MetodoHttp { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public long TempoExecucaoMs { get; set; }
    public DateTime DataRequisicao { get; set; }
    public string? ChaveApi { get; set; }
}