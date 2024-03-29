namespace SnippetAdmin.Constants
{
    public static class MessageConstant
    {
        #region Common

        public static readonly (string, string) EMPTYTUPLE = (string.Empty, string.Empty);
        public static readonly (string, string) SYSTEM_ERROR_001 = ("SYSTEM_ERROR_001", "发生系统错误！请联系管理员！");
        public static readonly (string, string) SYSTEM_ERROR_002 = ("SYSTEM_ERROR_002", "页码不能小于0！");
        public static readonly (string, string) SYSTEM_ERROR_003 = ("SYSTEM_ERROR_003", "页面大小不能小于0！");
        public static readonly (string, string) SYSTEM_ERROR_004 = ("SYSTEM_ERROR_004", "排序字段错误！");
        public static readonly (string, string) SYSTEM_ERROR_005 = ("SYSTEM_ERROR_005", "分表不存在！");

        public static readonly (string, string) SYSTEM_INFO_001 = ("SYSTEM_INFO_001", "删除成功！");
        public static readonly (string, string) SYSTEM_INFO_002 = ("SYSTEM_INFO_002", "更新成功！");
        public static readonly (string, string) SYSTEM_INFO_003 = ("SYSTEM_INFO_003", "添加成功！");
        public static readonly (string, string) SYSTEM_INFO_004 = ("SYSTEM_INFO_004", "操作成功！");

        #endregion Common

        #region AccountController

        public static readonly (string, string) ACCOUNT_INFO_0001 = ("ACCOUNT_INFO_0001", "登录成功！");
        public static readonly (string, string) ACCOUNT_INFO_0002 = ("ACCOUNT_INFO_0002", "第三方信息获取成功，请绑定您的账号！");
        public static readonly (string, string) ACCOUNT_INFO_0003 = ("ACCOUNT_INFO_0003", "密码修改成功！");

        public static readonly (string, string) ACCOUNT_ERROR_0001 = ("ACCOUNT_ERROR_0001", "账号或密码错误！");
        public static readonly (string, string) ACCOUNT_ERROR_0002 = ("ACCOUNT_ERROR_0002", "请输入账号！");
        public static readonly (string, string) ACCOUNT_ERROR_0003 = ("ACCOUNT_ERROR_0003", "请输入密码！");
        public static readonly (string, string) ACCOUNT_ERROR_0004 = ("ACCOUNT_ERROR_0004", "无法识别的第三方登录类型！");
        public static readonly (string, string) ACCOUNT_ERROR_0005 = ("ACCOUNT_ERROR_0005", "第三方登录类型不能是空！");
        public static readonly (string, string) ACCOUNT_ERROR_0006 = ("ACCOUNT_ERROR_0006", "第三方信息缓存密钥不能是空！");
        public static readonly (string, string) ACCOUNT_ERROR_0007 = ("ACCOUNT_ERROR_0007", "第三方账号信息已过期，请返回登陆页面重试！");
        public static readonly (string, string) ACCOUNT_ERROR_0008 = ("ACCOUNT_ERROR_0008", "无法获取账号登录凭证，请重新登录！");
        public static readonly (string, string) ACCOUNT_ERROR_0009 = ("ACCOUNT_ERROR_0009", "账号被别人登录！");
        public static readonly (string, string) ACCOUNT_ERROR_0010 = ("ACCOUNT_ERROR_0010", "Jwt不合法！");
        public static readonly (string, string) ACCOUNT_ERROR_0011 = ("ACCOUNT_ERROR_0011", "刷新token已过期！");
        public static readonly (string, string) ACCOUNT_ERROR_0012 = ("ACCOUNT_ERROR_0012", "账号未激活！");
        public static readonly (string, string) ACCOUNT_ERROR_0013 = ("ACCOUNT_ERROR_0013", "用户旧密码错误！");
        public static readonly (string, string) ACCOUNT_ERROR_0014 = ("ACCOUNT_ERROR_0014", "密码必须包含数字！");
        public static readonly (string, string) ACCOUNT_ERROR_0015 = ("ACCOUNT_ERROR_0015", "密码必须包含大写字母！");
        public static readonly (string, string) ACCOUNT_ERROR_0016 = ("ACCOUNT_ERROR_0016", "密码必须包含小写字母！");
        public static readonly (string, string) ACCOUNT_ERROR_0017 = ("ACCOUNT_ERROR_0017", "密码必须包含特殊符号！");
        public static readonly (string, string) ACCOUNT_ERROR_0018 = ("ACCOUNT_ERROR_0018", "密码长度必须大于等于{0}！");

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
        public static readonly (string, string) ORGANIZATION_INFO_0005 = ("ORGANIZATION_INFO_0005", "操作成功！");

        public static readonly (string, string) ORGANIZATION_ERROR_0001 = ("ORGANIZATION_ERROR_0001", "请输入组织名称！");
        public static readonly (string, string) ORGANIZATION_ERROR_0002 = ("ORGANIZATION_ERROR_0002", "组织名称过长！");
        public static readonly (string, string) ORGANIZATION_ERROR_0003 = ("ORGANIZATION_ERROR_0003", "请输入数字字母！");
        public static readonly (string, string) ORGANIZATION_ERROR_0004 = ("ORGANIZATION_ERROR_0004", "组织编码重复！");
        public static readonly (string, string) ORGANIZATION_ERROR_0006 = ("ORGANIZATION_ERROR_0006", "职位名称重复！");
        public static readonly (string, string) ORGANIZATION_ERROR_0007 = ("ORGANIZATION_ERROR_0007", "组织类型编码重复！");
        public static readonly (string, string) ORGANIZATION_ERROR_0008 = ("ORGANIZATION_ERROR_0008", "组织类型名称重复！");
        public static readonly (string, string) ORGANIZATION_ERROR_0009 = ("ORGANIZATION_ERROR_0009", "无法将组织变更为自己组织的下属组织！");

        #endregion OrganizationController

        #region PositionController

        public static readonly (string, string) POSITION_INFO_0001 = ("POSITION_INFO_0001", "保存成功！");
        public static readonly (string, string) POSITION_INFO_0002 = ("POSITION_INFO_0002", "删除成功！");
        public static readonly (string, string) POSITION_INFO_0003 = ("POSITION_INFO_0003", "状态设置成功！");

        public static readonly (string, string) POSITION_ERROR_0001 = ("POSITION_ERROR_0001", "职位名重复！");
        public static readonly (string, string) POSITION_ERROR_0002 = ("POSITION_ERROR_0002", "职位编码重复！");

        #endregion

        #region RoleController

        public static readonly (string, string) ROLE_INFO_0001 = ("ROLE_INFO_0001", "保存成功！");
        public static readonly (string, string) ROLE_INFO_0002 = ("ROLE_INFO_0002", "删除成功！");
        public static readonly (string, string) ROLE_INFO_0004 = ("ROLE_INFO_0004", "状态设置成功！");

        public static readonly (string, string) ROLE_ERROR_0001 = ("ROLE_ERROR_0001", "请输入角色名！");
        public static readonly (string, string) ROLE_ERROR_0002 = ("ROLE_ERROR_0002", "角色名过长！");
        public static readonly (string, string) ROLE_ERROR_0003 = ("ROLE_ERROR_0003", "请输入角色代码！");
        public static readonly (string, string) ROLE_ERROR_0004 = ("ROLE_ERROR_0004", "角色代码过长！");
        public static readonly (string, string) ROLE_ERROR_0005 = ("ROLE_ERROR_0005", "角色代码只允许数字字母下划线！");
        public static readonly (string, string) ROLE_ERROR_0006 = ("ROLE_ERROR_0006", "备注过长！");
        public static readonly (string, string) ROLE_ERROR_0007 = ("ROLE_ERROR_0007", "角色名重复！");
        public static readonly (string, string) ROLE_ERROR_0008 = ("ROLE_ERROR_0008", "角色代码重复！");

        #endregion RoleController

        #region UserController

        public static readonly (string, string) USER_INFO_0001 = ("USER_INFO_0001", "操作成功！");
        public static readonly (string, string) USER_INFO_0002 = ("USER_INFO_0002", "删除成功！");
        public static readonly (string, string) USER_INFO_0003 = ("USER_INFO_0003", "密码设置成功！");
        public static readonly (string, string) USER_INFO_0004 = ("USER_INFO_0004", "成员添加成功！");
        public static readonly (string, string) USER_INFO_0005 = ("USER_INFO_0005", "成员移出成功！");

        public static readonly (string, string) USER_ERROR_0001 = ("USER_ERROR_0001", "请选择成员！");
        public static readonly (string, string) USER_ERROR_0002 = ("USER_ERROR_0002", "请输入用户名！");
        public static readonly (string, string) USER_ERROR_0003 = ("USER_ERROR_0003", "用户名过长！");
        public static readonly (string, string) USER_ERROR_0004 = ("USER_ERROR_0004", "用户名只允许数字字母！");
        public static readonly (string, string) USER_ERROR_0005 = ("USER_ERROR_0005", "请输入姓名！");
        public static readonly (string, string) USER_ERROR_0006 = ("USER_ERROR_0006", "姓名过长！");
        public static readonly (string, string) USER_ERROR_0007 = ("USER_ERROR_0007", "请输入密码！");
        public static readonly (string, string) USER_ERROR_0008 = ("USER_ERROR_0008", "密码过长！");
        public static readonly (string, string) USER_ERROR_0009 = ("USER_ERROR_0009", "请输入确认密码！");
        public static readonly (string, string) USER_ERROR_0011 = ("USER_ERROR_0011", "两次输入的密码不一致！");

        public static readonly (string, string) USER_ERROR_0012 = ("USER_ERROR_0012", "用户名重复！");

        #endregion UserController

        #region JobController
        public static readonly (string, string) JOB_INFO_0001 = ("JOB_INFO_0001", "创建成功！");
        public static readonly (string, string) JOB_INFO_0002 = ("JOB_INFO_0002", "删除成功！");
        public static readonly (string, string) JOB_INFO_0003 = ("JOB_INFO_0003", "更新成功！");
        public static readonly (string, string) JOB_INFO_0004 = ("JOB_INFO_0004", "任务已启用！");
        public static readonly (string, string) JOB_INFO_0005 = ("JOB_INFO_0005", "任务已停止！");
        public static readonly (string, string) JOB_INFO_0006 = ("JOB_INFO_0006", "任务开始执行！");

        public static readonly (string, string) JOB_ERROR_0001 = ("JOB_ERROR_0001", "没有找到该任务！");
        public static readonly (string, string) JOB_ERROR_0002 = ("JOB_ERROR_0002", "Cron表达式错误！");
        public static readonly (string, string) JOB_ERROR_0003 = ("JOB_ERROR_0003", "任务名称不能重复！");

        #endregion UserController

        #region DicController

        public static readonly (string, string) DICTIONARY_INFO_0001 = ("DICTIONARY_INFO_0001", "字典类型创建成功！");
        public static readonly (string, string) DICTIONARY_INFO_0002 = ("DICTIONARY_INFO_0002", "字典类型更新成功！");
        public static readonly (string, string) DICTIONARY_INFO_0003 = ("DICTIONARY_INFO_0003", "字典类型删除成功！");
        public static readonly (string, string) DICTIONARY_INFO_0004 = ("DICTIONARY_INFO_0004", "字典项目创建成功！");
        public static readonly (string, string) DICTIONARY_INFO_0005 = ("DICTIONARY_INFO_0005", "字典项目更新成功！");
        public static readonly (string, string) DICTIONARY_INFO_0006 = ("DICTIONARY_INFO_0006", "字典项目删除成功！");
        public static readonly (string, string) DICTIONARY_INFO_0007 = ("DICTIONARY_INFO_0007", "字典项目状态修改成功！");

        public static readonly (string, string) DICTIONARY_ERROR_0001 = ("DICTIONARY_ERROR_0001", "字典类型名称重复！");
        public static readonly (string, string) DICTIONARY_ERROR_0002 = ("DICTIONARY_ERROR_0002", "字典类型代码重复！");
        public static readonly (string, string) DICTIONARY_ERROR_0003 = ("DICTIONARY_ERROR_0003", "同一字典类型下字典项目名称重复！");
        public static readonly (string, string) DICTIONARY_ERROR_0004 = ("DICTIONARY_ERROR_0004", "同一字典类型下字典项目代码重复！");

        public static readonly (string, string) DICTIONARY_ERROR_0005 = ("DICTIONARY_ERROR_0005", "请输入字典类型名称！");
        public static readonly (string, string) DICTIONARY_ERROR_0006 = ("DICTIONARY_ERROR_0006", "字典类型名称过长！");
        public static readonly (string, string) DICTIONARY_ERROR_0007 = ("DICTIONARY_ERROR_0007", "请输入字典类型编码！");
        public static readonly (string, string) DICTIONARY_ERROR_0008 = ("DICTIONARY_ERROR_0008", "字典类型编码过长！");
        public static readonly (string, string) DICTIONARY_ERROR_0009 = ("DICTIONARY_ERROR_0009", "字典类型编码只允许数字字母下划线！");

        public static readonly (string, string) DICTIONARY_ERROR_0010 = ("DICTIONARY_ERROR_0010", "请输入字典项目名称！");
        public static readonly (string, string) DICTIONARY_ERROR_0011 = ("DICTIONARY_ERROR_0011", "字典项目名称过长！");
        public static readonly (string, string) DICTIONARY_ERROR_0012 = ("DICTIONARY_ERROR_0012", "请输入字典项目编码！");
        public static readonly (string, string) DICTIONARY_ERROR_0013 = ("DICTIONARY_ERROR_0013", "字典项目编码过长！");
        public static readonly (string, string) DICTIONARY_ERROR_0014 = ("DICTIONARY_ERROR_0014", "字典项目编码只允许数字字母下划线！");

        #endregion

        #region SettingController

        public static readonly (string, string) SETTING_INFO_0001 = ("SETTING_INFO_0001", "{0}保存成功！");

        public static readonly (string, string) SETTING_ERROR_0001 = ("SETTING_ERROR_0001", "没有找到该配置信息，请联系管理员！");

        #endregion

        #region Sharding

        public static readonly (string, string) SHARDING_ERROR_001 = ("SHARDING_ERROR_001", "该月记录不存在！");

        #endregion
    }
}