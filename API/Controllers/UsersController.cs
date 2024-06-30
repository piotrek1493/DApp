using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public UsersController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
    {
        _uow = uow;
        _mapper = mapper;
        _photoService = photoService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery] UserParams userParams, CancellationToken cancellationToken)
    {
        var gender = await _uow.UserRepository.GetUserGender(User.GetUsername());
        userParams.CurrentUsername = User.GetUsername();

        if (string.IsNullOrEmpty(userParams.Gender))
        {
            userParams.Gender = gender == "male" ? "female" : "male";
        }

        var users = await _uow.UserRepository.GetMembersAsync(userParams, cancellationToken);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize,
            users.TotalCount, users.TotalPages));

        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser([FromRoute] string username, CancellationToken cancellationToken)
    {
        return Ok(await _uow.UserRepository.GetMemberAsync(username, cancellationToken));
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser([FromBody] MemberUpdateDto memberUpdateDto, CancellationToken cancellationToken)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        _mapper.Map(memberUpdateDto, user);

        if (await _uow.Complete()) return NoContent();

        return BadRequest($"Failed to update user with the id of '{user.Id}'.");
    }

    /// <summary>
    /// Adds a photo to current user
    /// </summary>
    /// <param name="file">The photo in .jpg or .png format</param>
    /// <returns>A JSON with created result</returns>
    [HttpPost("add-photo")]
    [ProducesResponseType(typeof(HttpStatusCode), 201)]
    public async Task<ActionResult> AddPhoto([FromBody] IFormFile file, CancellationToken cancellationToken)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var result = await _photoService.AddPhotoAsync(file, cancellationToken);

        if (result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        if (user.Photos.Count == 0) photo.IsMain = true;

        user.Photos.Add(photo);

        if (await _uow.Complete())
        {
            return CreatedAtAction(nameof(GetUser),
                new { username = user.UserName },
                _mapper.Map<PhotoDto>(photo));
        }

        return BadRequest("An error occured while adding photo.");
    }


    /// <summary>
    /// Sets chosen photo as user's main photo
    /// </summary>
    /// <param name="photoId">The id of the photo</param>
    [HttpPut("set-main-photo/{photoId}")]
    [ProducesResponseType(typeof(HttpStatusCode), 200)]
    public async Task<ActionResult> SetMainPhoto([FromRoute] int photoId, CancellationToken cancellationToken)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("This is already your main photo.");

        var currentMainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);
        if (currentMainPhoto != null) currentMainPhoto.IsMain = false;
        photo.IsMain = true;

        if (await _uow.Complete()) return NoContent();

        return BadRequest("An error occured setting the main photo.");
    }

    /// <summary>
    /// Deletes a photo
    /// </summary>
    /// <param name="photoId">The identifier of the photo</param>
    [HttpDelete("delete-photo/{photoId}")]
    [ProducesResponseType(typeof(HttpStatusCode), 204)]
    public async Task<ActionResult> DeletePhoto([FromRoute] int photoId, CancellationToken cancellationToken)
    {
        var user = await _uow.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if (user == null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

        if (photo == null) return NotFound();

        if (photo.IsMain) return BadRequest("You cannot delete your main photo. Please set a different photo as your main photo first.");

        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAsync(photo.PublicId, cancellationToken);
            if (result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _uow.Complete()) return NoContent();

        return BadRequest("Problem deleting photo.");
    }

}