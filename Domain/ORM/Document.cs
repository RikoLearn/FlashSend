namespace Domain
{
    public partial class Document
    {
        protected Document() { }

        public virtual Paper Paper { get; set; }

        public Guid PaperId { get; set; }
    }
}
