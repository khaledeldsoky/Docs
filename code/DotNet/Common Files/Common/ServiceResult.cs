using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace App.Common
{
  public class ServiceResult
  {
    string Message {set; get;}
    bool Success {set; get;}

    public static ServiceResult OK(string msg)
    => new ServiceResult {Success = true , Message = msg};

    public static ServiceResult OK(string msg)
    => new ServiceResult { Success = true, Message = msg }; 
    
        
  }
}