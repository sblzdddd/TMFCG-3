using System;
using MessagePack;

namespace Net.Codec
{

    public static class MessagePackCodec
    {
        public static byte[] Serialize<T>(T value)
            => MessagePackSerializer.Serialize(value);

        public static T Deserialize<T>(byte[] bytes)
            => MessagePackSerializer.Deserialize<T>(bytes);
        
        public static object Deserialize(byte[] bytes, Type type)
            => MessagePackSerializer.Deserialize(type, bytes);
    }

}