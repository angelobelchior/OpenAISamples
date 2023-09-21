namespace OpenAISamples.Models;

public class Response
{
    public string id { get; set; }
    public List<Choice> choices { get; set; }
    public long created { get; set; }
    public List<Link>? data { get; set; }
}