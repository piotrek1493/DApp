using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<AppUser>> GetUsersAsync(CancellationToken cancellationToken);
        Task<AppUser> GetUserByIdAsync(int id, CancellationToken cancellationToken);
        Task<AppUser> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams, CancellationToken cancellationToken);
        Task<MemberDto> GetMemberAsync(string username, CancellationToken cancellationToken);
    }
}
