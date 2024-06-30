using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface ILikesRepository
{
    Task<UserLike> GetUserLikeAsync(int sourceUserId, int targetUserId, CancellationToken cancellationToken);
    Task<AppUser> GetUserWithLikesAsync(int userId, CancellationToken cancellationToken);
    Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams, CancellationToken cancellationToken);
}