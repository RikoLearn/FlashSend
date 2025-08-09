namespace Domain
{
    public partial class Document
    {
        protected Document()
        {
            Paper = null!;
            Name = null!;
        }

        public virtual Paper Paper { get; set; }

        public Guid PaperId { get; set; }
    }
}
