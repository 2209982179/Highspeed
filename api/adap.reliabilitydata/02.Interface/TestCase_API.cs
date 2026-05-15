using highspeed.business._01.Models;
using Google.Protobuf.WellKnownTypes;
using System.Runtime.InteropServices;
using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._02.Interface
{
    /// <summary>
    /// 接口API
    /// </summary>
    public interface TestCase_API
    {
        /// <summary> 
        /// 获取用例列表
        /// </summary>
        /// <param name="ItemID"></param>
        /// <returns></returns>
        Task<TestCaseMixClass?> GetTestCaseList();

        /// <summary>
        /// 根据id查找测试列车信息
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        Task<TestTrainInformationModel?> GetTrainDatas(String TrainName);

        /// <summary>
        /// 更新列车信息
        /// </summary>
        /// <param name="TrainName"></param>
        /// <param name="FormationType"></param>
        /// <param name="VehicleLoad"></param>
        /// <param name="MaximumOperatingSpeed"></param>
        /// <param name="AverageTravelSpeed"></param>
        /// <param name="AverageDwellTime"></param>
        /// <param name="AverageAcceleration"></param>
        /// <param name="AverageDeceleration"></param>
        /// <param name="TrainImpactRate"></param>
        /// <param name="RequirementsFaultStatus"></param>
        /// <param name="RequirementsRampRescue"></param>
        /// <param name="RequirementsNoiseLevelIn"></param>
        /// <param name="RequirementsNoiseLevelOut"></param>
        /// <returns></returns>
        Task<bool> UpdateTrainDatas(string TrainName, string? FormationType, string? VehicleLoad, string? MaximumOperatingSpeed, string? AverageTravelSpeed, string? AverageDwellTime, string? AverageAcceleration, string? AverageDeceleration,
            string? TrainImpactRate, string? RequirementsFaultStatus, string? RequirementsRampRescue, string? RequirementsNoiseLevelIn, string? RequirementsNoiseLevelOut);

        /// <summary>
        /// 获取记录信号
        /// </summary>
        /// <returns></returns>
        Task<List<TestRecodeModel>?> GetTestRecodeDatas();

        /// <summary>
        /// 更新记录信号
        /// </summary>
        /// <returns></returns>
        Task<bool> UpdateTestRecodeDatas(int TestCaseID, List<string> RecodeNames);


        /// <summary>
        /// 查询用例对应的监控信号
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        Task<List<TestRecodeModel>?> GetOneTestRecodeDatas(int TestCaseID, List<TestRecodeModel> testRecodes);

        /// <summary>
        /// 创建测试用例
        /// </summary> 
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        Task<bool> AddNewTestCase(string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark);

        /// <summary>
        /// 更新测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        Task<bool> UpdateTestCase(int TestCaseID, string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark);

        /// <summary>
        /// 修改测试用例前置条件
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <param name="ExtendedField7"></param>
        /// <param name="ExtendedField8"></param>
        /// <returns></returns>
        Task<bool> UpdatePreconditions(int TestCaseID, string ExtendedField7, string ExtendedField8);

        /// <summary>
        /// 删除测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <returns></returns>
        Task<bool> DeleteTestCase(int TestCaseID);

        /// <summary>
        /// 执行测试用例
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        Task<bool> ExecuteTestCase(int TestCaseID);

        /// <summary>
        /// 获取协议配置信息
        /// </summary>
        /// <returns></returns>
        Task<List<ConfigureMessageContent>?> GetProtocolConfig();

        /// <summary>
        /// Excel导出
        /// </summary> 
        /// <returns></returns>
        Task<string> ExportExcel_TestCase();

        /// <summary>
        /// Word导出
        /// </summary> 
        /// <returns></returns>
        Task<string> ExportWord_TestCase();
    }
}
