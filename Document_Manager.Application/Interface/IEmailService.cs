using System;
using System.Collections.Generic;
using System.Text;

namespace Document_Manager.Application.Interface
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
    }
}
