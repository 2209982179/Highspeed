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
    public class TestCase_APIImpl : TestCase_API
    {
        /// <summary>
        /// 获取用例数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TestCaseMixClass?> GetTestCaseList()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return TestCase_Handles.GetTestCaseList();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 获取列车信息
        /// </summary>
        /// <param name="TrainName"></param>
        /// <returns></returns>
        public async Task<TestTrainInformationModel?> GetTrainDatas(String TrainName)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return TestCase_Handles.GetTrainDatas(TrainName);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 获取协议配置信息
        /// </summary>
        /// <returns></returns>
        public async Task<List<ConfigureMessageContent>?> GetProtocolConfig()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return TestCase_Handles.GetProtocolConfig();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }


        /// <summary>
        /// 更改列车信息
        /// </summary>
        /// <param name="TrainName"></param>
        /// <returns></returns>
        public async Task<bool> UpdateTrainDatas(string TrainName, string? FormationType, string? VehicleLoad, string? MaximumOperatingSpeed, string? AverageTravelSpeed, string? AverageDwellTime, string? AverageAcceleration, string? AverageDeceleration,
            string? TrainImpactRate, string? RequirementsFaultStatus, string? RequirementsRampRescue, string? RequirementsNoiseLevelIn, string? RequirementsNoiseLevelOut)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return TestCase_Handles.UpdateTrainDatas(TrainName, FormationType, VehicleLoad, MaximumOperatingSpeed, AverageTravelSpeed, AverageDwellTime, AverageAcceleration, AverageDeceleration,
                        TrainImpactRate, RequirementsFaultStatus, RequirementsRampRescue, RequirementsNoiseLevelIn, RequirementsNoiseLevelOut);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 获取监控信号数据
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<TestRecodeModel>?> GetTestRecodeDatas()
        {
            return await Task.Run(() =>
            {
                try
                {
                    return TestCase_Handles.GetTestRecodeDatas();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 查询用例对应的监控信号
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        public async Task<List<TestRecodeModel>?> GetOneTestRecodeDatas(int TestCaseID, List<TestRecodeModel> testRecodes)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return TestCase_Handles.GetOneTestRecodeDatas(TestCaseID, testRecodes);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 更新监控信号数据
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <param name="RecodeNames"></param>
        /// <param name="testRecodes"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> UpdateTestRecodeDatas(int TestCaseID, List<string> RecodeNames)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return TestCase_Handles.UpdateTestRecodeDatas(TestCaseID, RecodeNames);
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
        public virtual async Task<bool> AddNewTestCase(string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(TestCaseName))
                    {
                        return false;
                    }

                    return TestCase_Handles.CreateTestCase(TestCaseName, ExtendedField1, ExtendedField2, TrainType, Remark);
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
        public virtual async Task<bool> UpdateTestCase(int TestCaseID, string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(TestCaseName))
                    {
                        return false;
                    }

                    return TestCase_Handles.UpdateTestCase(TestCaseID, TestCaseName, ExtendedField1, ExtendedField2, TrainType, Remark);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 修改测试用例前置条件
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <param name="ExtendedField7"></param>
        /// <param name="ExtendedField8"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public virtual async Task<bool> UpdatePreconditions(int TestCaseID, string ExtendedField7, string ExtendedField8)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (TestCaseID == -1)
                    {
                        return false;
                    }

                    return TestCase_Handles.UpdatePreconditions(TestCaseID, ExtendedField7, ExtendedField8);
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
        public virtual async Task<bool> DeleteTestCase(int TestCaseID)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (TestCaseID < 0)
                    {
                        return false;
                    }

                    return TestCase_Handles.DeleteTestCase(TestCaseID);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// 执行用例
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExecuteTestCase(int TestCaseID)
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (TestCaseID < 0)
                    {
                        return false;
                    }

                    return TestCase_Handles.ExecuteTestCase(TestCaseID);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// Excel导出
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> ExportExcel_TestCase()
        {
            return await Task.Run(() =>
            {
                try
                {
                    string? workpath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

                    String guid = Guid.NewGuid().ToString();
                    if (!Directory.Exists(workpath + "ReportTemp\\" + guid))
                        Directory.CreateDirectory(workpath + "ReportTemp\\" + guid);
                    string url = "ReportTemp\\" + guid + "\\TestCase_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx";

                    string NewFile = workpath + url;
                    ReportHelp_TestCase reportHelp_Prediction = new ReportHelp_TestCase();
                    Document doc = reportHelp_Prediction.SaveWord_WithModel(NewFile, "");

                    return NewFile;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    throw new Exception(ex.Message);
                }
            });
        }

        /// <summary>
        /// Word报告导出
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> ExportWord_TestCase()
        {
            return await Task.Run(() =>
            {
                try
                {
                    string? workpath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

                    String guid = Guid.NewGuid().ToString();
                    if (!Directory.Exists(workpath + "ReportTemp\\" + guid))
                        Directory.CreateDirectory(workpath + "ReportTemp\\" + guid);
                    string url = "ReportTemp\\" + guid + "\\TestCase_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".docx";

                    string NewFile = workpath + url;
                    ReportHelp_TestCase reportHelp_Prediction = new ReportHelp_TestCase();
                    Document doc = reportHelp_Prediction.SaveWord_WithModel(NewFile, "");

                    return NewFile;
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