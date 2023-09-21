namespace OpenAISamples.Models;

public class Prompt
{
    public string model { get; set; }
    public List<Message> messages { get; set; }
    public double temperature { get; set; }

    public static Prompt Create(string model, string prompt, double temperature = 0.7)
    {
        return new Prompt
        {
            model = model,
            messages = new List<Message>
            {
                new() { role = "user", content = prompt }
            },
            temperature = temperature
        };
    }
}