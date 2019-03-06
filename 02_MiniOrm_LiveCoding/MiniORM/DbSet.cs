namespace MiniORM
{
    using System.Collections.Generic;
    public class DbSet<T>
    {
        public List<T> Entities { get; set; }
    }
}