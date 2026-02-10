using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Models.DTOs
{
  public class RegisterDto
  {
    
    string UserName { set; get; }
    string Email { set; get; }
    string Password { set; get; }
    string Phone { set; get; }
  }
}