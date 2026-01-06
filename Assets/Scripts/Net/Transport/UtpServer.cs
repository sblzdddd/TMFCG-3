using Net.Codec;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net.Transport
{
    public class UtpServer
    {
        public NetworkDriver Driver { get; private set; }
        public NativeList<NetworkConnection> Connections { get; private set; }
        public NetDispatcher Dispatcher { get; } = new();
        

        public void Start(ushort port=12637)
        {
            Driver = NetworkDriver.Create();
            Connections = new NativeList<NetworkConnection>(4, Allocator.Persistent);

            var endpoint = NetworkEndpoint.AnyIpv4.WithPort(port);
            if (Driver.Bind(endpoint) != 0)
            {
                Debug.LogError("Failed to bind to port 12637.");
                return;
            }
            Driver.Listen();

            Debug.Log("UTP Server started");
        }

        public void Update()
        {
            Driver.ScheduleUpdate().Complete();

            // 接新连接
            NetworkConnection conn;
            while ((conn = Driver.Accept()) != default)
                Connections.Add(conn);

            // 收消息
            for (var i = 0; i < Connections.Length; i++)
            {
                var connection = Connections[i];
                if (!connection.IsCreated)
                    continue;

                NetworkEvent.Type evt;
                while ((evt = Driver.PopEventForConnection(connection, out var stream))
                       != NetworkEvent.Type.Empty)
                {
                    if (evt != NetworkEvent.Type.Data)
                        continue;

                    var type = stream.ReadUShort();

                    var payloadLen = stream.Length - 2;
                    var payload = new byte[payloadLen];
                    stream.ReadBytes(payload);

                    OnMessage(connection, type, payload);
                }
            }

        }

        public void Cleanup()
        {
            if (!Driver.IsCreated) return;
            Driver.Dispose();
            Connections.Dispose();
        }
        
        void OnMessage(NetworkConnection conn, ushort type, byte[] payload)
        {
            Debug.Log($"Server recv msg {type}");
            Dispatcher.Dispatch(type, payload);
        }

        public void Broadcast<T>(T msg)
        {
            foreach (var conn in Connections)
            {
                if (!conn.IsCreated) continue;
                Send(conn, msg);
            }
        }
        
        public void Send<T>(NetworkConnection conn, T msg)
        {
            var typeId = NetMessageRegistry.GetId(typeof(T));
            var payload = MessagePackCodec.Serialize(msg);
            SendRaw(conn, typeId, payload);
        }

        void SendRaw(NetworkConnection conn, ushort type, byte[] payload)
        {
            Driver.BeginSend(conn, out var writer);
            writer.WriteUShort(type);
            writer.WriteBytes(payload);
            Driver.EndSend(writer);
        }
    }
}