using System.Collections.Generic;
using UnityEngine;

namespace Datatypes.Networking
{
    public struct Position
    {
        public Vector2 position;
		public double timestamp;
    }

	public class TruncatableSortedList<TKey, TValue> : SortedList<TKey, TValue>
	{
		/**
		 * Remove every entry in the list before the given index.
		 */
		public void RemoveUntilIndex (int index)
		{
			for (int i = 0; i < index; i++)
			{
				// Don't forget it's a sorted list, so removing 0 bumps everything else back!
				RemoveAt(0);
			}
		}
	}
}
