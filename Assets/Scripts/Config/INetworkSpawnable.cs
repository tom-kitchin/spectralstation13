using UnityEngine.Networking;

namespace Config
{
    public interface INetworkSpawnable
    {
        NetworkHash128 assetId { get; }
    }
}
