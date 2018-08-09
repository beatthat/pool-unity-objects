using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
    /// <summary>
    /// Base class for an ObjectPool that is a Unity Component configured
    /// with a prefab prototype of the pool's object.
    /// </summary>
    public class UnityObjectPool<T> : MonoBehaviour, IObjectPool<T>
        where T : Component, IPoolable
    {

        public bool m_disablePooling = false;
        public bool m_logPoolStats = false;

        public int m_warmCount = 0;

        public T m_prefab;

        /// <summary>
        /// Prototype of the pool's object
        /// </summary>
        public T prefab
        {
            get
            {
                return m_prefab;
            }
        }


        void Start()
        {
            Warm();
        }

        private void Warm()
        {
            if (m_warmCount < 1)
            {
                return;
            }

            using (var items = ListPool<IPoolable>.Get())
            {
                for (var i = 0; i < m_warmCount; i++)
                {
                    items.Add(GetInstance());
                }
                foreach (var p in items)
                {
                    p.OnPoolWarm();
                    p.ReleasePoolable();
                }
            }
        }

        public void DestroyPool()
        {
            GetObjectPool().DestroyPool();
            m_objectPool = null;
        }

        public int numNewCalls
        {
            get
            {
                return GetObjectPool().numNewCalls;
            }
        }

        public int numGetCalls
        {
            get
            {
                return GetObjectPool().numGetCalls;
            }
        }

        public int numReuseGetCalls
        {
            get
            {
                return GetObjectPool().numReuseGetCalls;
            }
        }

        public bool disablePooling
        {
            get
            {
                return GetObjectPool().disablePooling;
            }
            set
            {
                GetObjectPool().disablePooling = value;
            }
        }

        // non-virtual because IOS cannot handle virtual generic methods
        public T GetInstance()
        {
            object inst = GetObjectPool().GetInstance();
            InitInstance(inst);
            return inst as T;
        }

        // param is object instead of generic T because IOS cannot handle virtual generic methods
        virtual protected void InitInstance(object o)
        {
        }

        protected ObjectPool<T> GetObjectPool()
        {
            if (m_objectPool == null)
            {
                var objectFactory = new PrototypeObjectFactory<T>(this.prefab);

                var objectRetainer = new UnityObjectRetainer<T>(this.transform);

                var objectPool = new ObjectPool<T>(objectFactory, objectRetainer);

                objectPool.disablePooling = m_disablePooling;
                objectPool.logPoolStats = m_logPoolStats;

                m_objectPool = objectPool;
            }

            return m_objectPool;
        }


        private ObjectPool<T> m_objectPool;
    }

}
