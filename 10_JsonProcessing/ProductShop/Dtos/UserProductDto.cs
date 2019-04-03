using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Dtos
{
    public class UserProductDto
    {
        public UserProductDto()
        {
            Users = new List<UserDto>();
        }
        [AutoMapper.IgnoreMap]

        public int UsersCount => Users.Count;

        public List<UserDto> Users { get; set; }
    }
}
