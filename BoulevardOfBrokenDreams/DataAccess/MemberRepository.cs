using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Models.DTO;
using BoulevardOfBrokenDreams.Services;
using Microsoft.EntityFrameworkCore;

namespace BoulevardOfBrokenDreams.DataAccess
{
    public class MemberRepository
    {
        private MumuDbContext context;
        public MemberRepository(MumuDbContext _context)
        {
            this.context = _context;
        }

        public async Task<string> CreateMember(SignUpDTO user)
        {
            if (!SignUpPropsValidation(user)) return "輸入不完整，請確認後再試";

            bool isUserExist = await IsMemberExist(user.username);

            if (!isUserExist)
            {
                Member member = new Member();

                member.Nickname = user.nickname;
                member.Username = user.username;
                member.Email = user.email;
                member.RegistrationTime = DateTime.UtcNow;

                string hashedPassword = Hash.HashPassword(user.password);

                member.Password = hashedPassword;

                context.Members.Add(member);
                await context.SaveChangesAsync();

                return "註冊成功";
            }
            else
            {
                return "使用者已存在";
            }
        }

        private bool SignUpPropsValidation(SignUpDTO user)
        {
            if (string.IsNullOrEmpty(user.nickname) ||
                string.IsNullOrEmpty(user.username) ||
                string.IsNullOrEmpty(user.email) ||
                string.IsNullOrEmpty(user.password))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<Boolean> IsMemberExist(string username)
        {
            Member? foundMember = await context.Members.FirstOrDefaultAsync(m => m.Username == username);

            if (foundMember != null)
            {
                return true;
            }

            return false;
        }
        
        public async Task<Member?> GetMember(string username)
        {
            Member? foundMember = await context.Members.FirstOrDefaultAsync(m => m.Username == username);

            return foundMember;
        }

        public async Task<Member?> AuthMember(SignInDTO user)
        {
            if (!SignInPropsValidation(user)) return null;

            Member? foundMember = await context.Members.FirstOrDefaultAsync(m => m.Username == user.username);

            if (foundMember != null)
            {
                if (foundMember.Password == null)
                {
                    return null;
                }

                if (Hash.VerifyHashedPassword(user.password, foundMember.Password))
                {
                    return foundMember;
                }

                return null;
            }

            return null;
        }

        private bool SignInPropsValidation(SignInDTO user)
        {
            if (string.IsNullOrEmpty(user.username) ||
                string.IsNullOrEmpty(user.password))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
