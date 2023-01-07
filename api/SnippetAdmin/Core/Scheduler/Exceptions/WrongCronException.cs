namespace SnippetAdmin.Core.Scheduler.Exceptions
{
	public class WrongCronException : Exception
	{
		public WrongCronException(string message) : base(message)
		{ }
	}
}
