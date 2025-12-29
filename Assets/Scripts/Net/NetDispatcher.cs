using System;
using System.Collections.Generic;
using Net.Codec;

namespace Net
{
    public class NetDispatcher
    {
        private readonly Dictionary<Type, Delegate> handlers = new();

        public void On<T>(Action<T> handler)
        {
            handlers[typeof(T)] = handler;
        }

        public void Dispatch(ushort typeId, byte[] payload)
        {
            var type = NetMessageRegistry.GetType(typeId);

            if (!handlers.TryGetValue(type, out var handler)) return;
            var obj = MessagePackCodec.Deserialize(payload, type);
            handler.DynamicInvoke(obj);
        }
    }
}