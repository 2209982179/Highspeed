using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._01.Models
{
    /// <summary>
    /// 参数数据结构
    /// </summary> 
    public class TestRecodeModel
    {
        /// <summary>
        /// 参数ID
        /// </summary> 
        public int RecodeID { get; set; } = -1;

        /// <summary>
        /// 参数名称
        /// </summary> 
        public string? RecodeName { get; set; }

        /// <summary>
        /// 备注
        /// </summary> 
        public string? Remark { get; set; }

        /// <summary>
        /// 拓展字段1
        /// </summary>
        public string? ExtendedField1 { get; set; }

        /// <summary>
        /// 拓展字段2
        /// </summary>
        public string? ExtendedField2 { get; set; }

        /// <summary>
        /// 拓展字段3
        /// </summary>
        public string? ExtendedField3 { get; set; }
    }
}
