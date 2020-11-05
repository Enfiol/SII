using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SII
{
    public class UserMark
    {
        [Key]
        public int Id { get; set; }
        public Lection LinkedLection { get; set; }
        public User LinkedUser { get; set; }
    }
}
