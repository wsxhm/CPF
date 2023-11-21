using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;

namespace CPF.Design
{
    /// <summary>
    /// 双通道，支持双向侦听，非Message模式
    /// </summary>
    public class NamedPipe : IDisposable
    {
        public string Name { get; private set; }

        BinaryWriter binaryWriter;
        //BinaryReader binaryReader;
        NamedPipeServerStream serverStreamIn;
        NamedPipeClientStream pipeClientIn;
        NamedPipeServerStream serverStreamOut;
        NamedPipeClientStream pipeClientOut;
        ManualResetEvent invokeMre = new ManualResetEvent(false);
        ConcurrentQueue<string> msgs = new ConcurrentQueue<string>();
        public event Action<NamedPipe, string, bool> AcceptMessage;
        bool isServer;
        public NamedPipe(string name, bool isServer = true, int num = 1)
        {
            Name = name;
            this.isServer = isServer;
            if (isServer)
            {
                serverStreamIn = new NamedPipeServerStream(name + "_in", PipeDirection.Out, num);
                binaryWriter = new BinaryWriter(serverStreamIn, Encoding.Unicode);
                serverStreamOut = new NamedPipeServerStream(name + "_out", PipeDirection.In, num);
            }
            else
            {
                pipeClientIn = new NamedPipeClientStream(".", name + "_out", PipeDirection.Out);
                binaryWriter = new BinaryWriter(pipeClientIn, Encoding.Unicode);

                pipeClientOut = new NamedPipeClientStream(".", name + "_in", PipeDirection.In);
            }
        }

        public bool IsConnected
        {
            get
            {
                if (isServer)
                {
                    return serverStreamIn.IsConnected && serverStreamOut.IsConnected;
                }
                return pipeClientIn.IsConnected && pipeClientOut.IsConnected;
            }
        }
        //ManualResetEvent invokeMre = new ManualResetEvent(false);
        /// <summary>
        /// 等待连接
        /// </summary>
        /// <param name="timeout">客户端模式超时</param>
        /// <param name="useEvent">使用接收事件</param>
        public void WaitForConnection(int timeout = 2000, bool useEvent = false)
        {
            if (isServer)
            {
                serverStreamIn.WaitForConnection();
                serverStreamOut.WaitForConnection();
            }
            else
            {
                pipeClientOut.Connect(timeout);
                pipeClientIn.Connect(timeout);
                //pipeClient.ReadMode = PipeTransmissionMode.Message;
            }

            new Thread(() =>
            {
                while (IsConnected)
                {
                    invokeMre.WaitOne();
                    invokeMre.Reset();
                    if (!IsConnected)
                    {
                        break;
                    }
                    while (msgs.TryDequeue(out var msg))
                    {
                        try
                        {
                            var bytes = Encoding.Unicode.GetBytes(msg);
                            binaryWriter.Write(BitConverter.GetBytes(bytes.Length));
                            binaryWriter.Write(bytes);
                            binaryWriter.Flush();
                        }
                        catch (Exception e)
                        {
                            break;
                        }
                    }
                }
            })
            { Name = "发送消息" + Name, IsBackground = true }.Start();
            //invokeMre.WaitOne();
            //invokeMre.Reset();
            if (useEvent)
            {
                new Thread(() =>
                {
                    try
                    {
                        while (IsConnected)
                        {
                            var msg = ReadString();
                            if (AcceptMessage != null)
                            {
                                AcceptMessage(this, msg, true);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (AcceptMessage != null)
                        {
                            AcceptMessage(this, null, false);
                        }
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                })
                { IsBackground = true, Name = "接受消息" + Name }.Start();
            }
        }

        public void WriteString(string str)
        {
            msgs.Enqueue(str);
            invokeMre.Set();
        }

        byte[] d = new byte[10240];
        public string ReadString()
        {
            PipeStream s = serverStreamOut;
            if (!isServer)
            {
                s = pipeClientOut;
            }
            //MemoryStream ms = new MemoryStream();
            //do
            //{
            //    ms.Write(d, 0, s.Read(d, 0, d.Length));
            //}
            //while (!s.IsMessageComplete);
            //{
            //    return Encoding.Unicode.GetString(ms.ToArray());
            //}
            byte[] lenB = new byte[4];
            s.Read(lenB, 0, lenB.Length);
            var len = BitConverter.ToInt32(lenB, 0);
            var data = new byte[len];
            s.Read(data, 0, len);
            return Encoding.Unicode.GetString(data);
        }
        public void Close()
        {
            serverStreamIn?.Close();
            pipeClientIn?.Close();
            serverStreamOut?.Close();
            pipeClientOut?.Close();
            invokeMre.Set();
        }

        public void Disconnect()
        {
            serverStreamIn?.Disconnect();
            serverStreamOut?.Disconnect();
            pipeClientOut?.Dispose();
            pipeClientIn?.Dispose();
            if (!isServer)
            {
                binaryWriter?.Dispose();
                pipeClientIn = new NamedPipeClientStream(".", Name + "_out", PipeDirection.Out);
                binaryWriter = new BinaryWriter(pipeClientIn, Encoding.Unicode);
                pipeClientOut = new NamedPipeClientStream(".", Name + "_in", PipeDirection.In);
            }
            invokeMre.Set();
        }

        public void Dispose()
        {
            binaryWriter?.Dispose();
            serverStreamIn?.Dispose();
            pipeClientIn?.Dispose();
            serverStreamOut?.Dispose();
            pipeClientOut?.Dispose();
            invokeMre.Set();
        }
    }
}
