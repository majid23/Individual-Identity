namespace Individual_Identity.Services.Interfaces
{
    public interface IUserAccessor
    {
        public string GetCurrentUsername();
        public int GetCurrentUserIdentifier();
    }
}
