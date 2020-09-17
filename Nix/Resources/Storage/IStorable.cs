using LiteDB;
using System;

namespace Nix.Resources
{
    public interface IStorable
    {
        [BsonId]
        public int ID { get; }
        public DateTime StoredAt { get; }
    }
}
