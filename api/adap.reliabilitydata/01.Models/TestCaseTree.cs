using Numerics.NET.Statistics.Distributions;
using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._01.Models
{
    public class TestCaseTree
    {
        /// <summary>
        /// id
        /// </summary>
        public int? TestId = -1;

        /// <summary>
        /// 试验名称
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string TestType { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<TestCaseTree> child = new List<TestCaseTree>();
    }

    public class TestCaseMixClass
    {
        public List<TestCaseTree> testCaseTree { get; set; }

        public List<TestCaseModel> testCaseModel { get; set; }

        public TestCaseMixClass(List<TestCaseTree>? testCaseTree1, List<TestCaseModel>? testCaseModel1) {
            testCaseTree = testCaseTree1;
            testCaseModel = testCaseModel1;
        }

    }



    public class CaseAndParameterIds
    {
        public int TestCaseID { get; set; }

        public int ParameterID { get; set; }
    }

    public class CaseAndRecodeIds
    {
        public int TestCaseID { get; set; }

        public int RecodeID { get; set; }
    }

    public class CaseAndTrainIds
    {
        public int TestCaseID { get; set; }

        public int TrainID { get; set; }
    }
}
