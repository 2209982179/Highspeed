using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BKDataAnalysis;
using highspeed.framework.Common;

namespace IntegratedSystem.DAL
{
    /// <summary>
    /// UDP组装
    /// </summary>
    public class UDPClientDefineModel
    {
        /// <summary>
        /// 整条通信信息
        /// </summary>
        public string IPInfo { get; set; } = "";

        /// <summary>
        /// 用于发送的UDP
        /// </summary>
        public UdpClient SendClient { get; set; }
        /// <summary>
        /// 发送目标
        /// </summary>
        public IPEndPoint SendRemoteIpep { get; set; }

        /// <summary>
        /// 用于接收的UDP
        /// </summary>
        public UdpClient ReceiveClient { get; set; }
        /// <summary>
        /// 接收目标
        /// </summary>
        public IPEndPoint ReceiveRemoteIpep { get; set; }

        /// <summary>
        /// 本机IP
        /// </summary>
        public string Local_IP { get; set; }
        /// <summary>
        /// 本机发送端口
        /// </summary>
        public string Local_Port1 { get; set; }
        /// <summary>
        /// 本机接收端口
        /// </summary>
        public string Local_Port2 { get; set; }
        /// <summary>
        /// 服务端IP
        /// </summary>
        public string Ser_IP { get; set; }
        /// <summary>
        /// 服务器发送端口
        /// </summary>
        public string Ser_Port1 { get; set; }
        /// <summary>
        /// 服务器接收端口
        /// </summary>
        public string Ser_Port2 { get; set; }


    }

    /// <summary>
    /// UDP类
    /// </summary>
    public class c_UDPHelper
    {
        //UDP客户端，流，IP，端口等信息
        public Dictionary<string, UDPClientDefineModel> AllUDPInfos = new Dictionary<string, UDPClientDefineModel>();

        /// <summary>
        /// 接收数据队列——显示器界面
        /// </summary>
        public static Dictionary<string, Queue> AllQueues_ReceiveData_Monitors = new Dictionary<string, Queue>();

        /// <summary>
        /// 接收数据队列——拓扑图
        /// </summary>
        public static Dictionary<string, Queue> AllQueues_ReceiveData_AD = new Dictionary<string, Queue>();

        public static c_kernel32Helper kernel32Helper = new c_kernel32Helper(Environment.CurrentDirectory + "\\UserSettings\\UserSettings.ini");

        /// <summary>
        /// UDP连接创建
        /// </summary>
        /// <param name="ErrLog">错误信息</param>
        /// <returns></returns>
        public bool StartConnect(ref string ErrLog)
        {
            try
            {
                //清除队列
                lock (AllQueues_ReceiveData_AD)
                {
                    AllQueues_ReceiveData_AD.Clear();
                }
                //清除队列
                lock (AllQueues_ReceiveData_Monitors)
                {
                    AllQueues_ReceiveData_Monitors.Clear();
                }

                //初始化UDP客户端，IP，端口
                AllUDPInfos.Clear();

                string IP = kernel32Helper.GetIniString("NetSettings", "IPAddress", "");

                string portRegx = @"^\d+$";

                bool ck = false;
                foreach (string ob1 in IP.Split(new char[] { ';' }))
                {
                    if (ob1 == "")
                    {
                        continue;
                    }

                    if (AllUDPInfos.ContainsKey(ob1))
                    {
                        continue;
                    }

                    string Local_IP = ob1.Split(new char[] { ',' })[0];//IP
                    string Local_Port1 = ob1.Split(new char[] { ',' })[1];//发送端口
                    string Local_Port2 = ob1.Split(new char[] { ',' })[2];//接收端口
                    string Ser_IP = ob1.Split(new char[] { ',' })[3];//IP
                    string Ser_Port1 = ob1.Split(new char[] { ',' })[4];//服务器发送端口
                    string Ser_Port2 = ob1.Split(new char[] { ',' })[5];//服务器接收端口

                    if (Regex.IsMatch(Local_Port1, portRegx) && Regex.IsMatch(Local_Port2, portRegx) && Regex.IsMatch(Ser_Port1, portRegx) && Regex.IsMatch(Ser_Port2, portRegx))
                    {
                        UDPClientDefineModel uDPClientDefineModel = new UDPClientDefineModel();
                        uDPClientDefineModel.Local_IP = Local_IP;
                        uDPClientDefineModel.Local_Port1 = Local_Port1;
                        uDPClientDefineModel.Local_Port2 = Local_Port2;
                        uDPClientDefineModel.Ser_IP = Ser_IP;
                        uDPClientDefineModel.Ser_Port1 = Ser_Port1;
                        uDPClientDefineModel.Ser_Port2 = Ser_Port2;

                        //收发同端口时
                        if (Local_Port1 == Local_Port2)
                        {
                            //发送接收
                            UdpClient client1 = GetUdpClient(ob1, true, ref ErrLog);

                            uDPClientDefineModel.SendClient = client1;
                            uDPClientDefineModel.ReceiveClient = client1;
                        }
                        else
                        {
                            //发送
                            UdpClient client1 = GetUdpClient(ob1, true, ref ErrLog);
                            //接收
                            UdpClient client2 = GetUdpClient(ob1, false, ref ErrLog);

                            uDPClientDefineModel.SendClient = client1;
                            uDPClientDefineModel.ReceiveClient = client2;
                        }

                        //收发同端口时
                        if (Ser_Port1 == Ser_Port2)
                        {
                            //收发
                            IPEndPoint remoteIpep = GetIPEndPoint(ob1, true, ref ErrLog);

                            uDPClientDefineModel.SendRemoteIpep = remoteIpep;
                            uDPClientDefineModel.ReceiveRemoteIpep = remoteIpep;
                        }
                        else
                        {
                            //发送
                            IPEndPoint remoteIpep1 = GetIPEndPoint(ob1, true, ref ErrLog);
                            //接收
                            IPEndPoint remoteIpep2 = GetIPEndPoint(ob1, false, ref ErrLog);

                            uDPClientDefineModel.SendRemoteIpep = remoteIpep1;
                            uDPClientDefineModel.ReceiveRemoteIpep = remoteIpep2;
                        }

                        if (!AllUDPInfos.ContainsKey(ob1))
                        {
                            AllUDPInfos.Add(ob1, uDPClientDefineModel);
                        }

                        ck = true;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (ck)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrLog = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取或构造UdpClient
        /// </summary>
        /// <param name="IPInfo"></param>
        /// <param name="isSend"></param>
        /// <param name="ErrLog"></param>
        /// <returns></returns>
        public UdpClient GetUdpClient(string IPInfo, bool isSend, ref string ErrLog)
        {
            try
            {
                string Local_IP = IPInfo.Split(new char[] { ',' })[2];//IP
                string Local_Port1 = IPInfo.Split(new char[] { ',' })[3];//发送端口
                string Local_Port2 = IPInfo.Split(new char[] { ',' })[4];//接收端口
                string Ser_IP = IPInfo.Split(new char[] { ',' })[5];//IP
                string Ser_Port1 = IPInfo.Split(new char[] { ',' })[6];//服务器发送端口
                string Ser_Port2 = IPInfo.Split(new char[] { ',' })[7];//服务器接收端口

                if (isSend)//发送Udp
                {
                    IEnumerable<KeyValuePair<string, UDPClientDefineModel>> SendUDPs = AllUDPInfos.Where(d => d.Value.Local_IP == Local_IP && d.Value.Local_Port1 == Local_Port1);

                    if (SendUDPs.Count() > 0)
                    {
                        return SendUDPs.FirstOrDefault().Value.SendClient;
                    }
                    else
                    {
                        UdpClient client1 = new UdpClient();
                        try
                        {
                            IPEndPoint LocalP1 = new IPEndPoint(IPAddress.Parse(Local_IP), int.Parse(Local_Port1));
                            client1 = new UdpClient(LocalP1);
                            return client1;
                        }
                        catch (Exception ex)
                        {
                            ErrLog = Local_IP + int.Parse(Local_Port1).ToString() + ",UDP启动失败\r\n" + ex.Message;
                            return null;
                        }
                    }
                }
                else
                {
                    IEnumerable<KeyValuePair<string, UDPClientDefineModel>> ReceiveUDPs = AllUDPInfos.Where(d => d.Value.Local_IP == Local_IP && d.Value.Local_Port2 == Local_Port2);

                    if (ReceiveUDPs.Count() > 0)
                    {
                        return ReceiveUDPs.FirstOrDefault().Value.ReceiveClient;
                    }
                    else
                    {
                        UdpClient client1 = new UdpClient();
                        try
                        {
                            IPEndPoint LocalP1 = new IPEndPoint(IPAddress.Parse(Local_IP), int.Parse(Local_Port2));
                            client1 = new UdpClient(LocalP1);
                            return client1;
                        }
                        catch (Exception ex)
                        {
                            ErrLog = Local_IP + int.Parse(Local_Port2).ToString() + ",UDP启动失败\r\n" + ex.Message;
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrLog = "UDP启动失败\r\n" + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取或构造IPEndPoint
        /// </summary>
        /// <param name="IPInfo"></param>
        /// <param name="isSend"></param>
        /// <param name="ErrLog"></param>
        /// <returns></returns>
        public IPEndPoint GetIPEndPoint(string IPInfo, bool isSend, ref string ErrLog)
        {
            try
            {
                string Local_IP = IPInfo.Split(new char[] { ',' })[2];//IP
                string Local_Port1 = IPInfo.Split(new char[] { ',' })[3];//发送端口
                string Local_Port2 = IPInfo.Split(new char[] { ',' })[4];//接收端口
                string Ser_IP = IPInfo.Split(new char[] { ',' })[5];//IP
                string Ser_Port1 = IPInfo.Split(new char[] { ',' })[6];//服务器发送端口
                string Ser_Port2 = IPInfo.Split(new char[] { ',' })[7];//服务器接收端口

                if (isSend)//发送Udp
                {
                    IEnumerable<KeyValuePair<string, UDPClientDefineModel>> SendUDPs = AllUDPInfos.Where(d => d.Value.Ser_IP == Ser_IP && d.Value.Ser_Port2 == Ser_Port2);

                    if (SendUDPs.Count() > 0)
                    {
                        return SendUDPs.FirstOrDefault().Value.ReceiveRemoteIpep;
                    }
                    else
                    {
                        //发送
                        IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(Ser_IP), int.Parse(Ser_Port2));
                        return remoteIpep;
                    }
                }
                else
                {
                    IEnumerable<KeyValuePair<string, UDPClientDefineModel>> ReceiveUDPs = AllUDPInfos.Where(d => d.Value.Ser_IP == Ser_IP && d.Value.Ser_Port1 == Ser_Port1);

                    if (ReceiveUDPs.Count() > 0)
                    {
                        return ReceiveUDPs.FirstOrDefault().Value.SendRemoteIpep;
                    }
                    else
                    {
                        //接收
                        IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse(Ser_IP), int.Parse(Ser_Port1));
                        return remoteIpep;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrLog = "UDP启动失败\r\n" + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public bool StopConnect()
        {
            try
            {
                lock (AllQueues_ReceiveData_AD)
                {
                    AllQueues_ReceiveData_AD.Clear();
                }
                //清除队列
                lock (AllQueues_ReceiveData_Monitors)
                {
                    AllQueues_ReceiveData_Monitors.Clear();
                }

                foreach (UDPClientDefineModel UDP1 in AllUDPInfos.Values)
                {
                    try
                    {
                        UDP1.SendClient.Close();
                    }
                    catch (Exception)
                    {
                    }
                }
                foreach (UDPClientDefineModel UDP1 in AllUDPInfos.Values)
                {
                    try
                    {
                        UDP1.ReceiveClient.Close();
                    }
                    catch (Exception)
                    {
                    }
                }

                AllUDPInfos.Clear();

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return false;
            }
        }

        /// <summary>
        /// 发送UDP指令
        /// </summary>
        /// <param name="IPInfo">服务器信息</param>
        /// <param name="Messages">报文</param>
        /// <param name="errors">错误信息</param>
        /// <returns></returns>
        public bool SendAllDatas(string IPInfo, byte[] Messages, ref string errors)
        {
            if (Messages.Length != 0)
            {
                try
                {
                    byte[] sendBytes = Messages;

                    foreach (KeyValuePair<string, UDPClientDefineModel> keyValue in AllUDPInfos)
                    {
                        if (keyValue.Key == IPInfo)
                        {
                            int val = keyValue.Value.SendClient.Send(sendBytes, sendBytes.Length, keyValue.Value.SendRemoteIpep);
                            Logger.Info("成功", "消息发出：" + BitConverter.ToString(Messages).ToUpper());
                            return true;
                        }
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    Logger.Error("软件出错","消息发出失败：" + ex.Message + "\r\n" + ex.ToString());
                    errors = ex.Message;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="token">线程通知</param>
        /// <param name="IPInfo">服务器信息</param>
        /// <param name="MessageHeaderLength">接收长度</param>
        /// <param name="errors">返回错误信息</param>
        /// <returns></returns>
        public void ReceiveAllData(CancellationToken token, string IPInfo, int MessageHeaderLength, ref string errors)
        {
            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    byte[] receiveBytes = new byte[] { };
                    bool RD = false;
                    while (true)
                    {
                        kernel32Helper.SuperSleep(10 * 1000);//单位微秒
                                                                      //接收
                        receiveBytes = new byte[MessageHeaderLength];

                        foreach (KeyValuePair<string, UDPClientDefineModel> keyValue in AllUDPInfos)
                        {
                            if (keyValue.Key == IPInfo)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    return;
                                }

                                string Ser_IP = keyValue.Key.Split(new char[] { ',' })[5];//IP

                                lock (Ser_IP)
                                {
                                    IPEndPoint remoteIpep = keyValue.Value.ReceiveRemoteIpep;
                                    receiveBytes = keyValue.Value.ReceiveClient.Receive(ref remoteIpep);

                                    // 检查实际接收的 IP 是否匹配预期的 Ser_IP
                                    if (remoteIpep.Address.ToString() != Ser_IP)
                                    {
                                        continue; // 丢弃不匹配的数据，继续接收
                                    }
                                    if (receiveBytes.Length == 0)
                                    {
                                        RD = false;
                                        receiveBytes = new byte[MessageHeaderLength];
                                    }
                                    else
                                    {
                                        RD = true;
                                    }
                                }

                                break;
                            }
                        }

                        if (RD)
                        {
                            break;
                        }
                    }

                    if (receiveBytes.Length > 0)
                    {
                        Queue TempPair1 = null;
                        AllQueues_ReceiveData_AD.TryGetValue(IPInfo, out TempPair1);
                        if (TempPair1 != null)
                        {
                            lock (TempPair1)
                            {
                                if (TempPair1.Count > 1000)
                                {
                                    TempPair1.Clear();
                                }
                                if (token.IsCancellationRequested)
                                {
                                    return;
                                }
                                TempPair1.Enqueue(receiveBytes);
                            }
                        }

                        Queue TempPair2 = null; ;
                       

                        Queue TempPair3 = null;
                        AllQueues_ReceiveData_Monitors.TryGetValue(IPInfo, out TempPair3);
                        if (TempPair3 != null)
                        {
                            lock (TempPair3)
                            {
                                if (TempPair3.Count > 200)
                                {
                                    TempPair3.Clear();
                                }
                                if (token.IsCancellationRequested)
                                {
                                    return;
                                }
                                TempPair3.Enqueue(receiveBytes);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    errors = ex.Message;
                }
            }
        }
    }
}