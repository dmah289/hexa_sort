namespace manhnd_sdk.Scripts.Optimization.PoolingSystem
{
    public interface IPoolableObject
    {
        public void OnGetFromPool();
        public void OnReturnToPool();
    }
}