using System.Collections.Generic;

namespace DestroyIt
{
    public static class ListExtensions
    {
        public static void Move<T>(this IList<T> list, int indexToMove, MoveDirection direction)
        {
            if (direction == MoveDirection.Up && indexToMove > 0)
            {
                var old = list[indexToMove - 1];
                list[indexToMove - 1] = list[indexToMove];
                list[indexToMove] = old;
            }
            else if (direction == MoveDirection.Down && indexToMove < list.Count -1)
            {
                var old = list[indexToMove + 1];
                list[indexToMove + 1] = list[indexToMove];
                list[indexToMove] = old;
            }
        }
    }
}