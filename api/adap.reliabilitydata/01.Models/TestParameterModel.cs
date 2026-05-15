using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._01.Models
{
    /// <summary>
    /// 参数数据结构
    /// </summary> 
    public class TestParameterModel
    {
        /// <summary>
        /// 参数ID
        /// </summary> 
        public int ParameterID { get; set; } = -1;

        /// <summary>
        /// 参数名称
        /// </summary> 
        public string? ParameterKey { get; set; }

        /// <summary>
        /// 参数描述
        /// </summary>  
        public string? ParameterDescription { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        public object? DefaultValue { get; set; }

        /// <summary>
        /// 测试值
        /// </summary>
        public object? TestValue { get; set; }

        /// <summary>
        /// 参数值的类型
        /// </summary>
        public ParameterValueType? ParameterValueType { get; set; }

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

        /// <summary>
        /// 拓展字段4
        /// </summary>
        public string? ExtendedField4 { get; set; }

        /// <summary>
        /// 拓展字段5
        /// </summary>
        public string? ExtendedField5 { get; set; }

        /// <summary>
        /// 拓展字段6
        /// </summary>
        public string? ExtendedField6 { get; set; }

        /// <summary>
        /// 拓展字段7
        /// </summary>
        public string? ExtendedField7 { get; set; }

        /// <summary>
        /// 拓展字段8
        /// </summary>
        public string? ExtendedField8 { get; set; }

        /// <summary>
        /// 拓展字段9
        /// </summary>
        public string? ExtendedField9 { get; set; }

        /// <summary>
        /// 拓展字段10
        /// </summary>
        public string? ExtendedField10 { get; set; }
    }
}
