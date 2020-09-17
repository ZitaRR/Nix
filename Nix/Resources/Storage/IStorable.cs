using LiteDB;
using System;

namespace Nix.Resources
{
    public interface IStorable
    {
        [BsonId]
        public int ID { get; }
        public DateTime CreatedAt { get; }
    }
}
