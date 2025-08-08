namespace Domain
{
    public partial class Paper
    {
        protected Paper()
        {
            Documents = new List<Document>();
        }

        public virtual IList<Document> Documents { get; set; }
    }
}
