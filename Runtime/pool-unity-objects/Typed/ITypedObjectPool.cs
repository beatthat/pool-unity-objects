namespace BeatThat.Pools.UnityObjects
{
    /// <summary>
    /// Extension of ObjectPool that can participate within a MultitypedObjectPool
    /// by declaring an objectTypeId for the object type that this pool manages.
    /// </summary>
    public interface ITypedObjectPool<T> : IObjectPool<T> // annoying webplayer generics bug: anyone who implements or uses this interface cannot have a more constrained definition of T
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
where T : Component, Poolable
#else
where T : class
#endif
	{
		/// <summary>
		/// Name identifier for this pool. 
		/// Typically pools are MonoBehaviours and name is the unity object name
		/// </summary>
		string name
		{
			get;
		}
		
		/// <summary>
		/// Type id for the objects that this pool manages.
		/// Must be unique *AT LEAST* within the space of this pools siblings
		/// within a parent MultiplexObjectPool.
		/// 
		/// EXAMPLE: If a MultiplexObjectPool handles type 'Explosion',
		/// then the objectTypeId's for subpools might be ExplosionType.SMALL ( = 1),
		/// and ExplosionType.LARGE ( = 2)
		/// 
		/// Frequently objectTypeId can derive from an enum, which would enable
		/// unity-edit configuration.
		/// </summary>
		/// <value>
		/// The object type identifier.
		/// </value>
		int objectTypeId
		{
			get;
		}
	}
}

