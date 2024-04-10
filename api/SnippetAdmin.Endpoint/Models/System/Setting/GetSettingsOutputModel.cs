namespace SnippetAdmin.Endpoint.Models.System.Setting
{
    public class GetSettingsOutputModel
    {
        public List<Setting> Settings { get; set; }
    }

    public class Setting
    {
        public string? Key { get; set; }

        public string? Value { get; set; }
    }
}
