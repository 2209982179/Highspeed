using Castle.DynamicProxy;
using highspeed.api.Common;
using highspeed.business._01.Models;
using highspeed.business._03.Implement;
using Microsoft.AspNetCore.Mvc;
using static highspeed.business._00.Enum.CommonEnum;
namespace highspeed.api.Controllers
{
    /// <summary>
    /// 通用Control接口
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class TestStepController : ControllerBase
    {
        /// <summary>
        /// 测试用例API
        /// </summary>
        public TestStep_APIImpl API;

        /// <summary>
        /// 接口实例化
        /// </summary>
        public TestStepController()
        {
            ProxyGenerator generator = new ProxyGenerator();//实例化【代理类生成器】  
            API = generator.CreateClassProxy<TestStep_APIImpl>();
        }

        /// <summary>
        /// 获取测试用例
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<List<TestStepModel>?> GetTestStepDatas(int TestCaseID)
        {
            if (TestCaseID == null)
                TestCaseID = -1;

            return await API.GetTestStepDatas(TestCaseID);
        }

        /// <summary>
        /// 创建测试用例
        /// </summary> 
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<bool> CreateTestStep(int TestCaseID, int ParentID = 0, int PreviousID = -1, string? TestStepName = null, string? StepValue = null, StepType TestStepType = StepType.Execute, SendReceiveType sendReceiveType = SendReceiveType.Other, string? Remark = null, string? BranchType = null, int? BindTestStepID = null, string? IfOperator = null, string? IfExpectedValue = null, int? LoopCount = null)
        {
            return await API.CreateTestStep(TestCaseID, ParentID, PreviousID, TestStepName, StepValue, TestStepType, sendReceiveType, Remark, BranchType, BindTestStepID, IfOperator, IfExpectedValue, LoopCount);
        }

        /// <summary>
        /// 更新测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<bool> UpdateTestStep(int TestCaseID, int TestStepID, string? TestStepName = null, string? StepValue = null, StepType TestStepType = StepType.Execute, SendReceiveType sendReceiveType = SendReceiveType.Other, string? Remark = null, int ParentID = 0, int PreviousID = -1, string? BranchType = null, int? BindTestStepID = null, string? IfOperator = null, string? IfExpectedValue = null, int? LoopCount = null)
        {
            return await API.UpdateTestStep(TestCaseID, TestStepID, TestStepName, StepValue, TestStepType, sendReceiveType, Remark, ParentID, PreviousID, BranchType, BindTestStepID, IfOperator, IfExpectedValue, LoopCount);
        }

        /// <summary>
        /// 删除测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<bool> DeleteTestStep(int TestStepID)
        {
            return await API.DeleteTestStep(TestStepID);
        }
    }
}
