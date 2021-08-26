namespace SnippetAdmin.Constants
{
    public class RegexPatternConstant
    {
        public const string NumberRegex = @"^[0-9]*$";                              // 数字
        public const string IntRegex = @"^(0|[1-9][0-9]*)$";                        // 整数
        public const string FloatRegex = @"^\d+(\.\d+)?$";                          // 小数
        public const string ChineseRegex = @"^[\u4e00-\u9fa5]*$";                   // 中文
        public const string LetterNumberRegex = @"^[A-Za-z0-9]*$";                  // 字母数字
        public const string UppercaseRegex = @"^[A-Z]*$";                           // 大写字母
        public const string LowercaseRegex = @"^[a-z]*$";                           // 小写字母
        public const string LetterRegex = @"^[A-Za-z]*$";                           // 字母
        public const string AllCharacterRegex = @"^[\u4E00-\u9FA5A-Za-z0-9]*$";     // 全字符

        public const string EmailRegex = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";                              // 邮箱地址
        public const string RegRegex = @"[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(/.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+/.?";           // 域名
        public const string TelephoneRegex = @"^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$";   // 手机号码
        public const string IdRegex = @"^\d{8,18}|[0-9x]{8,18}|[0-9X]{8,18}?$";                                         // 身份ID
        public const string IpRegex = @"\d+\.\d+\.\d+\.\d+";                                                            // Ip地址

        public const string GuidRegex = @"^[0-9a-f]{8}(-[0-9a-f]{4}){3}-[0-9a-f]{12}$";     // GUID
    }
}