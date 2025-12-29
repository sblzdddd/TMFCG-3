using System;
using System.Collections.Generic;

namespace Net
{
    public static class NetMessageRegistry
    {
        private static readonly Dictionary<Type, ushort> TypeToId = new();
        private static readonly Dictionary<ushort, Type> IDToType = new();

        public static void Register<T>(ushort id)
        {
            var type = typeof(T);
            TypeToId[type] = id;
            IDToType[id] = type;
        }

        public static ushort GetId(Type type)
            => TypeToId[type];

        public static Type GetType(ushort id)
            => IDToType[id];
    }

}