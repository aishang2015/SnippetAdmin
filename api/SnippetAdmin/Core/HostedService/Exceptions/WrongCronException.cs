namespace SnippetAdmin.Core.HostedService.Exceptions
{
    public class WrongCronException : Exception
    {
        public WrongCronException(string message) : base(message)
        { }
    }
}
