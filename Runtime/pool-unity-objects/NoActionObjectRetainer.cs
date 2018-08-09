namespace BeatThat.Pools.UnityObjects
{

    /// <summary>
    /// Do-nothing implementation of ObjectRetainer.
    /// Suitable for most ObjectPools of POCO types
    /// because the objects are retained by the ObjectPool's reference
    /// and there are no other unity parenting or positioning issues associated with retention.
    /// </summary>
    public class NoActionObjectRetainer<T> : IObjectRetainer<T> 
		// annoying webplayer generics bug: anyone who implements or uses this interface cannot have a more constrained definition of T
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
		where T : Component, Poolable
#endif
	{
		public NoActionObjectRetainer()
		{
		}
		
		public void Retain(T o)
		{
		}
		
		public void Release(T o)
		{
		}
		
	}
}

	
