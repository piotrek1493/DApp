using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _dbContext;

    public LikesRepository(DataContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserLike> GetUserLikeAsync(int sourceUserId, int targetUserId, CancellationToken cancellationToken)
    {
        return await _dbContext.Likes.FindAsync(new object[] { sourceUserId, targetUserId }, cancellationToken);
    }

    public async Task<PagedList<LikeDto>> GetUserLikesAsync(LikesParams likesParams, CancellationToken cancellationToken)
    {
        var users = _dbContext.Users.OrderBy(u => u.UserName).AsQueryable();
        var likes = _dbContext.Likes.AsQueryable();

        if (likesParams.Predicate == "liked")
        {
            likes = likes.Where(l => l.SourceUserId == likesParams.UserId);
            users = likes.Select(l => l.TargetUser);
        }

        if (likesParams.Predicate == "likedBy")
        {
            likes = likes.Where(l => l.TargetUserId == likesParams.UserId);
            users = likes.Select(l => l.SourceUser);
        }

        var likedUsers = users.Select(user => new LikeDto
        {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize,
            cancellationToken);
    }

    public async Task<AppUser> GetUserWithLikesAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .Include(x => x.LikedUsers)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }
}