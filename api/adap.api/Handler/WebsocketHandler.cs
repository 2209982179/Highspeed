using adap.safetyandreliabilityapi._05.Data.Reliability_Prediction;
using Google.Protobuf.WellKnownTypes;
using highspeed.business._01.Models;
using Org.BouncyCastle.Ocsp;
using System.Collections.Generic;
using System.Data;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace highspeed.api.WebsocketHandler
{
    /// <summary>
    /// WebSocket服务
    /// </summary>
    public class WebsocketHandler
    {
        WebSocket socket;
        public const int BufferSize = 4096;

        public WebsocketHandler(WebSocket socket)
        {
            this.socket = socket;
        }

        public async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);
            while (this.socket.State == WebSocketState.Open)
            {
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);

                if (incoming.Count > 0)
                {
                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, incoming.Count);
                    if (receivedMessage == "获取结果ID:1")
                    {
                        TestCaseMixClass? DataRs = await TestCase_Handles.GetTestCaseList();

                        foreach (var testCase in DataRs.testCaseTree)
                        {
                            Thread.Sleep(10);
                            DataRs = await TestCase_Handles.GetTestCaseList();
                            // 序列化为JSON
                            string jsonResponse = JsonSerializer.Serialize(DataRs, new JsonSerializerOptions
                            {
                                //PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                WriteIndented = false,
                                // 添加以下配置来禁用字符转义
                                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                            });

                            // 发送JSON响应
                            byte[] responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                            var outgoing = new ArraySegment<byte>(responseBytes);
                            await this.socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                }
            }
        }

        public static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;
            var socket = await hc.WebSockets.AcceptWebSocketAsync();
            var h = new WebsocketHandler(socket);
            await h.EchoLoop();
        }

        /// <summary> 
        /// 路由绑定处理 
        /// </summary> 
        /// <param name="app"></param> 
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(WebsocketHandler.Acceptor);
        }
    }
}
