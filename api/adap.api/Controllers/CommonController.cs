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
    public class CommonController : ControllerBase
    {
        /// <summary>
        /// 测试用例API
        /// </summary>
        public TestCase_APIImpl API;
        /// <summary>
        /// 接口实例化
        /// </summary>
        public CommonController()
        {
            ProxyGenerator generator = new ProxyGenerator();//实例化【代理类生成器】  
            API = generator.CreateClassProxy<TestCase_APIImpl>();
        }

        /// <summary>
        /// 获取测试用例
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<TestCaseMixClass?> GetTestCaseList()
        {
            return await API.GetTestCaseList();
        }

        /// <summary>
        /// 获取记录信号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<List<TestRecodeModel>?> GetTestRecodeDatas()
        {
            return await API.GetTestRecodeDatas();
        }

        /// <summary>
        /// 更新列车信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<bool> UpdateTrainDatas(string TrainName, string? FormationType, string? VehicleLoad, string? MaximumOperatingSpeed, string? AverageTravelSpeed, string? AverageDwellTime, string? AverageAcceleration, string? AverageDeceleration,
            string? TrainImpactRate, string? RequirementsFaultStatus, string? RequirementsRampRescue, string? RequirementsNoiseLevelIn, string? RequirementsNoiseLevelOut)
        {
            return await API.UpdateTrainDatas(TrainName, FormationType, VehicleLoad, MaximumOperatingSpeed, AverageTravelSpeed, AverageDwellTime, AverageAcceleration, AverageDeceleration,
                        TrainImpactRate, RequirementsFaultStatus, RequirementsRampRescue, RequirementsNoiseLevelIn, RequirementsNoiseLevelOut);
        }

        /// <summary>
        /// 更新记录信号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<bool> UpdateTestRecodeDatas(int TestCaseID, List<string> RecodeNames)
        {
            return await API.UpdateTestRecodeDatas(TestCaseID, RecodeNames);
        }

        /// <summary>
        /// 查询用例对应的监控信号
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<List<TestRecodeModel>?> GetOneTestRecodeDatas(int TestCaseID, List<TestRecodeModel> testRecodes)
        {
            return await API.GetOneTestRecodeDatas(TestCaseID, testRecodes);
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
        public async Task<bool> AddNewTestCase(string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark)
        {
            return await API.AddNewTestCase(TestCaseName, ExtendedField1, ExtendedField2, TrainType, Remark);
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
        public async Task<bool> UpdateTestCase(int TestCaseID, string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark)
        {
            return await API.UpdateTestCase(TestCaseID, TestCaseName, ExtendedField1, ExtendedField2, TrainType, Remark);
        }

        /// <summary>
        /// 修改测试用例前置条件
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <param name="ExtendedField7"></param>
        /// <param name="ExtendedField8"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        [Route("[Action]")]
        public async Task<bool> UpdatePreconditions(int TestCaseID, string ExtendedField7, string ExtendedField8)
        {
            return await API.UpdatePreconditions(TestCaseID, ExtendedField7, ExtendedField8);
        }

        /// <summary>
        /// 删除测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param> 
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<bool> DeleteTestCase(int TestCaseID)
        {
            return await API.DeleteTestCase(TestCaseID);
        }

        /// <summary>
        /// 执行测试用例
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public virtual async Task<bool> ExecuteTestCase(int TestCaseID)
        {
            return await API.ExecuteTestCase(TestCaseID);
        }

        /// <summary>
        /// 获取协议配置信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<List<ConfigureMessageContent>?> GetProtocolConfig()
        {
            return await API.GetProtocolConfig();
        }

        /// <summary>
        /// Excel导出
        /// </summary> 
        /// <returns></returns> 
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult?> ExportExcel_TestCase()
        {
            string url = await API.ExportExcel_TestCase();

            if (!System.IO.File.Exists(url))
            {
                return NotFound("文件生成失败！");
            }

            var fileStream = new FileStream(url, FileMode.Open, FileAccess.Read);
            return File(fileStream, Consts.HeaderKey_ContentType_Stream, new FileInfo(url).Name);
        }

        /// <summary>
        /// Word导出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[Action]")]
        public async Task<IActionResult?> ExportWord_TestCase()
        {
            string url = await API.ExportWord_TestCase();

            if (!System.IO.File.Exists(url))
            {
                return NotFound("文件生成失败！");
            }

            var fileStream = new FileStream(url, FileMode.Open, FileAccess.Read);
            return File(fileStream, Consts.HeaderKey_ContentType_Stream, new FileInfo(url).Name);
        }
    }
}
