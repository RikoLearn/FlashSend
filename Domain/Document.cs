namespace Domain
{
    public partial class Document
    {
        public Document(Paper paper, string name, string content, DocumnetType type = DocumnetType.Text)
        {
            Paper = paper ?? throw new ArgumentNullException(nameof(paper));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InsertDateTime = DateTime.Now;
            Content = content;
            Type = type;
        }

        public Document(Paper paper, string name, string filePath, string extension, int fileSize, DocumnetType type)
        {
            Paper = paper ?? throw new ArgumentNullException(nameof(paper));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            InsertDateTime = DateTime.Now;
            FilePath = filePath;
            Extension = extension;
            Type = type;
            FileSize = fileSize;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string? Content { get; private set; }

        public string? FilePath { get; private set; }

        public string? Extension { get; private set; }

        public int? FileSize { get; private set; }

        public DateTime InsertDateTime { get; private set; }

        public DocumnetType Type { get; private set; }
    }
}
