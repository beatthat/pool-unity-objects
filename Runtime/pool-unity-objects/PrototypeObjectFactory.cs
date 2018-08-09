using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
    /// <summary>
    /// Factory that creates objects from a prototype instance.
    /// The prototype is generally a prefab, but not an absolute requirement.	
    /// </summary>
    public class PrototypeObjectFactory<T> : IObjectFactory<T> 
		where T : Component, IPoolable
	{

		/// <summary>
		/// Inits the factory with the prototype for all instances.
		/// </summary>
		/// <param name='proto'>
		/// The prototype *MUST* either be a UnityEngine.Component 
		/// or otherwise a POCO with a zero-arg constructor
		/// </param>
		public PrototypeObjectFactory(T proto)
		{
			m_prototype = proto;
		}
		
		// non-virtual because IOS cannot handle virtual generic methods
		/// <summary>
		/// Creates a new instance of the prototype object.
		/// This implementation is non virtual because of IOS issues 
		/// with virtual methods that have generic return type.
		/// If the new object requires configuration beyond what can be copied from the prototype,
		/// override InitInstance(object) for that purpose.
		/// </summary>
		/// <returns>
		/// A new copy of the factory prototype.
		/// </returns>
		public T GetInstance() {
			T inst = NewInstance();
			
			InitInstance(inst);
			
			return inst;
		}
		
		/// <summary>
		/// Override this template method to configure a new instance
		/// beyond what can be copied from the prototype.
		/// </summary>
		/// 
		
		/// <summary> 
		/// Override this template method to configure a new instance
		/// beyond what can be copied from the prototype.
		/// </summary>
		/// <param name='o'>
		/// The new instance of the object to configure/init.
		/// This method uses param type 'object' rather than the generic type
		/// because of IOS issues with virtual methods and generic types,
		/// so downcast is required.
		/// </param>
		virtual protected void InitInstance(object o)
		{
		}
			
		
		/// <summary>
		/// Meat of the instantiation code. 
		/// Uses Unity instantiation for Component types and zero-arg constructor for POCO types.
		/// Cannot be overriden because of IOS problems with virtual functions and generics.
		/// </summary>
		/// <returns>
		/// The instance.
		/// </returns>
		protected T NewInstance() 
		{
			if(m_prototype is Component)
			{
				return ((GameObject)GameObject.Instantiate((m_prototype as Component).gameObject)).GetComponent(typeof(T)) as T;
			}
			else
			{
				if(m_constructor == null)
				{
					m_constructor = m_prototype.GetType().GetConstructor(new System.Type[] {});
				}
				return m_constructor.Invoke(new object[] {}) as T;
			}
		}
			
		private System.Reflection.ConstructorInfo m_constructor;
		private T m_prototype;
	}

}
			
	
	
