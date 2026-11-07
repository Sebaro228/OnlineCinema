using AutoMapper;
using Domain.DTOs;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ProfileService(UserManager<User> manager, IMapper mapper)
    {
        public async Task<UserDto> GetProfileAsync(string id)
        {
            var user = await manager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            var userDto = mapper.Map<UserDto>(user);
            userDto.Role = (await manager.GetRolesAsync(user)).First();
            return userDto;
        }
        public async Task UpdateProfileAsync(string id, UpdateProfile updateProfileDTO)
        {
            var user = await manager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            user.UserName = updateProfileDTO.UserName;
            user.Avatar = updateProfileDTO.Avatar;
            await manager.UpdateAsync(user);
        }
        public async Task<(bool succes, string error)> ChangePassword(string id, ChangePassword changePasswordDTO)
        {
            var user = await manager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            var res = await manager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            if (res.Succeeded) return (true, null);
            var error = string.Join(", ", res.Errors.Select(x => x.Description));
            return (false, error);
        }
        public async Task<(bool succes, string error)> DeleteProfile(string id, DeleteProfile deleteProfileDTO)
        {
            var user = await manager.FindByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            if (!await manager.CheckPasswordAsync(user, deleteProfileDTO.Password)) return (false, "Incorrect password");
            if (deleteProfileDTO.UserName != user.UserName) return (false, "Incorrect username");
            if (deleteProfileDTO.Email != user.Email) return (false, "Incorrect email");
            var res = await manager.DeleteAsync(user);
            if (res.Succeeded) return (true, null);
            var error = string.Join(", ", res.Errors.Select(x => x.Description));
            return (false, error);
        }
    }
}
