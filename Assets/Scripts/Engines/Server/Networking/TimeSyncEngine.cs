using System.Collections;
using Svelto.ECS;
using Services.Networking;

namespace Engines.Server.Networking
{
	/**
	 * On the heartbeat, update everyone with server time.
	 */
	public class TimeSyncEngine : IEngine, ILateInitEngine
	{
		public void LateInit()
		{
			TaskRunner.Instance.Run(Heartbeat);
		}

		IEnumerator Heartbeat()
		{
			yield return SpectreConnectionConfig.heartbeatEnumerator;
			HeartbeatTick();
		}

		void HeartbeatTick()
		{
			SpectreServer.SyncTimeToAll();
		}
	}
}
