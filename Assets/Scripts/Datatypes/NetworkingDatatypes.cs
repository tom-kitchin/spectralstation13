using System.Collections.Generic;
using UnityEngine;

namespace Datatypes.Networking
{
    public struct Position
    {
        public Vector2 position;
        public double timestamp;
    }

    public class TimestampedList<TValue> : SortedList<double, TValue>
    {
        /**
         * Get the most recent entry in the list, i.e., with highest value timestamp.
         */
        public TValue Last {
            get {
                return Values[Count - 1];
            }
        }

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

        /**
		 * Remove outdated entries.
		 * Entries yet to come are useful, the entry just before the given time is useful for reference, 
		 * but anything before that is pointless.
		 */
        public void RemoveOutdatedTimestamps (double timestamp)
        {
            RemoveUntilIndex(GetIndexBeforeTimestamp(timestamp));
        }

        /**
		 * Get the index of the entry timestamped immediately after the given timestamp.
		 * Returns 0 if the given timestamp is earlier than all entries.
		 * Returns -1 if the given timestamp is later than all entries.
		 */
        public int GetIndexAfterTimestamp (double timestamp)
        {
            for (int i = 0; i < Count; i++)
            {
                if (Keys[i] > timestamp)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
		 * Get the index of the entry timestamped immediately before the given timestamp.
		 * Returns -1 if the given timestamp is earlier than all entries.
		 * Returns the index of the last entry if the given timestamp is later than all entries.
		 */
        public int GetIndexBeforeTimestamp (double timestamp)
        {
            int index = GetIndexAfterTimestamp(timestamp);
            if (index < 0)
            {
                return Count - 1;
            }
            return index - 1;
        }

        public TValue GetByIndex (int index)
        {
            return Values[index];
        }
    }
}
