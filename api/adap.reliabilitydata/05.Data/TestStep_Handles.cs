using Castle.Core.Configuration;
using highspeed.business._01.Models;
using highspeed.framework;
using highspeed.framework.Common;
using highspeed.framework.DB.MongoDB;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using static highspeed.business._00.Enum.CommonEnum;

namespace adap.safetyandreliabilityapi._05.Data.Reliability_Prediction
{
    public class TestStep_Handles
    {
        /// <summary>
        /// 获取测试步骤列表
        /// </summary>
        /// <returns></returns>
        public static Task<List<TestStepModel>?> GetTestStepDatas()
        {
            try
            {
                string sqlStr = $@"Select * From TestCommand";
                DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr);
                List<TestStepModel> fcs = list.Select().ToObjectList<TestStepModel>();

                if (fcs != null)
                {
                    return Task.FromResult<List<TestStepModel>?>(fcs);
                }
                else
                {
                    return Task.FromResult<List<TestStepModel>?>(new List<TestStepModel>());
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Task.FromResult<List<TestStepModel>?>(new List<TestStepModel>());
            }
        }

        /// <summary>
        /// 根据id查找测试步骤列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Task<List<TestStepModel>?> GetTestStepDatas(int TestCaseID)
        {
            try
            {
                string sqlStr = $@"Select * From TestStep WHERE TestCaseID = @TestCaseID";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID }};
                DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr, parameters);
                List<TestStepModel> fcs = list.Select().ToObjectList<TestStepModel>();

                return Task.FromResult<List<TestStepModel>?>(fcs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Task.FromResult<List<TestStepModel>?>(new List<TestStepModel>());
            }
        }

        /// <summary>
        /// 创建测试步骤
        /// </summary> 
        /// <param name="TestCaseGUID">步骤GUID</param>
        /// <param name="TestCaseName">步骤名称</param>
        /// <param name="TestCaseType">步骤类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        public static bool CreateTestStep(int TestCaseID, int ParentID, int PreviousID, string? TestStepName, string? StepValue, StepType TestStepType, SendReceiveType sendReceiveType, string? Remark, string? BranchType, int? BindTestStepID, string? IfOperator, string? IfExpectedValue, int? LoopCount)
        {
            try
            {
                if (!ValidateControlStepBinding(TestCaseID, TestStepType, BindTestStepID))
                {
                    return false;
                }

                // ExtendedField1: BranchType, ExtendedField2: BindTestStepID,
                // ExtendedField3: IfOperator, ExtendedField4: IfExpectedValue, ExtendedField5: LoopCount
                string sqlStr = @"INSERT INTO TestStep
                                  (TestCaseID, ParentID, PreviousID, TestStepName, StepValue, TestStepType, sendReceiveType, Remark, ExtendedField1, ExtendedField2, ExtendedField3, ExtendedField4, ExtendedField5)
                                  VALUES
                                  (@TestCaseID, @ParentID, @PreviousID, @TestStepName, @StepValue, @TestStepType, @sendReceiveType, @Remark, @ExtendedField1, @ExtendedField2, @ExtendedField3, @ExtendedField4, @ExtendedField5)";
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID },
                    { "@ParentID", ParentID },
                    { "@PreviousID", PreviousID },
                    { "@TestStepName", NormalizeStepName(TestStepName, TestStepType) },
                    { "@StepValue",  NormalizeStepValue(StepValue, TestStepType)},
                    { "@TestStepType", TestStepType.ToString() },
                    { "@sendReceiveType", sendReceiveType.ToString() },
                    { "@Remark",  string.IsNullOrWhiteSpace(Remark)?"":Remark},
                    { "@ExtendedField1", string.IsNullOrWhiteSpace(BranchType) ? "" : BranchType },
                    { "@ExtendedField2", BindTestStepID.HasValue ? BindTestStepID.Value.ToString() : "" },
                    { "@ExtendedField3", string.IsNullOrWhiteSpace(IfOperator) ? "" : IfOperator },
                    { "@ExtendedField4", string.IsNullOrWhiteSpace(IfExpectedValue) ? "" : IfExpectedValue },
                    { "@ExtendedField5", LoopCount.HasValue && LoopCount.Value > 0 ? LoopCount.Value.ToString() : "1" } };

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
        /// 修改测试步骤
        /// </summary>
        /// <param name="TestCaseID">步骤ID</param>
        /// <param name="TestCaseGUID">步骤GUID</param>
        /// <param name="TestCaseName">步骤名称</param>
        /// <param name="TestCaseType">步骤类型</param>
        /// <param name="Remark">备注</param>
        /// <returns></returns>
        public static bool UpdateTestStep(int TestCaseID, int TestStepID, string? TestStepName, string? StepValue, StepType TestStepType, SendReceiveType sendReceiveType, string? Remark, int ParentID, int PreviousID, string? BranchType, int? BindTestStepID, string? IfOperator, string? IfExpectedValue, int? LoopCount)
        {
            try
            {
                if (!ValidateControlStepBinding(TestCaseID, TestStepType, BindTestStepID))
                {
                    return false;
                }

                string sqlStr = @"UPDATE TestStep SET
                                   TestCaseID = @TestCaseID,
                                   ParentID = @ParentID,
                                   PreviousID = @PreviousID,
                                   TestStepName = @TestStepName,
                                   StepValue = @StepValue,
                                   TestStepType = @TestStepType,
                                   sendReceiveType = @sendReceiveType,
                                   Remark = @Remark,
                                   ExtendedField1 = @ExtendedField1,
                                   ExtendedField2 = @ExtendedField2,
                                   ExtendedField3 = @ExtendedField3,
                                   ExtendedField4 = @ExtendedField4,
                                   ExtendedField5 = @ExtendedField5
                                   WHERE TestStepID =@TestStepID";

                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@TestCaseID", TestCaseID },
                    { "@ParentID", ParentID },
                    { "@PreviousID", PreviousID },
                    { "@TestStepID", TestStepID },
                    { "@TestStepName", NormalizeStepName(TestStepName, TestStepType) },
                    { "@StepValue", NormalizeStepValue(StepValue, TestStepType) },
                    { "@TestStepType", TestStepType.ToString() },
                    { "@sendReceiveType", sendReceiveType.ToString() },
                    { "@Remark",  string.IsNullOrWhiteSpace(Remark)?"":Remark},
                    { "@ExtendedField1", string.IsNullOrWhiteSpace(BranchType) ? "" : BranchType },
                    { "@ExtendedField2", BindTestStepID.HasValue ? BindTestStepID.Value.ToString() : "" },
                    { "@ExtendedField3", string.IsNullOrWhiteSpace(IfOperator) ? "" : IfOperator },
                    { "@ExtendedField4", string.IsNullOrWhiteSpace(IfExpectedValue) ? "" : IfExpectedValue },
                    { "@ExtendedField5", LoopCount.HasValue && LoopCount.Value > 0 ? LoopCount.Value.ToString() : "1" } };

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
        /// 删除测试步骤
        /// </summary>
        /// <param name="TestCaseID">步骤ID</param>
        /// <returns></returns>
        public static bool DeleteTestStep(int TestStepID)
        {
            try
            {
                List<int> deleteIds = GetDeleteStepIds(TestStepID);
                if (deleteIds.Count == 0)
                {
                    deleteIds.Add(TestStepID);
                }

                string sqlStr = $@"DELETE FROM TestStep WHERE TestStepID IN ({string.Join(",", deleteIds)})";

                int handle = GlobalContext.Reposhell.DBHelper.ExecuteNonQuery(sqlStr);

                if (handle > 0)
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

        private static string NormalizeStepName(string? testStepName, StepType stepType)
        {
            if (stepType == StepType.If)
            {
                return "IF";
            }

            if (stepType == StepType.For)
            {
                return "FOR";
            }

            return string.IsNullOrWhiteSpace(testStepName) ? "" : testStepName.Replace("信号", "");
        }

        private static string NormalizeStepValue(string? stepValue, StepType stepType)
        {
            if (stepType == StepType.If)
            {
                return "";
            }

            if (stepType == StepType.For)
            {
                return "Loop";
            }

            return string.IsNullOrWhiteSpace(stepValue) ? "" : stepValue;
        }

        private static List<int> GetDeleteStepIds(int rootStepId)
        {
            string sqlStr = @"Select TestStepID, ParentID From TestStep";
            DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr);
            List<TestStepModel> allSteps = list.Select().ToObjectList<TestStepModel>();

            List<int> result = new List<int>();
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(rootStepId);

            while (queue.Count > 0)
            {
                int currentId = queue.Dequeue();
                if (result.Contains(currentId))
                {
                    continue;
                }

                result.Add(currentId);
                foreach (TestStepModel child in allSteps.Where(x => x.ParentID == currentId))
                {
                    queue.Enqueue(child.TestStepID);
                }
            }

            return result;
        }

        private static bool ValidateControlStepBinding(int testCaseId, StepType stepType, int? bindTestStepId)
        {
            if (stepType != StepType.If)
            {
                return true;
            }

            if (!bindTestStepId.HasValue || bindTestStepId.Value <= 0)
            {
                return false;
            }

            string sqlStr = @"Select * From TestStep WHERE TestCaseID = @TestCaseID AND TestStepID = @TestStepID";
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@TestCaseID", testCaseId },
                { "@TestStepID", bindTestStepId.Value }
            };

            DataTable list = GlobalContext.Reposhell.DBHelper.QueryDataTable(sqlStr, parameters);
            List<TestStepModel> steps = list.Select().ToObjectList<TestStepModel>();
            TestStepModel? bindStep = steps.FirstOrDefault();
            return bindStep != null && bindStep.TestStepType == StepType.Execute;
        }
    }
}
