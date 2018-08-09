using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
	/// <summary>
	/// Implementation of ObjectRetainer generally used for ObjectPools
	/// of UnityEngine::Component type objects.
	/// When an object is Retained, makes that object a child 
	/// of this retainer's Transform (generally the pool itself).
	/// </summary>
	public class UnityObjectRetainer<T> : IObjectRetainer<T>
		// annoying webplayer generics bug: anyone who implements or uses this interface cannot have a more constrained definition of T
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
		where T : Component, Poolable
#endif
	{
		/// <summary>
		/// Inits the ObjectRetainer with a parent transform for retained objects
		/// and the default moveRetainedObjectsToRetainerOrigin=FALSE
		/// </summary>
		public UnityObjectRetainer(Transform parent) : this(parent, false)
		{
		}
		
		/// <summary>
		/// Inits the ObjectRetainer with a parent transform for retained objects.
		/// </summary>
		/// <param name='parent'>
		/// Retained objects will be made child of this Transform, generally this transform is an ObjectPool
		/// </param>
		/// <param name='moveRetainedObjectsToRetainerOrigin'>
		/// If TRUE, then retained objects will be moved to the origin of this ObjectRetainer's
		/// 'parent' Transform. This allows pooled objects to be moved away from cameras and other action
		/// when they are passivated.
		/// </param>
		public UnityObjectRetainer(Transform parent, bool moveRetainedObjectsToRetainerOrigin)
		{
			m_parent = parent;
			m_moveRetainedObjectsToRetainerOrigin = moveRetainedObjectsToRetainerOrigin;
		}
		
		public void Retain(T o)
		{
            var c = o as Component;
            if(c == null) {
                return;
            }
			
			Transform objectTrans = c.transform;
            objectTrans.SetParent(m_parent, false);
			if(m_moveRetainedObjectsToRetainerOrigin) {
				objectTrans.transform.localPosition = Vector3.zero;
			}
		}
		
		public void Release(T o)
		{
            var c = o as Component;
            if (c == null)
            {
                return;
            }
            c.transform.SetParent(null, false);
		}
		
		private Transform m_parent;
		private bool m_moveRetainedObjectsToRetainerOrigin = true;
		
	}
}

