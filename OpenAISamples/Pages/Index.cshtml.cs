using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAISamples.Services;

namespace OpenAISamples.Pages;

[BindProperties]
public class IndexModel : PageModel
{
    public string Cargo { get; set; } = "Desenvolvedor AspNet Core Sênior";
    public string Conhecimentos { get; set; } = "AspNet Core, EF7, Azure, VueJS, Sql Server";
    public string FaixaSalarial { get; set; } = "R$8000";
    public string FormaDeContratacao { get; set; } = "CLT";
    public string Resposta { get; set; } = string.Empty;

    private readonly OpenAI _openAi;
    public IndexModel(IConfiguration configuration)
    {
        var apiKey = configuration?.GetValue<string>("ApiKey") ?? throw new ArgumentNullException("ApiKey");
        _openAi = new OpenAI(apiKey);
    }

    public async Task OnPost()
    {
        var prompt = ObterPrompt();
        Resposta = await _openAi.Completions(prompt);
    }

    private string ObterPrompt()
        =>
            $"""
             Crie uma vaga de emprego na empresa TechTI com as seguintes informações:
             - Título do cargo sendo {Cargo}.
             - Resumo da empresa.
             - Descrição das Responsabilidades.
             - As Habilidades Desejadas são {Conhecimentos}.
             - Informações sobre Qualificações
             - Oportunidades de Desenvolvimento Profissional
             - Benefícios em forma de lista 
             - Remuneração de {FaixaSalarial}.
             - Forma de contratação {FormaDeContratacao}.
             - Resultado no formato de html.
             """;
}