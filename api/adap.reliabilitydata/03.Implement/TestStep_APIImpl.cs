using adap.safetyandreliabilityapi._05.Data.Reliability_Prediction;
using adap.safetyandreliabilityapi._06.Reports;
using Aspose.Words;
using highspeed.business._01.Models;
using highspeed.business._02.Interface;
using highspeed.framework.Common;
using Quartz.Util;
using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._03.Implement
{
    /// <summary>
    /// 接口API实现
    /// </summary>
    public class TestStep_APIImpl : TestStep_API
    {
        /// <summary>
        /// 获取用例数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<TestStepModel>?> GetTestStepDatas(int TestCaseID)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if(TestCaseID == -1)
                        return TestStep_Handles.GetTestStepDatas();
                    else
                        return TestStep_Handles.GetTestStepDatas(TestCaseID);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 创建测试用例
        /// </summary> 
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        public virtual async Task<bool> CreateTestStep(int TestCaseID, int ParentID, int PreviousID, string? TestStepName, string? StepValue, StepType TestStepType, SendReceiveType sendReceiveType, string? Remark, string? BranchType, int? BindTestStepID, string? IfOperator, string? IfExpectedValue, int? LoopCount)
        {
            return await Task.Run(() =>
            {
                try
                {
                    bool isNormalStep = TestStepType == StepType.Execute || TestStepType == StepType.Temporality || TestStepType == StepType.Recode;
                    if (TestCaseID != -1 && isNormalStep && string.IsNullOrWhiteSpace(TestStepName) && string.IsNullOrWhiteSpace(StepValue))
                    {
                        return false;
                    }

                    return TestStep_Handles.CreateTestStep(TestCaseID, ParentID, PreviousID, TestStepName, StepValue, TestStepType, sendReceiveType, Remark, BranchType, BindTestStepID, IfOperator, IfExpectedValue, LoopCount);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 更新测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        public virtual async Task<bool> UpdateTestStep(int TestCaseID, int TestStepID, string? TestStepName, string? StepValue, StepType TestStepType, SendReceiveType sendReceiveType, string? Remark, int ParentID, int PreviousID, string? BranchType, int? BindTestStepID, string? IfOperator, string? IfExpectedValue, int? LoopCount)
        {
            return await Task.Run(() =>
            {
                try
                {
                    bool isNormalStep = TestStepType == StepType.Execute || TestStepType == StepType.Temporality || TestStepType == StepType.Recode;
                    if (TestCaseID != -1 && TestStepID != -1 && isNormalStep && string.IsNullOrWhiteSpace(TestStepName) && string.IsNullOrWhiteSpace(StepValue))
                    {
                        return false;
                    }

                    return TestStep_Handles.UpdateTestStep(TestCaseID, TestStepID, TestStepName, StepValue, TestStepType, sendReceiveType, Remark, ParentID, PreviousID, BranchType, BindTestStepID, IfOperator, IfExpectedValue, LoopCount);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 删除测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <returns></returns>
        public virtual async Task<bool> DeleteTestStep(int TestStepID)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (TestStepID < 0)
                    {
                        return false;
                    }

                    return TestStep_Handles.DeleteTestStep(TestStepID);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }
    }
}
