using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public partial class Paper
    {
        public Paper(TimeSpan expireTime, int uniqueNumber)
        {
            Documents = new List<Document>();
            InsertDateTime = DateTime.Now;

            if (expireTime == TimeSpan.MinValue)
                throw new Exception(nameof(ExpireTime));

            ExpireTime = expireTime;
            UniqueNumber = uniqueNumber;
        }

        [Key]
        public Guid Id { get;private set; }

        [Range(100000, 999999)]
        public int UniqueNumber { get; private set; }

        public DateTime InsertDateTime { get; private set; }

        public TimeSpan ExpireTime { get; private set; }
    }
}
