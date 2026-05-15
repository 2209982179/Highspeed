using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._01.Models
{
    /// <summary>
    /// 指令数据结构
    /// </summary> 
    public class TestStepModel
    {
        /// <summary>
        /// 测试用例ID
        /// </summary> 
        public int TestCaseID { get; set; } = -1;

        /// <summary>
        /// 指令ID
        /// </summary> 
        public int TestStepID { get; set; } = -1;

        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentID { get; set; } = 0;

        /// <summary>
        /// 当前的排序
        /// </summary>
        public int PreviousID { get; set; } = -1;

        /// <summary>
        /// 指令类型
        /// </summary>  
        public StepType TestStepType { get; set; }

        /// <summary>
        /// 指令类型
        /// </summary>  
        public SendReceiveType sendReceiveType { get; set; }

        /// <summary>
        /// 指令名称
        /// </summary> 
        public string? TestStepName { get; set; }

        /// <summary>
        /// 指令描述
        /// </summary>  
        public string? StepDescription { get; set; }

        /// <summary>
        /// 指令值
        /// </summary>
        public object? StepValue { get; set; }

        /// <summary>
        /// 指令值的类型
        /// </summary>
        public ParameterValueType? ParameterValueType { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary> 
        public string? CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string? ModificationTime { get; set; }

        /// <summary>
        /// 执行状况
        /// </summary> 
        public string? Execute { get; set; }

        /// <summary>
        /// 是否打印
        /// </summary> 
        public string? IfPrint { get; set; }

        /// <summary>
        /// 备注
        /// </summary> 
        public string? Remark { get; set; }

        /// <summary>
        /// 测试步骤车辆类型
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

        /// <summary>
        /// 分支类型 then/body
        /// </summary>
        public string? BranchType
        {
            get => ExtendedField1;
            set => ExtendedField1 = value;
        }

        /// <summary>
        /// IF 绑定的基础信号步骤ID
        /// </summary>
        public int? BindTestStepID
        {
            get
            {
                if (int.TryParse(ExtendedField2, out int value))
                {
                    return value;
                }

                return null;
            }
            set => ExtendedField2 = value.HasValue ? value.Value.ToString() : null;
        }

        /// <summary>
        /// IF 比较符号
        /// </summary>
        public string? IfOperator
        {
            get => ExtendedField3;
            set => ExtendedField3 = value;
        }

        /// <summary>
        /// IF 比较值
        /// </summary>
        public string? IfExpectedValue
        {
            get => ExtendedField4;
            set => ExtendedField4 = value;
        }

        /// <summary>
        /// IF 判断条件显示文本
        /// </summary>
        public string? IfCondition
        {
            get
            {
                if (string.IsNullOrWhiteSpace(IfOperator) || string.IsNullOrWhiteSpace(IfExpectedValue))
                {
                    return null;
                }

                return $"{IfOperator} {IfExpectedValue}";
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    IfOperator = null;
                    IfExpectedValue = null;
                    return;
                }

                string[] operators = new[] { ">=", "<=", "==", ">", "<" };
                string expression = value.Trim();
                string? matchOperator = operators.FirstOrDefault(expression.StartsWith);
                if (string.IsNullOrWhiteSpace(matchOperator))
                {
                    IfOperator = null;
                    IfExpectedValue = expression;
                    return;
                }

                IfOperator = matchOperator;
                IfExpectedValue = expression.Substring(matchOperator.Length).Trim();
            }
        }

        /// <summary>
        /// FOR 循环次数
        /// </summary>
        public int LoopCount
        {
            get
            {
                if (int.TryParse(ExtendedField5, out int value) && value > 0)
                {
                    return value;
                }

                return 1;
            }
            set => ExtendedField5 = value > 0 ? value.ToString() : "1";
        }
    }
}
