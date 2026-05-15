using Aspose.Words;
using DnsClient;
using System.Diagnostics;
using static highspeed.business._00.Enum.CommonEnum;

namespace highspeed.business._01.Models
{
    /// <summary>
    /// 列车信息数据结构
    /// </summary> 
    public class TestTrainInformationModel
    {
        /// <summary>
        /// 列车ID
        /// </summary> 
        public int TrainID { get; set; } = -1;

        /// <summary>
        /// 列车名称
        /// </summary> 
        public string? TrainName { get; set; }

        /// <summary>
        /// 列车类型
        /// </summary>  
        public TrainType TrainType { get; set; }

        /// <summary>
        /// 编组型式
        /// </summary>
        public string? FormationType { get; set; }

        /// <summary>
        /// 车辆载荷
        /// </summary>
        public string? VehicleLoad { get; set; }

        /// <summary>
        /// 最高运行速度
        /// </summary>
        public string? MaximumOperatingSpeed { get; set; }

        /// <summary>
        /// 最高运行速度
        /// </summary>
        public string? AverageTravelSpeed { get; set; }

        /// <summary>
        /// 平均站停时间
        /// </summary>
        public string? AverageDwellTime { get; set; }

        /// <summary>
        /// 在满载条件下，在平直干燥轨道上，车轮半磨耗状态，额定电压时，平均加速度
        /// </summary>
        public string? AverageAcceleration { get; set; }

        /// <summary>
        /// 在满载条件下，在平直干燥轨道上，三车轮半磨耗状态，列车在最高运行速度80km/h，从给制动指令到停车时，平均减速度
        /// </summary>
        public string? AverageDeceleration { get; set; }

        /// <summary>
        /// 列车纵向冲击率
        /// </summary>
        public string? TrainImpactRate { get; set; }

        /// <summary>
        /// 列车故障状态下运行能力要求
        /// </summary>
        public string? RequirementsFaultStatus { get; set; }

        /// <summary>
        /// 列车坡道救援能力要求
        /// </summary>
        public string? RequirementsRampRescue { get; set; }

        /// <summary>
        /// 车内噪声水平要求
        /// </summary>
        public string? RequirementsNoiseLevelIn { get; set; }

        /// <summary>
        /// 车外噪声水平要求
        /// </summary>
        public string? RequirementsNoiseLevelOut { get; set; }

        /// <summary>
        /// 备注
        /// </summary> 
        public string? Remark { get; set; }

        /// <summary>
        /// 拓展字段1
        /// </summary>
        public string? ExtendedField1 { get; set; }

        /// <summary>
        /// 拓展字段2
        /// </summary>
        public string? ExtendedField2 { get; set; }

        /// <summary>
        /// 拓展字段3
        /// </summary>
        public string? ExtendedField3 { get; set; }

        /// <summary>
        /// 拓展字段4
        /// </summary>
        public string? ExtendedField4 { get; set; }

        /// <summary>
        /// 拓展字段5
        /// </summary>
        public string? ExtendedField5 { get; set; }

        /// <summary>
        /// 拓展字段6
        /// </summary>
        public string? ExtendedField6 { get; set; }

        /// <summary>
        /// 拓展字段7
        /// </summary>
        public string? ExtendedField7 { get; set; }

        /// <summary>
        /// 拓展字段8
        /// </summary>
        public string? ExtendedField8 { get; set; }

        /// <summary>
        /// 拓展字段9
        /// </summary>
        public string? ExtendedField9 { get; set; }

        /// <summary>
        /// 拓展字段10
        /// </summary>
        public string? ExtendedField10 { get; set; }
    }
}
