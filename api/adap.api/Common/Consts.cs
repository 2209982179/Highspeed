namespace highspeed.api.Common
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class Consts
    {
        public const string HeaderKey_Authorization = "authorization";
        public const string HeaderKey_PermissionCheck = "Permission-Check";
        public const string HeaderKey_Readonly = "Readonly";
        public const string HeaderKey_FileName = "file-name";

        public const string HeaderKey_ContentType_Json = "application/json";
        public const string HeaderKey_ContentType_GZip_Json = "application/gzip-json";
        public const string HeaderKey_ContentType_Form_Data = "multipart/form-data";
        public const string HeaderKey_ContentType_Form_Urlencoded = "application/x-www-form-urlencoded";
        public const string HeaderKey_ContentType_Stream = "application/octet-stream";
        public const string HeaderKey_ContentType_Xlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public const int Auth_TimeoutMin = 60 * 24; //30;
    }
}
