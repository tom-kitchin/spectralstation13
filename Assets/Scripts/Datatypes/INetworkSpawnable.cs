using UnityEngine.Networking;

namespace Datatypes.Config
{
    public interface INetworkSpawnable
    {
        NetworkHash128 assetId { get; }
    }
}
