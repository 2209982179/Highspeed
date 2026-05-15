using ExCSS;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace highspeed.business._01.Models
{
    //通信协议格式
    public class ProtocolConfig
    {
        //报文头
        public ushort MessageHeader = 0x7E4C;

        //发送方终端号
        public char[] SenderPortID { set;  get;}

        //发送方端口号
        public ushort SenderPortNumber { set; get; }

        //接收方终端号
        public char[] ReceiverPortID { set; get; }

        //接收方端口号
        public ushort ReceiverPortNumber { set; get; }

        //备用 
        public byte Reserved = 0x00000000;

        //报文序号 
        public ushort MessageSequenceNumber = 0x0001;

        //报文接收方
        public ushort MessageRecipient = 0x0001;

        //时间
        public int Time { set; get; }

        //毫秒时间
        public ushort Milliseconds { set; get; }

        //报文长度
        public uint MessageLength { set; get; }

        //报文
        public byte[] Message { set; get; }

        //报文详细内容
        public ConfigureMessage configureMessage = new ConfigureMessage();
    }

    //报文内容格式
    public class ConfigureMessage
    {
        //指令条数
        public ushort Count { set; get; }

        //方向
        public byte Direction { set; get; }

        //指令拼接byte报文
        public byte[] ContentByteArrays { set; get; }

        //发送的指令
        public List<ConfigureMessageContent> ContentList = new List<ConfigureMessageContent>();
    }
    
    //报文内容
    public class ConfigureMessageContent
    {
        //配置类别
        public byte Category { set; get; }

        //配置类别描述
        public string CategoryRemark { set; get; }

        //配置项
        public byte Description { set; get; }

        //配置项描述
        public string DescriptionRemark { set; get; }

        //配置内容  
        public byte Content { set; get; }

        //配置内容描述
        public string ContentRemark { set; get; }

        //备用描述
        public string Remark { set; get; }
    }
}
