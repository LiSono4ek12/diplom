using Api.Entities;

namespace Api.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<ApplicationUser?> GetByIdAsync(Guid id);
    Task<ApplicationUser?> GetByResetTokenAsync(string token);
    Task AddAsync(ApplicationUser user);
    Task UpdateAsync(ApplicationUser user);
}