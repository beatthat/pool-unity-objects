namespace BeatThat.Pools.UnityObjects
{
    /// <summary>
    /// Provides a single point of access for managing
    /// objects with a common type and a multiplexable subtype.
    /// 
    /// EXAMPLE: If a MultitypeObjectPool handles type 'Explosion',
    /// then the objectTypeId's for subpools might be ExplosionType.SMALL ( = 1),
    /// and ExplosionType.LARGE ( = 2)
    /// </summary>
    public interface IMultitypeObjectPool<T> 
		where T : class, IPoolable
	{
		T GetInstance(int objectTypeId);
	}
}

