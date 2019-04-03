using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Dtos
{
    public class UserDto
    {
        public string LastName { get; set; }

        public int Age { get; set; }

        public ICollection<SoldProductDto> SoldProducts { get; set; }
    }
}
