public interface IPoolable
{
    public void Initialize();

    public void OnRetrievedFromPool();

    public void OnReturnedToPool();
}