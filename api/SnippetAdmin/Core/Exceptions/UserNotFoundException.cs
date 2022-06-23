namespace SnippetAdmin.Core.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("user not exist!") { }
    }
}
