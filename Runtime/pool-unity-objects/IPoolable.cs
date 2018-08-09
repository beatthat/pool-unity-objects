using System;
using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{

    public interface IPoolable
    {
        /// <summary>
        /// The action that gets executed on release.
        /// Generally you should NOT set this method.
        /// Typically its set by the pool that created/manages the object
        /// and called within Release (to return the object to the pool).
        /// </summary>
        /// <value>The release poolable delegate.</value>
        Action releasePoolableDelegate
        {
            get; set;
        }

        /// <summary>
        /// When done using a poolable, call this to release it.
        /// If the object belongs to a pool, 
        /// this should typically return the object to its pool.
        /// If the object does not belong to a pool, 
        /// this should typically destroy the object.
        /// </summary>
        void ReleasePoolable();

        /// <summary>
        /// Called when an object is taken from the pool. 
        /// </summary>
        void OnPoolTake();

        /// <summary>
        /// Called on a poolable when it is created at pool start up 
        /// to 'warm' the pool.
        /// Pool warming typically should happen under conditions
        /// where a frame can do expensive ops, e.g. at app startup 
        /// while the loading screen is still showing.
        /// </summary>
        void OnPoolWarm();

        /// <summary>
        /// Called on a poolable when it is returned to its pool
        /// </summary>
        void OnPoolReturn();

    }

    namespace PoolableExt
    {
        public static class Ext
        {

            public static void ReleaseOrDestroy(this IPoolable p)
            {
                if (p == null)
                {
                    return;
                }

                if (p.releasePoolableDelegate != null)
                {
                    p.releasePoolableDelegate();
                    return;
                }

                var c = p as Component;
                if (c == null)
                {
                    return;
                }

                GameObject go = c.gameObject;
                if (go != null)
                {
                    UnityEngine.Object.Destroy(go);
                }
            }

            public static void DestroyPoolable(this IPoolable p)
            {
                if (p == null)
                {
                    return;
                }

                p.releasePoolableDelegate = null;
                p.ReleasePoolable();
            }

        }
    }
}

