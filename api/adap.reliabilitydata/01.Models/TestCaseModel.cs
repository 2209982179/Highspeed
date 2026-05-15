using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._01.Models
{
    /// <summary>
    /// 测试用例数据结构
    /// </summary> 
    public class TestCaseModel
    {
        /// <summary>
        /// 测试用例ID
        /// </summary> 
        public int TestCaseID { get; set; } = -1;

        /// <summary>
        /// 父Id
        /// </summary>
        //public int ParentID { get; set; } = -1;

        /// <summary>
        /// 测试用例GUID
        /// </summary> 
        public string? TestCaseGUID { get; set; }

        /// <summary>
        /// 系统名称
        /// </summary>
        public string? ExtendedField1 { get; set; }
        //public string? SysName { get; set; }

        /// <summary>
        /// 试验名称
        /// </summary>
        public string? ExtendedField2 { get; set; }
        //public string? TestName { get; set; }

        /// <summary>
        /// 测试用例名称
        /// </summary> 
        public string? TestCaseName { get; set; }

        /// <summary>
        /// 测试用例类型
        /// </summary>  
        //public CaseType TestCaseType { get; set; }

        /// <summary>
        /// 车型
        /// </summary>  
        public TrainType Model { get; set; }


        /// <summary>
        /// 试验对象
        /// </summary>
        public TrainType TrainObject { get; set; }

        /// <summary>
        /// 试验速度
        /// </summary>
        public TrainType TrainSpeed { get; set; }

        
        /// <summary>
        /// 载荷状态
        /// </summary>  
        public TrainType TrainType { get; set; }

        /// <summary>
        /// 测试步骤-指令清单id合集
        /// </summary>
        public List<int>? ExtendedField3 { get; set; }
        //public string? TestCommandIds { get; set; }

        /// <summary>
        /// 初始化参数清单id合集
        /// </summary>
        public List<int>? ExtendedField4 { get; set; }

        /// <summary>
        /// 检查参数清单id合集
        /// </summary>
        public List<int>? ExtendedField5 { get; set; }

        /// <summary>
        /// 列车信息绑定id
        /// </summary>
        public String ExtendedField6 { get; set; }

        /// <summary>
        /// 测试步骤-指令清单
        /// </summary>
        public List<TestStepModel>? TestCommands { get; set; }

        /// <summary>
        /// 初始化参数清单
        /// </summary>
        public List<TestParameterModel>? TestInitializeParameters = new List<TestParameterModel>();

        /// <summary>
        /// 检查参数清单
        /// </summary>
        public List<TestRecodeModel>? TestRecodes = new List<TestRecodeModel>();

        /// <summary>
        /// 列车信息
        /// </summary>
        public TestTrainInformationModel? TestTrainInformation { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary> 
        public string? CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public string? ModificationTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary> 
        public string? Remark { get; set; }

        /// <summary>
        /// 执行车头
        /// </summary>
        public string? ExtendedField7 { get; set; } = "M1主控";
        //ExecuteCar

        /// <summary>
        /// 制动状态
        /// </summary>
        public string? ExtendedField8 { get; set; } = "制动缓解";
        //BrakeState

        /// <summary>
        /// 载荷状态 发送列车类型
        /// </summary>
        public string? ExtendedField9 { get; set; }
        //LoadState

        /// <summary>
        /// 拓展字段10
        /// </summary>
        public string? ExtendedField10 { get; set; }
    }
}
