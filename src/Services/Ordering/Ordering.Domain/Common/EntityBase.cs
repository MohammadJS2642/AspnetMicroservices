namespace Ordering.Domain.Common
{
    public abstract class EntityBase
    {
        public int Id { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? LastModifedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}
