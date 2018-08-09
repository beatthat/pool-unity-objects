using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
	/// <summary>
	/// Generally used to retain pooled objects, particularly when those objects
	/// are UnityEngine.Components. 
	/// 
	/// When a Unity object is passivated and held in a pool, 
	/// you frequently want that object to move under ObjectPool parent's Transform
	/// so that it doesn't pollute the root space
	/// and also maybe so that it's far away from any camera.
	/// ObjectRetainer's responsibility it to manage this problem of parenting and location
	/// of pooled objects while in their passive state.
	/// </summary>
	public interface IObjectRetainer<T>
		// annoying webplayer generics bug: anyone who implements or uses this interface cannot have a more constrained definition of T
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
		where T : Component, Poolable
#endif
	{
		/// <summary>
		/// Retain the object. In the ObjectPool context, 
		/// this is called when an object is returned to the pool.
		/// </summary>
		void Retain(T o);
		
		/// <summary>
		/// Release the object. In the ObjectPool context, 
		/// this is called when an object is checked out of the pool.
		/// </summary>
		void Release(T o);
	}

}
