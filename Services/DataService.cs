using BlogProj_12_10_22.Data;
using BlogProj_12_10_22.Enums;
using BlogProj_12_10_22.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BlogProj_12_10_22.Services
{
    public class DataService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        public DataService(ApplicationDbContext dbContext,
                           RoleManager<IdentityRole> roleManager,
                           UserManager<BlogUser> userManager,
                           IImageService imageService, 
                           IConfiguration configuration)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _imageService = imageService;
            _configuration = configuration;
        }

        public static string GetConnectionString(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };
            return builder.ToString();
        }

        public async Task ManageDataAsync() 
        {
            await _dbContext.Database.MigrateAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();

        }

        private async Task SeedRolesAsync()
        {
            if (_dbContext.Roles.Any())
            {
                return;
            }
            foreach (var role in Enum.GetNames(typeof(BlogRole)))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));

            }
        }

        private async Task SeedUsersAsync()
        {
            if (_dbContext.Users.Any())
            {
                return;
            }

            var adminUser = new BlogUser()
            {
                Email = "laurence.mccabe@gmail.com",
                UserName = "laurence.mccabe@gmail.com",
                FirstName = "Laurence",
                LastName = "Mccabe",
                PhoneNumber = "0444 44 4444",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["defaultUserImage"]),
                ContentType = Path.GetExtension(_configuration["defaultUserImage"]),

            };

            IdentityResult dbResult = await _userManager.CreateAsync(adminUser, "Password1!");

            if (dbResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, BlogRole.Administrator.ToString());

            }



            var modUser = new BlogUser()
            {
                Email = "johnsandwise@gmail.com",
                UserName = "johnsandwise@gmail.com",
                FirstName = "john",
                LastName = "sands",
                PhoneNumber = "03 3333 3333",
                EmailConfirmed = true,
                ImageData = await _imageService.EncodeImageAsync(_configuration["defaultUserImage"]),
                ContentType = Path.GetExtension(_configuration["defaultUserImage"]),
            };

            await _userManager.CreateAsync(modUser, "Password1!");

            await _userManager.AddToRoleAsync(modUser, BlogRole.Moderator.ToString());
        }

    }
}
