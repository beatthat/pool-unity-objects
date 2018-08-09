using System.Collections.Generic;
using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
	/// <summary>
	/// Provides a single point of access for managing
	/// objects with a common type and a multiplexable subtype.
	/// 
	/// EXAMPLE: If a MultitypeObjectPool handles type 'Explosion',
	/// then the objectTypeId's for subpools might be ExplosionType.SMALL ( = 1),
	/// and ExplosionType.LARGE ( = 2)
	/// 
	/// This implementation works by managing a set of service TypedObjectPool,
	/// each of which is responsible for managing objects of a unique subtype.
	/// </summary>
	public class MultitypeObjectPoolImpl<T> : IMultitypeObjectPool<T>
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
			where T : Component, Poolable
#else
			where T : class, IPoolable
#endif
	{
		/// <summary>
		/// Initialize the pool by searching for TypedObjectPool children of the passed-in transform.
		/// </summary>
		/// <param name='poolRoot'>
		/// Transform should be the parent of all TypedObjectPool service pools that support this pool.
		/// </param>
		public MultitypeObjectPoolImpl(Transform poolRoot)
			: this(poolRoot.GetComponentsInChildren<ITypedObjectPool<T>>())
		{
		}
		
		/// <summary>
		/// Initialize the pool with validation = TRUE as default.
		/// </summary>
		public MultitypeObjectPoolImpl(ITypedObjectPool<T>[] pools) : this(pools, true)
		{
		}
		
		/// <summary>
		/// Inits pool with an array of service pools and an option to disable validation.
		/// If validation is active, then a System.Exception will be thrown if
		/// two service pools handle the same TypedObjectPool::objectTypeId.
		/// </summary>
		/// <param name='pools'>
		/// The array of service pools.
		/// </param>
		/// <param name='validate'>
		/// If TRUE, performs validation, if FALSE, skips validation. 
		/// Typically better to leave validation on and fail fast.
		/// </param>
		public MultitypeObjectPoolImpl(ITypedObjectPool<T>[] pools, bool validate)
		{
			if(validate) {
				ValidateUniqueObjectTypeIds(pools);
			}	
			this.servicePools = pools;
		}
		
		public T GetInstance(int objectTypeId)
		{
			ITypedObjectPool<T> pool = GetPool(objectTypeId);
			
			if(pool != null) {
				return pool.GetInstance();
			}
			else {
				Debug.LogWarning("[" + Time.time + "] " + GetType() 
					+ "::GetInstance NO SERVICE TYPE FOUND FOR TYPE ID: " + objectTypeId);
				
				return null;
			}
		}
		
		public IEnumerable<int> supportedTypeIds
		{
			get {
				foreach(ITypedObjectPool<T> pool in this.servicePools) {
					yield return pool.objectTypeId;
				}
			}
		}
		
		public int numSupportedTypes
		{
			get {
				return this.servicePools.Length;
			}
		}
		
		protected ITypedObjectPool<T> GetPool(int objectTypeId)
		{
			foreach(ITypedObjectPool<T> p in this.servicePools) {
				if(p.objectTypeId == objectTypeId) {
					return p;
				}
			}
			
			Debug.LogWarning("[" + Time.time + "] " + GetType() 
					+ "::GetInstance NO SERVICE POOL FOUND FOR TYPE ID: " + objectTypeId);
			
			return null;
		}
		
		protected ITypedObjectPool<T>[] servicePools
		{
			get; private set;
		}
		
		private void ValidateUniqueObjectTypeIds(ITypedObjectPool<T>[] pools)
		{
			Dictionary<string, ITypedObjectPool<T>> workspace 
				= new Dictionary<string, ITypedObjectPool<T>>();
			
			foreach(ITypedObjectPool<T> p in pools) {
				ITypedObjectPool<T> duplicate;
				if(workspace.TryGetValue(p.objectTypeId.ToString(), out duplicate)) {
					throw new System.Exception("Illegal duplication of objectTypeId "
						+ p.objectTypeId + " in pools " + p.name + " and " + duplicate.name);
				}
			}
		}
	}
}

