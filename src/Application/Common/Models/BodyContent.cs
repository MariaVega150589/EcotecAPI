namespace ProjectAPI.Application.Common.Models;

public class BodyContent
{
    public From? from { get; set; }
    public List<Tos>? tos { get; set; }
    public string? subject { get; set; }
    public string? body { get; set; }
    public List<Attachment>? Attachments { get; set; }

    public bool? bodyIsHTML
    {
        get; set;
    }

    public class Attachment
    {
        public string? Content { get; set; }
        public string? Type { get; set; }
        public string? FileName { get; set; }
    }

    public class From
    {
        public string? name { get; set; }
        public string? email { get; set; }
    }

    public class Tos
    {
        public string? name { get; set; }
        public string? email { get; set; }
    }
}