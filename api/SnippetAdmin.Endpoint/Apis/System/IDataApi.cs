using Microsoft.AspNetCore.Mvc;
using SnippetAdmin.CommonModel;

namespace SnippetAdmin.Endpoint.Apis.System
{
	public interface IDataApi
	{
		public Task<CommonResult<List<string>>> GetCsvDataType();

		public Task<FileContentResult> ExportCsvData(IdInputModel<string> model);

		public Task<CommonResult<List<string>>> GetCodeDataType();

		public Task<FileContentResult> ExportCodeData(IdInputModel<string> model);
	}
}
