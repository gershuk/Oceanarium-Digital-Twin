using System;
using System.Linq;

using Aqua.SocketSystem;

using UnityEngine;

namespace Aqua.SocketMonoBehaviourWrapper
{
    public class SocketWrapperGenerator : MonoBehaviour
    {
        private const string _inputSocketContainerName = "InputSockets";
        private const string _outputSocketContainerName = "OutputSockets";
        private static readonly Type _inputInterfaceType = typeof(IInputSocket<>);
        private static readonly Type _outputInterfaceType = typeof(IOutputSocket<>);

        private void AddFiledsSockets (MonoBehaviour monoBehaviour,
                                            GameObject inputSocketContainer,
                                            GameObject outputSocketContainer)
        {
            var type = monoBehaviour.GetType();
            var fields = type.GetFields().Where(static p => p.IsPublic is true);

            foreach (var field in fields)
            {
                var returnType = field.FieldType;
                if (!returnType.IsGenericType)
                    continue;

                var genericTypeDefinition = returnType.GetGenericTypeDefinition();
                var returnTypeArgument = returnType.GetGenericArguments()[0];

                if (genericTypeDefinition == _inputInterfaceType)
                {
                    var wrapper = inputSocketContainer.AddComponent<InputSocketWrapper>();
                    wrapper.SetAllBase(monoBehaviour, field.Name, returnTypeArgument, SocketMemberType.Field);
                }

                if (genericTypeDefinition == _outputInterfaceType)
                {
                    var wrapper = outputSocketContainer.AddComponent<OutputSocketWrapper>();
                    wrapper.SetAllBase(monoBehaviour, field.Name, returnTypeArgument, SocketMemberType.Field);
                }
            }
        }

        //ToDo : Take out the general code from the methods into another method.
        private void AddPropertiesSockets (MonoBehaviour monoBehaviour,
                                    GameObject inputSocketContainer,
                                    GameObject outputSocketContainer)
        {
            var type = monoBehaviour.GetType();
            var properties = type.GetProperties().Where(static p => p.GetGetMethod()?.IsPublic is true);

            foreach (var property in properties)
            {
                var returnType = property.GetGetMethod().ReturnType;
                if (!returnType.IsGenericType)
                    continue;

                var genericTypeDefinition = returnType.GetGenericTypeDefinition();
                var returnTypeArgument = returnType.GetGenericArguments()[0];

                if (genericTypeDefinition == _inputInterfaceType)
                {
                    var wrapper = inputSocketContainer.AddComponent<InputSocketWrapper>();
                    wrapper.SetAllBase(monoBehaviour, property.Name, returnTypeArgument, SocketMemberType.Property);
                }

                if (genericTypeDefinition == _outputInterfaceType)
                {
                    var wrapper = outputSocketContainer.AddComponent<OutputSocketWrapper>();
                    wrapper.SetAllBase(monoBehaviour, property.Name, returnTypeArgument, SocketMemberType.Property);
                }
            }
        }

        private void Start () =>
                            //ToDo : Add socket binding.
                            DeleteAll();

        [ContextMenu(nameof(DeleteAll))]
        public void DeleteAll ()
        {
            DestroyImmediate(transform.Find(_inputSocketContainerName)?.gameObject);
            DestroyImmediate(transform.Find(_outputSocketContainerName)?.gameObject);
        }

        [ContextMenu(nameof(Generate))]
        public void Generate ()
        {
            DeleteAll();

            var monoBehaviours = gameObject.GetComponents<MonoBehaviour>();

            var inputSocketContainer = new GameObject(_inputSocketContainerName);
            inputSocketContainer.transform.parent = transform;
            inputSocketContainer.transform.localPosition = Vector3.zero;

            var outputSocketContainer = new GameObject(_outputSocketContainerName);
            outputSocketContainer.transform.parent = transform;
            outputSocketContainer.transform.localPosition = Vector3.zero;

            foreach (var monoBehaviour in monoBehaviours)
            {
                AddPropertiesSockets(monoBehaviour, inputSocketContainer, outputSocketContainer);
                AddFiledsSockets(monoBehaviour, inputSocketContainer, outputSocketContainer);
            }
        }
    }
}