using UnityEngine;

namespace Svelto.Factories
{
	public interface IMonoBehaviourFactory
	{
		M Build<M>(System.Func<M> constructor) where M:MonoBehaviour;
	}
}

