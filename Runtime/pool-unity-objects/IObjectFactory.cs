using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
	/// <summary>
	/// Most basic interface for a factory that creates an object of some generic type
	/// </summary>
	public interface IObjectFactory<T> 
// annoying webplayer generics bug: anyone who implements or uses this interface cannot have a more constrained definition of T
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
		where T : Component, Poolable
#else
		where T : class
#endif
	{
			
		T GetInstance();
			
	}
}

