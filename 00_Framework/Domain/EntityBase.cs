using System;

namespace _00_Framework.Domain
{
    /// <summary>
    /// avoid of repeating common methods and properties
    /// </summary>
    public class EntityBase
    {
        public long Id { get; private set; }
        public DateTime CreationDate { get; private set; }

        public EntityBase()
        {
            
            CreationDate = DateTime.Now;
        }
    }
}
