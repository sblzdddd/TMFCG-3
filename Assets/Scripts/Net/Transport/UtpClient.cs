using Net.Codec;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

namespace Net.Transport
{
    public class UtpClient
    {
        public NetworkDriver driver;
        public NetworkConnection connection;
        public NetDispatcher Dispatcher { get; } = new();

        public void Connect(string host, ushort port)
        {
            driver = NetworkDriver.Create();
            var endpoint = NetworkEndpoint.Parse(host, port);
            connection = driver.Connect(endpoint);
        }
        
        public void Cleanup()
        {
            driver.Dispose();
        }

        public void Update()
        {
            driver.ScheduleUpdate().Complete();

            NetworkEvent.Type evt;
            while ((evt = connection.PopEvent(driver, out var stream))
                   != NetworkEvent.Type.Empty)
            {
                if (evt != NetworkEvent.Type.Data) continue;
                var type = stream.ReadUShort();
                var payload = new byte[stream.Length - 2];
                stream.ReadBytes(payload);

                Debug.Log($"Client recv msg {type}");
                Dispatcher.Dispatch(type, payload);
            }
        }
        
        public void Send<T>(T msg)
        {
            var typeId = NetMessageRegistry.GetId(typeof(T));
            var payload = MessagePackCodec.Serialize(msg);
            SendRaw(typeId, payload);
        }

        private void SendRaw(ushort type, byte[] payload)
        {
            driver.BeginSend(connection, out var writer);
            writer.WriteUShort(type);
            writer.WriteBytes(payload);
            driver.EndSend(writer);
        }
    }
}