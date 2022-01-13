using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdApi.Services
{
    public interface IValidationService
    {
        Task<bool> ValidateEmail(string emailId, int userId);
    }
}
