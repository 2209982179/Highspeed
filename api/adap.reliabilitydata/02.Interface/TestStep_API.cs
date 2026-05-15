using highspeed.business._01.Models;
using Google.Protobuf.WellKnownTypes;
using System.Runtime.InteropServices;
using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._02.Interface
{
    /// <summary>
    /// 接口API
    /// </summary>
    public interface TestStep_API
    {
        /// <summary>
        /// 根据id查找测试步骤列表
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        Task<List<TestStepModel>?> GetTestStepDatas(int TestCaseID);

        /// <summary>
        /// 创建测试步骤
        /// </summary> 
        /// <param name="TestCaseName">步骤名称</param>
        /// <param name="TestCaseType">步骤类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        Task<bool> CreateTestStep(int TestCaseID, int ParentID, int PreviousID, string? TestStepName, string? StepValue, StepType TestStepType, SendReceiveType sendReceiveType, string? Remark, string? BranchType, int? BindTestStepID, string? IfOperator, string? IfExpectedValue, int? LoopCount);

        /// <summary>
        /// 更新测试步骤
        /// </summary>
        Task<bool> UpdateTestStep(int TestCaseID, int TestStepID, string? TestStepName, string? StepValue, StepType TestStepType, SendReceiveType sendReceiveType, string? Remark, int ParentID, int PreviousID, string? BranchType, int? BindTestStepID, string? IfOperator, string? IfExpectedValue, int? LoopCount);

        /// <summary>
        /// 删除测试步骤
        /// </summary>
        /// <param name="TestCaseID">步骤ID</param>
        /// <returns></returns>
        Task<bool> DeleteTestStep(int TestStepID);

    }
}
