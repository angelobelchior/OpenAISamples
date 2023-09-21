using System.Text;
using System.Text.Json;
using OpenAISamples.Models;

namespace OpenAISamples.Services;

public class OpenAI
{
    private const string COMPLETIONS_URL = "https://api.openai.com/v1/chat/completions";
    private const string GENERATIONS_URL = "https://api.openai.com/v1/images/generations";

    private readonly string _apiKey;
    
    public OpenAI(string apiKey) => _apiKey = apiKey;
    
    public async Task<string> Completions(string content)
    {
        var prompt = Prompt.Create("gpt-3.5-turbo-16k", content); //https://platform.openai.com/docs/models/overview
        var response = await Post<Response, Prompt>(COMPLETIONS_URL, prompt);
        return response?.choices.FirstOrDefault()?.message?.content ?? "Não foi possível obter o response";
    }

    public async Task<IEnumerable<string>> Generations(string prompt, short quantity = 3, string size = "256x256")
    {
        var input = new Input
        {
            prompt = prompt,
            n = quantity,
            size = size
        };
        
        var response = await Post<Response, Input>(GENERATIONS_URL, input);
        return response?.data?.Select(x => x.url!) ?? Enumerable.Empty<string>();
    }
    
    private async Task<TResponse?> Post<TResponse, TRequest>(string url, TRequest request)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        
        var json = JsonSerializer.Serialize(request);
        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, stringContent);
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<TResponse>(responseBody);
        return responseObject ?? default;
    }
}