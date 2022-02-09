namespace Individual_Identity.Core
{
    public interface ISoftDeletedEntity
    {
        public bool Deleted { get; set; }
    }
}
