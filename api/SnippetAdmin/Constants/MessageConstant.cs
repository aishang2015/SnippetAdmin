namespace SnippetAdmin.Constants
{
    public static class MessageConstant
    {
        #region Common

        public static readonly (string, string) EMPTYTUPLE = (string.Empty, string.Empty);
        public static readonly (string, string) SYSTEM_ERROR_001 = ("SYSTEM_ERROR_001", "发生系统错误！请联系管理员！");

        #endregion Common

        #region AccountController

        public static readonly (string, string) ACCOUNT_INFO_0001 = ("ACCOUNT_INFO_0001", "登录成功！");
        public static readonly (string, string) ACCOUNT_INFO_0002 = ("ACCOUNT_INFO_0002", "第三方信息获取成功，请绑定您的账号！");

        public static readonly (string, string) ACCOUNT_ERROR_0001 = ("ACCOUNT_ERROR_0001", "账号或密码错误！");
        public static readonly (string, string) ACCOUNT_ERROR_0002 = ("ACCOUNT_ERROR_0002", "请输入账号！");
        public static readonly (string, string) ACCOUNT_ERROR_0003 = ("ACCOUNT_ERROR_0003", "请输入密码！");
        public static readonly (string, string) ACCOUNT_ERROR_0004 = ("ACCOUNT_ERROR_0004", "无法识别的第三方登录类型！");
        public static readonly (string, string) ACCOUNT_ERROR_0005 = ("ACCOUNT_ERROR_0005", "第三方登录类型不能是空！");
        public static readonly (string, string) ACCOUNT_ERROR_0006 = ("ACCOUNT_ERROR_0006", "第三方信息缓存密钥不能是空！");
        public static readonly (string, string) ACCOUNT_ERROR_0007 = ("ACCOUNT_ERROR_0007", "第三方账号信息已过期，请返回登陆页面重试！");

        #endregion AccountController

        #region ElementController

        public static readonly (string, string) ELEMENT_INFO_0001 = ("ELEMENT_INFO_0001", "创建成功！");
        public static readonly (string, string) ELEMENT_INFO_0002 = ("ELEMENT_INFO_0002", "删除成功！");
        public static readonly (string, string) ELEMENT_INFO_0003 = ("ELEMENT_INFO_0003", "更新成功！");

        public static readonly (string, string) ELEMENT_ERROR_0001 = ("ELEMENT_ERROR_0001", "请输元素名称！");
        public static readonly (string, string) ELEMENT_ERROR_0002 = ("ELEMENT_ERROR_0002", "元素名称过长！");
        public static readonly (string, string) ELEMENT_ERROR_0003 = ("ELEMENT_ERROR_0003", "请选择元素类型！");
        public static readonly (string, string) ELEMENT_ERROR_0004 = ("ELEMENT_ERROR_0004", "请输入元素标识！");
        public static readonly (string, string) ELEMENT_ERROR_0005 = ("ELEMENT_ERROR_0005", "元素标识过长！");
        public static readonly (string, string) ELEMENT_ERROR_0006 = ("ELEMENT_ERROR_0006", "元素标识只允许数字字母下划线！");

        #endregion ElementController

        #region OrganizationController

        public static readonly (string, string) ORGANIZATION_INFO_0001 = ("ORGANIZATION_INFO_0001", "创建成功！");
        public static readonly (string, string) ORGANIZATION_INFO_0002 = ("ORGANIZATION_INFO_0002", "删除成功！");
        public static readonly (string, string) ORGANIZATION_INFO_0003 = ("ORGANIZATION_INFO_0003", "更新成功！");
        public static readonly (string, string) ORGANIZATION_INFO_0004 = ("ORGANIZATION_INFO_0004", "职位编辑成功！");

        public static readonly (string, string) ORGANIZATION_ERROR_0001 = ("ORGANIZATION_ERROR_0001", "请输入组织名称！");
        public static readonly (string, string) ORGANIZATION_ERROR_0002 = ("ORGANIZATION_ERROR_0002", "组织名称过长！");
        public static readonly (string, string) ORGANIZATION_ERROR_0003 = ("ORGANIZATION_ERROR_0003", "请输入数字字母！");
        public static readonly (string, string) ORGANIZATION_ERROR_0004 = ("ORGANIZATION_ERROR_0004", "组织编码重复！");
        public static readonly (string, string) ORGANIZATION_ERROR_0005 = ("ORGANIZATION_ERROR_0005", "职位编码重复！");
        public static readonly (string, string) ORGANIZATION_ERROR_0006 = ("ORGANIZATION_ERROR_0006", "职位名称重复！");

        #endregion OrganizationController
    }
}