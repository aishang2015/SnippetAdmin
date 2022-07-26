namespace SnippetAdmin.EntityFrameworkCore
{
    public class NoDatabaseOptionException : Exception
    {
        public NoDatabaseOptionException() : base("没有配置数据库，无法找到数据库配置片段！") { }
    }
}
