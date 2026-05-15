namespace highspeed.business._00.Enum
{
    public class CommonEnum
    {
        /// <summary>
        /// 车辆类型
        /// </summary>  
        public enum TrainType
        {
            AW0,
            AW1,
            AW2
        }

        /// <summary>
        /// 指令类型
        /// </summary>  
        public enum StepType
        {
            //记录
            Recode,

            //执行
            Execute,

            //时序
            Temporality,

            //IF 判断
            If,

            //FOR 循环
            For
        }

        /// <summary>
        /// 指令发送接收类型
        /// </summary>  
        public enum SendReceiveType
        {
            //发送
            Send,

            //接收
            Receive,

            //其他
            Other
        }

        /// <summary>
        /// 指令发送接收类型
        /// </summary>  
        public enum temporalityType
        {
            //发送
            IF,

            //接收
            FOR,

            //其他
            Other
        }

        /// <summary>
        /// 参数类型
        /// </summary>  
        public enum ParameterValueType
        {
            //记录
            Recode,

            //执行
            Execute,

            //时序
            Temporality
        }
    }
}
