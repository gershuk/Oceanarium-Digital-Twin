#nullable enable

using System;

using UnityEngine;

namespace Aqua.SocketMonoBehaviourWrapper
{
    public enum SocketMemberType
    {
        Field,
        Property,
    }

    public class BaseSocketWrapper : MonoBehaviour
    {
        public string AssemblyQualifiedName;
        public string Name;
        public MonoBehaviour Owner;
        public SocketMemberType SocketMemberType;
        public string TypeName;

        public void SetAllBase (MonoBehaviour owner, string name, Type dataType, SocketMemberType socketMemberType)
        {
            Owner = owner;
            Name = name;
            AssemblyQualifiedName = dataType.AssemblyQualifiedName;
            TypeName = dataType.Name;
            SocketMemberType = socketMemberType;
        }
    }
}