﻿using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using BoulevardOfBrokenDreams.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace BoulevardOfBrokenDreams.DataAccess
{
    public class MemberRepository
    {
        private readonly MumuDbContext _context;
        public MemberRepository(MumuDbContext context)
        {
            this._context = context;
        }

        public async Task<string> CreateMember(SignUpDTO user)
        {
            string validationRes = SignUpValidation(user);

            if (validationRes != string.Empty) { return "註冊資料格式錯誤"; }

            bool isUserExist = _context.Members.Any(m => m.Username == user.username);

            if(isUserExist) return "使用者已存在";

            bool isEmailExist = _context.Members.Any(m => m.Email == user.email);

            if(isEmailExist) return "Email已被使用";

            if (!isUserExist && !isEmailExist)
            {
                Member member = new Member
                {
                    Nickname = user.nickname,
                    Username = user.username,
                    Email = user.email,
                    RegistrationTime = DateTime.UtcNow,
                    Password = Hash.HashPassword(user.password),
                    Thumbnail = "https://cdn.mumumsit158.com/Members/User.jpg"
                };

                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                return "註冊成功";
            }
            else
            {
                return "使用者已存在";
            }
        }

        public async Task<Boolean> IsMemberExist(string username)
        {
            Member? foundMember = await _context.Members.FirstOrDefaultAsync(m => m.Username == username);

            if (foundMember != null)
            {
                return true;
            }

            return false;
        }

        public async Task<Member?> GetMember(string username)
        {
            Member? foundMember = await _context.Members.FirstOrDefaultAsync(m => m.Username == username);

            return foundMember;
        }

        public async Task<Member?> AuthMember(SignInDTO user)
        {
            if (!SignInPropsValidation(user)) return null;

            Member? foundMember = await _context.Members.FirstOrDefaultAsync(m => m.Username == user.username);

            if (foundMember != null)
            {
                if (Hash.VerifyHashedPassword(user.password, foundMember.Password!))
                {
                    return foundMember;
                }

                return null;
            }

            return null;
        }



        private string SignUpValidation(SignUpDTO user)
        {
            if (user.nickname.Length < 2 || user.nickname.Length > 50)
            {
                return "暱稱長度必須在2至50之間";
            }

            if (user.username.Length < 8 || user.username.Length > 24)
            {
                return "帳號長度必須在8至24之間";
            }

            if (!IsValidEmail(user.email))
            {
                return "請輸入正確的Email格式";
            }

            if (user.password.Length < 8 || user.password.Length > 24)
            {
                return "密碼長度必須在8至24之間";
            }

            if (!IsValidPassword(user.password))
            {
                return "密碼必須包含大小寫英文字母及數字";
            }

            return string.Empty;
        }

        private bool SignInPropsValidation(SignInDTO user)
        {
            return !string.IsNullOrEmpty(user.username) && !string.IsNullOrEmpty(user.password);
        }

        public bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(email, pattern);
        }

        public bool IsValidPassword(string password)
        {
            string pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[^a-zA-Z0-9]).{8,24}$";
            return Regex.IsMatch(password, pattern);
        }

        public async Task<Member?> GetMemberByEmail(string email)
        {
            Member? foundMember = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);

            return foundMember;
        }
    }
}
