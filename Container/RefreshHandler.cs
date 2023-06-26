using Dotnet7API.Repos;
using Dotnet7API.Repos.Models;
using Dotnet7API.Service;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Dotnet7API.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly LearndataContextb _context;

        public RefreshHandler(LearndataContextb context)
        {
            _context = context;
        }
        public async Task<string> GenerateToken(string username)
        {
            var randomNumber = new byte[32];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);
                var existToken = _context.TblRefreshTokens.FirstOrDefaultAsync(item => item.UserId == username).Result;
                if (existToken != null)
                {
                    existToken.RefreshToken = refreshToken;
                }
                else
                {
                    await _context.TblRefreshTokens.AddAsync(new TblRefreshToken
                    {
                        UserId = username,
                        TokenId = new Random().Next().ToString(),
                        RefreshToken = refreshToken,
                    });
                }
                await _context.SaveChangesAsync();

                return refreshToken;
            }
        }
    }
}
