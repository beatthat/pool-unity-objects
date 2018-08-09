using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
	/// <summary>
	/// Basic interface for an object pool of a single object type.
	/// In terms of usage, ObjectPool presents the same GetInstance() function as ObjectFactory
	/// but adds the additional constraint that pooled objects must implement the Poolable interface
	/// </summary>
	public interface IObjectPool<T> : IObjectFactory<T>
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
where T : Component, Poolable
#else
where T : class
#endif
	{
		/// <summary>
		/// Stat: how many times has a new instance been created (as opposed to a pooled object being reused)
		/// </summary>
		int numNewCalls
		{
			get;
		}
		
		/// <summary>
		/// Stat: how many times has an instance been requested via GetInstance()
		/// </summary>
		int numGetCalls
		{
			get;
		}
		
		/// <summary>
		/// Stat: how many times has GetInstance() returned a previously existing pooled object
		/// </summary>
		int numReuseGetCalls
		{
			get;
		}
		
		/// <summary>
		/// Destroy the pool and cleanup all pooled instances and other resources.
		/// </summary>
		void DestroyPool();
	}
}

