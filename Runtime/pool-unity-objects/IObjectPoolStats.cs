
namespace BeatThat.Pools.UnityObjects
{
	public interface IObjectPoolStats<T>
	{
		void ReleaseCalled(T obj);
	}
}
		
