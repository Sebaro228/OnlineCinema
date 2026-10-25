using DAL.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService : Service<User>
    {
        public UserService(IRepository<User> rep) : base(rep) { }
    }
}
