using System;
using UnityEngine;

namespace TicTacToe.Common
{
    public struct Sequence : IEquatable<Sequence>
    {
        public Vector2Int From;

        public Vector2Int To;

        public static bool operator ==(Sequence left, Sequence right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Sequence left, Sequence right)
        {
            return !left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            return obj is Sequence other && Equals(other);
        }

        public bool Equals(Sequence other)
        {
            return From.Equals(other.From) && To.Equals(other.To);
        }

        public override int GetHashCode()
        {
            return From.GetHashCode() ^ To.GetHashCode();
        }

        public override string ToString()
        {
            return $"Sequence[{From}, {To}]";
        }
    }
}
