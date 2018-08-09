using System.Collections.Generic;
using BeatThat.Pools.UnityObjects.PoolableExt;
using BeatThat.SafeRefs;
using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
    public class ObjectPool<T> : IObjectPool<T>, IObjectPoolStats<T>
#if (UNITY_WEBPLAYER || UNITY_WEBGL)
		where T : Component, Poolable
#else
		where T : class, IPoolable
#endif
	{
		
		public ObjectPool(IObjectFactory<T> objectFactory, IObjectRetainer<T> objectRetainer)
		{
			m_objectFactory = objectFactory;
			m_objectRetainer = objectRetainer;
		}
			
		virtual public void DestroyPool()
		{
	//		Debug.Log("[" + Time.time + "] " + GetType() + "::DestroyPool()");
			
			for(int i = m_objectPool.Count - 1; i >=0; i--)
			{
				PooledObject po = m_objectPool[i];
				if(po != null)
				{
					po.DestroyObject();
				}
				m_objectPool.RemoveAt(i);
			}
		}
		
		public bool disablePooling
		{
			get; set;
		}
		
		public bool logPoolStats
		{
			get; set;
		}
		
		public int numNewCalls
		{
			get
			{
				return m_statNewCount;
			}
		}
		
		public int numGetCalls
		{
			get
			{
				return m_statGetCount;
			}
		}
		
		public int numReuseGetCalls
		{
			get
			{
				return this.numGetCalls - this.numNewCalls;
			}
		}
			
		// non-virtual because IOS cannot handle virtual generic methods
		public T GetInstance() {
			T inst = null;
			if(!this.disablePooling) {
				inst = GetPooledInstance();
			}
			if(inst == null) {
				inst = NewInstance();
			}	
			
			InitInstance(inst);
			
			m_statGetCount++;
			
	//		Debug.Log("[" + Time.time + "] " + GetType() + "::GetInstance() m_statGetCount=" + m_statGetCount
	//		          + ", m_statNewCount=" + m_statNewCount);
			
			if(this.logPoolStats) {
				LogStats();
			}
				
			return inst;
		}
		
		// param is object instead of generic T because IOS cannot handle virtual generic methods
		virtual protected void InitInstance(object o)
		{
		}
			
		// non-virtual because IOS cannot handle virtual generic methods. Use ConfigureInstance for template overrides
		protected T NewInstance() {
			T inst = m_objectFactory.GetInstance();
			if(inst != null)
			{
				ConfigureNewInstance(inst);
			}
			
			m_statNewCount++;
			
			return inst;
		}
			
		// param is object instead of generic T because IOS cannot handle virtual generic methods
		virtual protected void ConfigureNewInstance(object o)
		{
		}
		
	    public void DeletePooledObjects() {
			lock(this) {
				for(int i = m_objectPool.Count - 1; i >= 0; i--)
				{
					PooledObject po = m_objectPool[i];
					m_objectPool.RemoveAt(i);
					var c = po.ClearObject() as Component;
                    if(c == null) {
                        continue;
                    }
                    Object.Destroy(c.gameObject);
				}
			}
		}
			
		public void ReleaseCalled(T obj)
		{
			m_statDeleteCount++;
			
			if(this.logPoolStats) {
				LogStats();
			}
		}
			
			
		private T GetPooledInstance() {
			List<PooledObject> pool = m_objectPool;
			lock(this) 
			{
				foreach(PooledObject po in pool) {
					if(po.TakeActivated()) {
						return po.obj;
					}
				}
					
				T inst = NewInstance();
				if(inst != null) {
					PooledObject po = new PooledObject(inst, false, m_objectRetainer, this);
					inst.releasePoolableDelegate = po.ReturnToPool;
					pool.Add(po);
					return inst;
				}
			}
			return null;
		}
			
		private void LogStats()
		{
			Debug.Log("[" + Time.time + "] " + GetType().ToString() + ": checkout-calls=" 
			          + m_statGetCount + ", delete-calls=" + m_statDeleteCount 
			          + ", instantiate-calls=" + m_statNewCount);
		}
		
		internal class PooledObject  
		{
			public PooledObject(
				T obj, bool free,
				IObjectRetainer<T> objectRetainer, 
				IObjectPoolStats<T> poolStats) 
			{
                m_object = new SafeRef<T>(obj);
				m_free = free;
				m_objectRetainer = objectRetainer;
				m_poolStats = poolStats;
			}
		
			public void ReturnToPool() 
			{
                var inst = m_object.value;
                if(inst == null) {
                    return;
                }

                inst.OnPoolReturn();
                m_objectRetainer.Retain(inst);
                m_poolStats.ReleaseCalled(inst);
				m_free = true;
			}
			
			public void DestroyObject() 
			{
				T tmp = ClearObject();
				
				if(tmp != null) {
					tmp.DestroyPoolable();
				}
			}
			
			public T obj 
			{
				get {
                    return m_object.value;
				}
			}
			
			public T ClearObject() {
				T tmp = m_object.value;
                m_object.value = null;
				return tmp;
			}
			
			public bool TakeActivated() 
			{
				lock(this) {
                    T inst;
                    if(m_free && (inst = m_object.value) != null) {
						m_free = false;
                        m_objectRetainer.Release(inst);
                        inst.OnPoolTake();
						return true;
					}
					else {
						return false;
					}
				}
			}
			
			public bool free 
			{
				get {
					return m_free;
				}
				set {
					m_free = value;
				}
			}
			
			private bool m_free;
			private SafeRef<T> m_object;
			private IObjectPoolStats<T> m_poolStats;
			private IObjectRetainer<T> m_objectRetainer;
		}
				
		private IObjectFactory<T> m_objectFactory;
		private IObjectRetainer<T> m_objectRetainer;
			
		private List<PooledObject> m_objectPool = new List<PooledObject>();
			
		private int m_statGetCount = 0;
		private int m_statDeleteCount = 0;
		private int m_statNewCount = 0;
		
		
		
		
		
	}
}

