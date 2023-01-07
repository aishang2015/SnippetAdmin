namespace SnippetAdmin.PluginBase.State
{
	public static class ApplicationRestartState
	{
		private static Mutex _mutex = new Mutex();

		private static volatile bool _canRestart = true;

		public static bool CanRestart
		{
			private set
			{
				_canRestart = value;
			}
			get
			{
				_mutex.WaitOne();
				var oldCanRestart = _canRestart;
				_canRestart = _canRestart ? !_canRestart : _canRestart;
				_mutex.ReleaseMutex();
				return oldCanRestart;
			}
		}

		public static void AllowRestart()
		{
			_mutex.WaitOne();
			_canRestart = !_canRestart ? !_canRestart : _canRestart;
			_mutex.ReleaseMutex();
		}
	}
}
