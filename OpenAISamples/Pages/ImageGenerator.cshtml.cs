using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenAISamples.Services;

namespace OpenAISamples.Pages;

[BindProperties]
public class ImageGenerator : PageModel
{
    public string Caracteristicas { get; set; } = "Empresa de TI com foco em desenvolvimento de APIs";
    public List<string> NomesSugeridos { get; set; } = new();
    public IEnumerable<string> Logotipos { get; set; } = Enumerable.Empty<string>();
    
    private readonly OpenAI _openAi;
    public ImageGenerator(IConfiguration configuration)
    {
        var apiKey = configuration?.GetValue<string>("ApiKey") ?? throw new ArgumentNullException("ApiKey");
        _openAi = new OpenAI(apiKey);
    }

    public void OnGet()
    {
    }

    public async Task OnPost()
    {
        NomesSugeridos = await ObterNomeDaEmpresa();
        Logotipos = await ObterLogosDaEmpresa(NomesSugeridos);
    }

    private async Task<List<string>> ObterNomeDaEmpresa()
    {
        var prompt = ObterPromptParaCriarNomeDeEmpresa();
        var response = await _openAi.Completions(prompt);
        var separator = ",";
        if (response.Contains("\n")) separator = "\n";
        return response?.Split(separator)?
            .Select(x => x.Substring(3))
            .ToList() ?? new List<string>();
    }
    
    private async Task<IEnumerable<string>> ObterLogosDaEmpresa(List<string> empresas)
    {
        var logos = new List<string>();
        var tasks = empresas.Select(async empresa =>
        {
            var prompt = ObterPromptParaCriarLogoDaEmpresa(empresa);
            var response = await _openAi.Generations(prompt, quantity: 1);
            return response;
        }).ToList();

        await Task.WhenAll(tasks);

        foreach (var task in tasks)
        {
            var response = await task;
            logos.AddRange(response);
        }
        return logos;
    }
    
    private string ObterPromptParaCriarNomeDeEmpresa()
        =>
            $"""
             Crie três nome para uma empresa com essas características: {Caracteristicas}. Retorno os nomes separados por vírgula.
             """;
    
    private string ObterPromptParaCriarLogoDaEmpresa(string nomeDaEmpresa)
        =>
            $"""
             Crie um logotipo minimalista para uma empresa chamada {nomeDaEmpresa}.
             """;
}