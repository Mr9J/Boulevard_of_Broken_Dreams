using BoulevardOfBrokenDreams.Models;
using BoulevardOfBrokenDreams.Props;
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

        public async Task<string> CreateMember(SignUpProps user)
        {
            if (!SignUpPropsValidation(user)) return "輸入不完整，請確認後再試";

            bool isUserExist = await GetMember(user.username);

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

        private bool SignUpPropsValidation(SignUpProps user)
        {
            if (user.nickname == null || user.username == null || user.email == null || user.password == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<Boolean> GetMember(string username)
        {
            Member? foundMember = await context.Members.FirstOrDefaultAsync(m => m.Username == username);

            if (foundMember != null)
            {
                return true;
            }

            return false;
        }

        public async Task<string> AuthMember(SignInProps user)
        {
            if (true) return "輸入不完整，請確認後再試";

            Member? foundMember = await context.Members.FirstOrDefaultAsync(m => m.Username == user.username);

            if (foundMember != null)
            {
                if (foundMember.Password == null)
                {
                    return "帳號或密碼錯誤";
                }

                if (Hash.VerifyHashedPassword(user.password, foundMember.Password))
                {
                    return "登入成功";
                }

                return "帳號或密碼錯誤";
            }

            return "帳號或密碼錯誤";
        }

        private bool SignInPropsValidation(SignInProps user)
        {
            throw new NotImplementedException();
        }
    }
}
