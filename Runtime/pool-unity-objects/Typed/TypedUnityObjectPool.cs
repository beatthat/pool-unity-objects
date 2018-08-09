using UnityEngine;

namespace BeatThat.Pools.UnityObjects
{
    /// <summary>
    /// Base class for an TypedObjectPool that is a Unity Component configured
    /// with a prefab prototype of the pool's object and a typeid, that is generally an enum.
    /// 
    /// This class is useful for building a GenericMultitypeObjectPool where that main pool
    /// has a collection of TTTypedObjectPool children, configured in the unity editor
    /// each supporting a speficic subtype for the parent pool.
    /// 
    /// EXAMPLE:
    /// 
    /// 	Parent pool manages objects of type Explosion.
    /// 
    /// 	The enum type is ExplosionType { SMALL, MED, LARGE }.
    /// 
    /// 	A simple class ExplosionTypePool is defined as follows
    /// 
    /// 		public class ExplosionTypePool 
    /// 			: TTTypedObjectPool<Explosion, ExplositonType> 
    /// 		{
    /// 		}
    /// 
    /// 
    /// 	...and there will be one ExplosionTypePool configured in unity for each of
    /// 	ExplosionType.SMALL, ExplosionType.MED, and ExplosionType.LARGE.
    /// 
    /// </summary>
    public class TypedUnityObjectPool<T, TypeT>
        : UnityObjectPool<T>, ITypedObjectPool<T>
            where T : Component, IPoolable
            where TypeT : struct, System.IConvertible
    {
        public TypeT m_type;

        public int objectTypeId
        {
            get
            {
                return m_type.ToInt32(FORMAT);
            }
        }

        private static System.Globalization.NumberFormatInfo FORMAT =
            new System.Globalization.NumberFormatInfo();

    }
}