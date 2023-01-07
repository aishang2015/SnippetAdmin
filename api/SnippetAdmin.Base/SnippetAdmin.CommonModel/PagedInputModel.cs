namespace SnippetAdmin.CommonModel
{
	public record PagedInputModel
	{
		public int Page { get; set; }

		public int Size { get; set; }

		public int TakeCount { get => Size; }

		public int SkipCount { get => Size * (Page - 1); }

		public SortModel[] Sorts { get; set; }

	}
}