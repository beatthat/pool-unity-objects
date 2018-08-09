using System;
using System.Collections.Generic;
using BeatThat.Service;
using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
    /// <summary>
    /// Convenience attrbibute for registering an UnityObjectPool<T> class.
    /// 
    /// Allows you to do this:
    /// 
    /// <code>
    /// [RegisterObjectPool] // auto registers service as IObjectPool<MyObjectType> and IObjectFactory<MyObjectType>
    /// public class MyObjectFactory : UnityObjectPool<MyObjectType> {}
    /// </code>
    /// 
    /// ...instead of the longer, boiler-plate version
    /// <code>
    /// [ResisterService(
    ///     proxyInterfaces: new Type[] { 
    ///         typeof(IObjectPool<MyObjectType>),
    ///         typeof(IObjectFactory<MyObjectType>)
    ///     }
    /// )]
    /// public class MyObjectPool : UnityObjectPool<MyObjectType> {}
    /// </code>
    /// </summary>
    public class RegisterObjectPoolAttribute : RegisterServiceAttribute
	{
        public RegisterObjectPoolAttribute(
            InterfaceRegistrationPolicy interfaceRegistrationPolicy
                = InterfaceRegistrationPolicy.RegisterInterfacesDeclaredOnTypeIfNoProxyInterfacesArgument,
            Type[] proxyInterfaces = null,
            int priority = 0) 
            : base(null, interfaceRegistrationPolicy, proxyInterfaces, priority)
        {
            
        }

        override public void GetProxyInterfaces(Type implType, ICollection<Type> toInterfaces)
        {
            base.GetProxyInterfaces(implType, toInterfaces);

            var fromInterfaces = implType.GetInterfaces();

            FindAndAddProxyType(implType, typeof(IObjectPool<>), fromInterfaces, toInterfaces);
            FindAndAddProxyType(implType, typeof(IObjectFactory<>), fromInterfaces, toInterfaces);

            Type uopType = FindUnityObjectPoolType(implType);
            if(uopType != null && !toInterfaces.Contains(uopType)) {
                toInterfaces.Add(uopType);
            }
        }

        private Type FindUnityObjectPoolType(Type type)
        {
            if(type == null || type == typeof(object)) {
                return null;
            }

            if (!type.IsGenericType) {
                return FindUnityObjectPoolType(type.BaseType);
            }                

            if(!typeof(UnityObjectPool<>).IsAssignableFrom(type.GetGenericTypeDefinition())) {
                return FindUnityObjectPoolType(type.BaseType);
            }

            if(type.ContainsGenericParameters) {
                return FindUnityObjectPoolType(type.BaseType);
            }

            return type;
        }

        private void FindAndAddProxyType(Type implType, Type genericType, Type[] searchTypes, ICollection<Type> toInterfaces)
        {
            var iFound = Array.Find(searchTypes, intf =>
            {
                return intf.IsGenericType && !intf.ContainsGenericParameters
                           && genericType.IsAssignableFrom(intf.GetGenericTypeDefinition());
            });

            if (iFound == null)
            {
#if UNITY_EDITOR || DEBUG_UNSTRIP
                Debug.LogWarning("RegisterEntityStore is applied to type "
                                 + implType.FullName + " which doesn't implement "
                                 + genericType.FullName);
#endif
                return;
            }

            var genArgs = iFound.GetGenericArguments();
            if (genArgs == null || genArgs.Length != 1)
            {

#if UNITY_EDITOR || DEBUG_UNSTRIP
                Debug.LogWarning("RegisterEntityStore is applied to type "
                                 + implType.FullName + " but "
                                 + genericType.FullName
                                 + " should be a single param generic type");
#endif
                return;
            }

            if (!toInterfaces.Contains(iFound))
            {
                toInterfaces.Add(iFound);
            }
        }

	}
}


