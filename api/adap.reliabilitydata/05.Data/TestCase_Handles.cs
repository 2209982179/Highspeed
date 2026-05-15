using BKDataAnalysis;
using Castle.Core.Configuration;
using highspeed.business._00.Enum;
using highspeed.business._01.Models;
using highspeed.framework;
using highspeed.framework.Common;
using highspeed.framework.DB.MongoDB;
using IntegratedSystem.DAL;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using static highspeed.business._00.Enum.CommonEnum;

namespace adap.safetyandreliabilityapi._05.Data.Reliability_Prediction
{
    public class TestCase_Handles
    {
        private const byte ProtocolDirectionInput = 0x01;
        private const byte ProtocolDirectionOutput = 0x00;

        public static c_kernel32Helper kernel32Helper = new c_kernel32Helper(Environment.CurrentDirectory + "\\UserSettings\\UserSettings.ini");

        /// <summary>
        /// 获取测试用例列表
        /// </summary>
        /// <returns></returns>
        public static Task<TestCaseMixClass?> GetTestCaseList()
        {
            try
            {
                string sqlStr = $@"Select * From TestCase";
                DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr);
                List<TestCaseModel> testCaseModel = list.Select().ToObjectList<TestCaseModel>();
                List<TestCaseTree> testCaseTrees = new List<TestCaseTree>();
                int num = testCaseModel.Select(x=>x.TestCaseID).Min();
                if (testCaseModel != null)
                {
                    foreach (TestCaseModel test in testCaseModel)
                    { 
                        if (!testCaseTrees.Select(x => x.TestName).ToList().Contains(test.ExtendedField1))
                        {
                            TestCaseTree testCase = new TestCaseTree();
                            testCase.TestId = --num;
                            testCase.TestName = test.ExtendedField1;
                            testCase.TestType = "System";
                            TestCaseTree testCaseSec = new TestCaseTree();
                            testCaseSec.TestId = --num;
                            testCaseSec.TestName = test.ExtendedField2;
                            testCaseSec.TestType = "Test";
                            TestCaseTree testCaseThr = new TestCaseTree();
                            testCaseThr.TestId = test.TestCaseID;
                            testCaseThr.TestName = test.TestCaseName;
                            testCaseThr.TestType = "TestCase";
                            testCaseSec.child.Add(testCaseThr);
                            testCase.child.Add(testCaseSec);
                            testCaseTrees.Add(testCase);
                        }
                        else if (testCaseTrees.Select(x => x.TestName).ToList().Contains(test.ExtendedField1) && !testCaseTrees.Where(y => y.TestName == test.ExtendedField1).FirstOrDefault().child.Select(x => x.TestName).ToList().Contains(test.ExtendedField2))
                        {
                            TestCaseTree testCase = testCaseTrees.Where(y => y.TestName == test.ExtendedField1).FirstOrDefault();
                            TestCaseTree testCaseSec = new TestCaseTree();
                            testCaseSec.TestId = --num;
                            testCaseSec.TestName = test.ExtendedField2;
                            testCaseSec.TestType = "Test";
                            TestCaseTree testCaseThr = new TestCaseTree();
                            testCaseThr.TestId = test.TestCaseID;
                            testCaseThr.TestName = test.TestCaseName;
                            testCaseThr.TestType = "TestCase";
                            testCaseSec.child.Add(testCaseThr);
                            testCase.child.Add(testCaseSec);
                        }
                        else if (testCaseTrees.Select(x => x.TestName).ToList().Contains(test.ExtendedField1) && testCaseTrees.Where(y => y.TestName == test.ExtendedField1).FirstOrDefault().child.Select(x => x.TestName).ToList().Contains(test.ExtendedField2))
                        {
                            TestCaseTree testCase = testCaseTrees.Where(y => y.TestName == test.ExtendedField1).FirstOrDefault();
                            TestCaseTree testCaseSec = testCase.child.Where(y => y.TestName == test.ExtendedField2).FirstOrDefault();
                            TestCaseTree testCaseThr = new TestCaseTree();
                            testCaseThr.TestId = test.TestCaseID;
                            testCaseThr.TestName = test.TestCaseName;
                            testCaseThr.TestType = "TestCase";
                            testCaseSec.child.Add(testCaseThr);
                        }
                    }

                    testCaseModel = GetTestCasesDetails(testCaseModel);
                    return Task.FromResult<TestCaseMixClass?>(new TestCaseMixClass(testCaseTrees, testCaseModel));
                }
                else
                    return Task.FromResult<TestCaseMixClass?>(new TestCaseMixClass(null, null));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Task.FromResult<TestCaseMixClass?>(new TestCaseMixClass(null, null));
            }
        }

        /// <summary>
        /// 获取测试用例步骤详细数据
        /// </summary>
        /// <param name="testCases"></param>
        /// <returns></returns>
        public static List<TestCaseModel> GetTestCasesDetails(List<TestCaseModel> testCases)
        {
            try
            {
                string ParameterIDsqlStr = $@"Select * From CaseAndParameter ";
                DataTable ParameterIDlist = GlobalContext.Reposhell.DBHelper.QueryDataTable(ParameterIDsqlStr);
                List<CaseAndParameterIds> ParameterIDfcs = ParameterIDlist.Select().ToObjectList<CaseAndParameterIds>();

                string ParametersqlStr = $@"Select * From TestParameter";
                DataTable Parameterlist = GlobalContext.Reposhell.DBHelper.QueryDataTable(ParametersqlStr);
                List<TestParameterModel> Parameterfcs = Parameterlist.Select().ToObjectList<TestParameterModel>();

                string RecodeIDsqlStr = $@"Select * From CaseAndRecode";
                DataTable RecodeIDlist = GlobalContext.Reposhell.DBHelper.QueryDataTable(RecodeIDsqlStr);
                List<CaseAndRecodeIds> RecodeIDfcs = RecodeIDlist.Select().ToObjectList<CaseAndRecodeIds>();

                string RecodesqlStr = $@"Select * From TestRecode";
                DataTable Recodelist = GlobalContext.Reposhell.DBHelper.QueryDataTable(RecodesqlStr);
                List<TestRecodeModel> Recodefcs = Recodelist.Select().ToObjectList<TestRecodeModel>();

                string TrainsqlStr = $@"Select * From TestTrainInformation";
                DataTable Trainlist = GlobalContext.Reposhell.DBHelper.QueryDataTable(TrainsqlStr);
                List<TestTrainInformationModel> Trainfcs = Trainlist.Select().ToObjectList<TestTrainInformationModel>();

                return testCases;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return  new List<TestCaseModel>();
            }
        }


        /// <summary>
        /// 获取协议配置信息
        /// </summary>
        /// <param name="testCases"></param>
        /// <returns></returns>
        public static List<ConfigureMessageContent> GetProtocolConfig()
        {
            try
            {
                string ParameterIDsqlStr = $@"Select * From protocolconfig";
                DataTable ParameterIDlist = GlobalContext.Reposhell.DBHelper.QueryDataTable(ParameterIDsqlStr);
                //List<ConfigureMessageContent> ParameterIDfcs = ParameterIDlist.Select().ToObjectList<ConfigureMessageContent>();
                static byte SafeByte(object val)
                {
                    if (val == null || val == DBNull.Value) return 0;
                    string s = val.ToString().Trim();

                    // 支持 0x 前缀的十六进制字符串
                    if (s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    {
                        return Convert.ToByte(s, 16);
                    }

                    return byte.TryParse(s, out byte b) ? b : (byte)0;
                }

                List<ConfigureMessageContent> ParameterIDfcs = ParameterIDlist.Select()
                    .Select(row => new ConfigureMessageContent
                    {
                        Category = SafeByte(row["Category"]),
                        CategoryRemark = row["CategoryRemark"]?.ToString() ?? "",
                        Description = SafeByte(row["Description"]),
                        DescriptionRemark = row["DescriptionRemark"]?.ToString() ?? "",
                        Content = SafeByte(row["Content"]),
                        ContentRemark = row["ContentRemark"]?.ToString() ?? "",
                        Remark = row["Remark"]?.ToString() ?? ""
                    }).ToList();

                return ParameterIDfcs;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<ConfigureMessageContent>();
            }
        }

        /// <summary>
        /// 获取列车信息
        /// </summary>
        /// <param name="TrainName"></param>
        /// <returns></returns>
        public static TestTrainInformationModel? GetTrainDatas(String TrainName)
        {
            try
            {
                string sqlStr = $@"Select * From TestTrainInformation WHERE TrainName = @TrainName";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TrainName", TrainName }};
                DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr, parameters);
                List<TestTrainInformationModel> fcs = list.Select().ToObjectList<TestTrainInformationModel>();

                if (list.Rows.Count > 0)
                    return fcs[0];
                else
                    return new TestTrainInformationModel();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new TestTrainInformationModel();
            }
        }

        /// <summary>
        /// 更新列车基本信息
        /// </summary>
        /// <param name="TrainName"></param>
        /// <returns></returns>
        public static bool UpdateTrainDatas(string TrainName, string? FormationType ,string? VehicleLoad,string? MaximumOperatingSpeed,string? AverageTravelSpeed,string? AverageDwellTime,string? AverageAcceleration ,string? AverageDeceleration ,
            string? TrainImpactRate, string? RequirementsFaultStatus, string? RequirementsRampRescue ,string? RequirementsNoiseLevelIn ,string? RequirementsNoiseLevelOut)
        {
            try
            {
                string sqlStr = $@"UPDATE TestTrainInformation SET 
                                   FormationType =@FormationType,
                                   VehicleLoad =@VehicleLoad,
                                   MaximumOperatingSpeed =@MaximumOperatingSpeed,
                                   AverageTravelSpeed =@AverageTravelSpeed,
                                   AverageDwellTime =@AverageDwellTime,
                                   AverageAcceleration =@AverageAcceleration,
                                   AverageDeceleration =@AverageDeceleration,
                                   TrainImpactRate =@TrainImpactRate,
                                   RequirementsFaultStatus =@RequirementsFaultStatus,
                                   RequirementsRampRescue =@RequirementsRampRescue,
                                   RequirementsNoiseLevelIn =@RequirementsNoiseLevelIn,
                                   RequirementsNoiseLevelOut =@RequirementsNoiseLevelOut,
                                   WHERE TrainName =@TrainName";

                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TrainName", TrainName },
                    { "@FormationType", FormationType.ToString() },
                    { "@VehicleLoad", VehicleLoad.ToString() },
                    { "@MaximumOperatingSpeed", MaximumOperatingSpeed.ToString() },
                    { "@AverageTravelSpeed", AverageTravelSpeed.ToString() },
                    { "@AverageDwellTime", AverageDwellTime.ToString() },
                    { "@AverageAcceleration", AverageAcceleration.ToString() },
                    { "@AverageDeceleration", AverageDeceleration.ToString() },
                    { "@TrainImpactRate", TrainImpactRate.ToString() },
                    { "@RequirementsFaultStatus", RequirementsFaultStatus.ToString() },
                    { "@RequirementsRampRescue", RequirementsRampRescue.ToString() },
                    { "@RequirementsNoiseLevelIn", RequirementsNoiseLevelIn.ToString() },
                    { "@RequirementsNoiseLevelOut", RequirementsNoiseLevelOut.ToString() }};

                int handle = GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(sqlStr, parameters);

                if (handle == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取用例记录信号
        /// </summary>
        /// <returns></returns>
        public static List<TestRecodeModel>? GetTestRecodeDatas()
        {
            try
            {
                string RecodesqlStr = $@"Select * From TestRecode";
                DataTable Recodelist = GlobalContext.Reposhell.DBHelper.QueryDataTable(RecodesqlStr);
                List<TestRecodeModel> list = Recodelist.Select().ToObjectList<TestRecodeModel>();

                if (Recodelist.Rows.Count > 0)
                    return list;
                else
                    return new List<TestRecodeModel>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<TestRecodeModel>();
            }
        }

        /// <summary>
        /// 查询用例对应的监控信号
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        public static List<TestRecodeModel>? GetOneTestRecodeDatas(int TestCaseID, List<TestRecodeModel> testRecodes)
        {
            try {
                string RecodesqlStr = $@"Select * From CaseAndRecode WHERE TestCaseID = @TestCaseID";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID }};
                DataTable RecodeIDlist = GlobalContext.Reposhell.DBHelper.QueryDataTable(RecodesqlStr, parameters);
                List<CaseAndRecodeIds> RecodeIDfcs = RecodeIDlist.Select().ToObjectList<CaseAndRecodeIds>();

                List < TestRecodeModel >  NewTestRecodes = new List<TestRecodeModel>();
                foreach (int Id in RecodeIDfcs.Select(x => x.RecodeID))
                {
                    NewTestRecodes.Add(testRecodes.Find(x => x.RecodeID == Id));
                }

                if (RecodeIDfcs.Count > 0)
                    return NewTestRecodes;
                else
                    return new List<TestRecodeModel>();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return new List<TestRecodeModel>();
            }
        }

        /// <summary>
        /// 监控信号插入
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <param name="RecodeNames"></param>
        /// <param name="testRecodes"></param>
        /// <returns></returns>
        public static bool UpdateTestRecodeDatas(int TestCaseID, List<string> RecodeNames)
        {
            try
            {
                string RecodesqlStr1 = $@"Select * From TestRecode";
                DataTable Recodelist = GlobalContext.Reposhell.DBHelper.QueryDataTable(RecodesqlStr1);
                List<TestRecodeModel> list = Recodelist.Select().ToObjectList<TestRecodeModel>();

                string sqlStr = $@"DELETE FROM CaseAndRecode 
                                   WHERE TestCaseID =@TestCaseID";

                Dictionary<string, object> parametersdelete = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID } };

                int handledelete = GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(sqlStr, parametersdelete);

                int handle = 0;
                foreach (string testRecode in RecodeNames)
                {
                    string RecodesqlStr = $@"INSERT INTO CaseAndRecode(TestCaseID, RecodeID) VALUES (@TestCaseID, @RecodeID)";
                    Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID },
                    { "@RecodeID", list.Find(x=>x.RecodeName == testRecode).RecodeID }};

                    handle += GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(RecodesqlStr, parameters);
                }

                if (handle == RecodeNames.Count())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 创建测试用例
        /// </summary> 
        /// <param name="TestCaseGUID">用例GUID</param>
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        public static bool CreateTestCase(string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark)
        {
            try
            {
                string sqlStr = $@"INSERT INTO TestCase(TestCaseGUID,ExtendedField1,ExtendedField2,TestCaseName,TrainType,Remark) VALUES (@TestCaseGUID,@ExtendedField1,@ExtendedField2,@TestCaseName,@TrainType,@Remark)";
                string TestCaseGUID = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseGUID", TestCaseGUID },
                    { "@TestCaseName", string.IsNullOrWhiteSpace(TestCaseName)?"":TestCaseName },
                    { "@ExtendedField1", ExtendedField1.ToString() },
                    { "@ExtendedField2", ExtendedField2.ToString() },
                    { "@TrainType", TrainType.ToString() },
                    { "@Remark",  string.IsNullOrWhiteSpace(Remark)?"":Remark} };

                int handle = GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(sqlStr, parameters);

                if (handle == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 修改测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <param name="TestCaseGUID">用例GUID</param>
        /// <param name="TestCaseName">用例名称</param>
        /// <param name="TestCaseType">用例类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        public static bool UpdateTestCase(int TestCaseID, string? TestCaseName, string ExtendedField1, string ExtendedField2, TrainType TrainType, string? Remark)
        {
            try
            {
                string sqlStr = $@"UPDATE TestCase SET 
                                   TestCaseName =@TestCaseName,
                                   ExtendedField1 =@ExtendedField1,
                                   ExtendedField2 =@ExtendedField2,
                                   TrainType =@TrainType,
                                   Remark =@Remark 
                                   WHERE TestCaseID =@TestCaseID";

                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID },
                    { "@TestCaseName", string.IsNullOrWhiteSpace(TestCaseName)?"":TestCaseName },
                    { "@ExtendedField1", ExtendedField1.ToString() },
                    { "@ExtendedField2", ExtendedField2.ToString() },
                    { "@TrainType", TrainType.ToString() },
                    { "@Remark",  string.IsNullOrWhiteSpace(Remark)?"":Remark} };

                int handle = GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(sqlStr, parameters);

                if (handle == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 修改测试用例前置条件
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <param name="ExtendedField7"></param>
        /// <param name="ExtendedField8"></param>
        /// <returns></returns>
        public static bool UpdatePreconditions(int TestCaseID, string ExtendedField7, string ExtendedField8)
        {
            try
            {
                string sqlStr = $@"UPDATE TestCase SET 
                                   ExtendedField7 =@ExtendedField7,
                                   ExtendedField8 =@ExtendedField8
                                   WHERE TestCaseID =@TestCaseID";

                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID },
                    { "@ExtendedField7", ExtendedField7.ToString() },
                    { "@ExtendedField8", ExtendedField8.ToString() } };

                int handle = GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(sqlStr, parameters);

                if (handle == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 删除测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <returns></returns>
        public static bool DeleteTestCase(int TestCaseID)
        {
            try
            {
                string sqlStr = $@"DELETE FROM TestCase 
                                   WHERE TestCaseID =@TestCaseID";

                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID } };

                int handle = GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(sqlStr, parameters);

                if (handle == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 获取单个测试用例所有数据
        /// </summary>
        /// <param name="TestCaseID"></param>
        /// <returns></returns>
        public static TestCaseModel GetOneTestCaseDetail(int TestCaseID)
        {
            try
            {
                TestCaseModel? testCase = GetTestCaseById(TestCaseID);
                if (testCase == null)
                {
                    return new TestCaseModel();
                }

                //获取用例步骤
                testCase.TestCommands = GetExecutableTestSteps(TestCaseID);
                if (testCase.TestCommands.Count == 0)
                {
                    return new TestCaseModel();
                }

                //获取列车信息
                testCase.TestTrainInformation = GetTrainDatas(testCase.TrainType.ToString());

                //获取对应记录信号
                List<TestRecodeModel> testRecodes = GetTestRecodeDatas();
                testCase.TestRecodes = GetOneTestRecodeDatas(TestCaseID, testRecodes);

                //前置环境  待添加



                return testCase;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        /// <summary>
        /// 执行测试用例
        /// </summary>
        /// <param name="TestCaseID">用例ID</param>
        /// <returns></returns>
        public static bool ExecuteTestCase(int TestCaseID)
        {
            try
            {
                TestCaseModel? testCase = GetTestCaseById(TestCaseID);
                if (testCase == null)
                {
                    return false;
                }

                List<TestStepModel> testSteps = GetExecutableTestSteps(TestCaseID);
                if (testSteps.Count == 0)
                {
                    return false;
                }

                testCase.TestTrainInformation = GetTrainDatas(testCase.TrainType.ToString());


                c_UDPHelper udpHelper = new c_UDPHelper();
                string errorMessage = string.Empty;
                if (!udpHelper.StartConnect(ref errorMessage))
                {
                    Logger.Error(new Exception("测试用例执行失败，UDP初始化失败：" + errorMessage));
                    return false;
                }

                try
                {
                    List<byte[]> Configbytes = new List<byte[]>();
                    Configbytes = c_MessageConversion.BuildConfigMessageByteList(TestCaseID);
                    ProtocolConfig protocol = new ProtocolConfig();

                    string IP = kernel32Helper.GetIniString("NetSettings", "IPAddress", "");
                    string ob1 = "";
                    string Local_IP = "";
                    string Local_Port1 = "";
                    string Local_Port2 = "";
                    string Ser_IP = "";
                    string Ser_Port1 = "";
                    string Ser_Port2 = "";
                    if (IP.Split(new char[] { ',' }).Count() > 0)
                    {
                        Local_IP = IP.Split(new char[] { ',' })[0];//IP
                        Local_Port1 = IP.Split(new char[] { ',' })[1];//发送端口
                        Local_Port2 = IP.Split(new char[] { ',' })[2];//接收端口
                        Ser_IP = IP.Split(new char[] { ',' })[3];//IP
                        Ser_Port1 = IP.Split(new char[] { ',' })[4];//服务器发送端口
                        Ser_Port2 = IP.Split(new char[] { ',' })[5];//服务器接收端口
                    }

                    protocol.SenderPortID = new char[] { Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[3])), Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[2])), Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[1])), Convert.ToChar(Convert.ToInt32(Local_IP.Split(".".ToArray()[0])[0])) };
                    protocol.SenderPortNumber = ushort.Parse(Local_Port1);
                    protocol.ReceiverPortID = new char[] { Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[3])), Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[2])), Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[1])), Convert.ToChar(Convert.ToInt32(Ser_IP.Split(".".ToArray()[0])[0])) };
                    protocol.ReceiverPortNumber = ushort.Parse(Ser_Port2);
                    protocol.MessageSequenceNumber = 0x00001;
                    protocol.MessageRecipient = 0x00001;

                    TimeSpan toNow = DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    protocol.Time = Convert.ToInt32(toNow.TotalSeconds);
                    protocol.Milliseconds = Convert.ToUInt16(toNow.Milliseconds);

                    List<byte[]> MessageresArr =  c_MessageConversion.CreateHeadMessage(protocol, Configbytes);

                    /*string? ipInfo = ResolveTargetEndpoint(udpHelper, testCase);
                    if (string.IsNullOrWhiteSpace(ipInfo))
                    {
                        Logger.Error(new Exception("测试用例执行失败，未找到匹配的UDP目标配置。"));
                        return false;
                    }

                    List<c_UDPHelper.ProtocolInstruction> protocolInstructions = new List<c_UDPHelper.ProtocolInstruction>();
                    foreach (TestStepModel step in testSteps)
                    {
                        if (!TryBuildProtocolInstruction(step.TestStepName, step.StepValue?.ToString(), step.sendReceiveType, out c_UDPHelper.ProtocolInstruction? instruction, out string instructionError))
                        {
                            Logger.Error(new Exception($"测试步骤[{step.TestStepID}]报文组装失败：{instructionError}"));
                            return false;
                        }
                    }

                    AppendPreconditionInstructions(testCase, protocolInstructions);
                    if (protocolInstructions.Count == 0)
                    {
                        Logger.Error(new Exception("测试用例执行失败，没有可发送的协议指令。"));
                        return false;
                    }

                    string sendError = string.Empty;
                    if (!udpHelper.SendControlProtocolDatas(ipInfo, protocolInstructions, ref sendError))
                    {
                        Logger.Error(new Exception($"测试用例[{TestCaseID}]发送失败：{sendError}"));
                        return false;
                    }*/

                    return true;
                }
                finally
                {
                    udpHelper.StopConnect();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }


        /// <summary>
        /// 查询ID获取单个测试用例
        /// </summary>
        /// <param name="testCaseID"></param>
        /// <returns></returns>
        public static TestCaseModel? GetTestCaseById(int testCaseID)
        {
            string sqlStr = @"Select * From TestCase WHERE TestCaseID = @TestCaseID";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@TestCaseID", testCaseID }
            };

            DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr, parameters);
            List<TestCaseModel> testCases = list.Select().ToObjectList<TestCaseModel>();
            return testCases.FirstOrDefault();
        }

        /// <summary>
        /// 获取单个测试用例测试步骤
        /// </summary>
        /// <param name="testCaseID"></param>
        /// <returns></returns>
        public static List<TestStepModel> GetExecutableTestSteps(int testCaseID)
        {
            string sqlStr = @"Select * From TestStep WHERE TestCaseID = @TestCaseID ORDER BY ParentID, PreviousID, TestStepID";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@TestCaseID", testCaseID }
            };

            DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr, parameters);
            List<TestStepModel> testSteps = list.Select().ToObjectList<TestStepModel>();
            if (testSteps.Count == 0)
            {
                return new List<TestStepModel>();
            }

            Dictionary<int, List<TestStepModel>> childrenMap = testSteps
                .GroupBy(x => x.ParentID)
                .ToDictionary(x => x.Key, x => x.OrderBy(y => y.PreviousID).ThenBy(y => y.TestStepID).ToList());

            Dictionary<int, TestStepModel> stepMap = testSteps.ToDictionary(x => x.TestStepID, x => x);
            List<TestStepModel> executableSteps = new List<TestStepModel>();
            ExpandExecutableSteps(childrenMap, stepMap, 0, executableSteps);

            return executableSteps
                .Where(x => x.TestStepType == StepType.Execute && x.sendReceiveType == SendReceiveType.Send)
                .ToList();
        }

        private static void ExpandExecutableSteps(Dictionary<int, List<TestStepModel>> childrenMap, Dictionary<int, TestStepModel> stepMap, int parentId, List<TestStepModel> result)
        {
            if (!childrenMap.TryGetValue(parentId, out List<TestStepModel>? children))
            {
                return;
            }

            foreach (TestStepModel step in children)
            {
                switch (step.TestStepType)
                {
                    case StepType.If:
                        if (EvaluateIfStep(step, stepMap))
                        {
                            ExpandBranchChildren(childrenMap, stepMap, step.TestStepID, "then", result);
                        }
                        break;
                    case StepType.For:
                        int loopCount = step.LoopCount > 0 ? step.LoopCount : 1;
                        for (int i = 0; i < loopCount; i++)
                        {
                            ExpandBranchChildren(childrenMap, stepMap, step.TestStepID, "body", result);
                        }
                        break;
                    default:
                        result.Add(step);
                        ExpandBranchChildren(childrenMap, stepMap, step.TestStepID, null, result);
                        break;
                }
            }
        }

        private static void ExpandBranchChildren(Dictionary<int, List<TestStepModel>> childrenMap, Dictionary<int, TestStepModel> stepMap, int parentStepId, string? branchType, List<TestStepModel> result)
        {
            if (!childrenMap.TryGetValue(parentStepId, out List<TestStepModel>? children))
            {
                return;
            }

            IEnumerable<TestStepModel> branchChildren = string.IsNullOrWhiteSpace(branchType)
                ? children.Where(x => string.IsNullOrWhiteSpace(x.BranchType))
                : children.Where(x => string.Equals(x.BranchType, branchType, StringComparison.OrdinalIgnoreCase));

            foreach (TestStepModel child in branchChildren.OrderBy(x => x.PreviousID).ThenBy(x => x.TestStepID).ToList())
            {
                switch (child.TestStepType)
                {
                    case StepType.If:
                        if (EvaluateIfStep(child, stepMap))
                        {
                            ExpandBranchChildren(childrenMap, stepMap, child.TestStepID, "then", result);
                        }
                        break;
                    case StepType.For:
                        int loopCount = child.LoopCount > 0 ? child.LoopCount : 1;
                        for (int i = 0; i < loopCount; i++)
                        {
                            ExpandBranchChildren(childrenMap, stepMap, child.TestStepID, "body", result);
                        }
                        break;
                    default:
                        result.Add(child);
                        ExpandBranchChildren(childrenMap, stepMap, child.TestStepID, null, result);
                        break;
                }
            }
        }

        private static bool EvaluateIfStep(TestStepModel step, Dictionary<int, TestStepModel> stepMap)
        {
            if (!step.BindTestStepID.HasValue || !stepMap.TryGetValue(step.BindTestStepID.Value, out TestStepModel? bindStep))
            {
                return false;
            }

            if (bindStep.TestStepType != StepType.Execute)
            {
                return false;
            }

            string actualValue = bindStep.StepValue?.ToString() ?? string.Empty;
            return EvaluateCondition(actualValue, step.IfOperator, step.IfExpectedValue);
        }

        private static bool EvaluateCondition(string actualValue, string? conditionOperator, string? expectedValue)
        {
            if (string.IsNullOrWhiteSpace(conditionOperator) || string.IsNullOrWhiteSpace(expectedValue))
            {
                return false;
            }
            string currentValue = actualValue.Trim().Trim('\'', '"');
            string expected = expectedValue.Trim().Trim('\'', '"');

            if (double.TryParse(currentValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double currentNumber) &&
                double.TryParse(expected, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double expectedNumber))
            {
                return conditionOperator switch
                {
                    ">" => currentNumber > expectedNumber,
                    "<" => currentNumber < expectedNumber,
                    ">=" => currentNumber >= expectedNumber,
                    "<=" => currentNumber <= expectedNumber,
                    "==" => currentNumber == expectedNumber,
                    _ => false
                };
            }

            int compareResult = string.Compare(currentValue, expected, StringComparison.OrdinalIgnoreCase);
            return conditionOperator switch
            {
                "==" => compareResult == 0,
                ">" => compareResult > 0,
                "<" => compareResult < 0,
                ">=" => compareResult >= 0,
                "<=" => compareResult <= 0,
                _ => false
            };
        }


        public static byte ParsePercentEnum(string? stepValue)
        {
            if (string.IsNullOrWhiteSpace(stepValue))
            {
                return 0x01;
            }

            string normalized = stepValue.Trim().Replace("%", "");
            if (byte.TryParse(normalized, out byte percent))
            {
                int index = Math.Clamp(percent / 10, 0, 10);
                return (byte)(index + 0x01);
            }

            return normalized switch
            {
                "0x01" => 0x01,
                "0x02" => 0x02,
                "0x03" => 0x03,
                "0x04" => 0x04,
                "0x05" => 0x05,
                "0x06" => 0x06,
                "0x07" => 0x07,
                "0x08" => 0x08,
                "0x09" => 0x09,
                "0x0A" => 0x0A,
                "0x0B" => 0x0B,
                _ => 0x01
            };
        }

        public static double ParseDouble(string? stepValue)
        {
            if (string.IsNullOrWhiteSpace(stepValue))
            {
                return 0D;
            }

            if (double.TryParse(stepValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double result) ||
                double.TryParse(stepValue, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.CurrentCulture, out result))
            {
                return result;
            }

            return 0D;
        }

        public sealed class ProtocolInstructionDefinition
        {
            public ProtocolInstructionDefinition(byte category, byte item, ProtocolContentType contentType, byte? fixedValue = null)
            {
                Category = category;
                Item = item;
                ContentType = contentType;
                FixedValue = fixedValue;
            }

            public byte Category { get; }

            public byte Item { get; }

            public ProtocolContentType ContentType { get; }

            public byte? FixedValue { get; }
        }

        public enum ProtocolContentType
        {
            Empty,
            FixedEnum,
            PercentEnum,
            Double,
            Raw
        }


        /// <summary>
        /// MongoDB测试
        /// </summary>
        /// <returns></returns>
        public static bool DoMongoDB()
        {
            try
            {

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }
    }
}
