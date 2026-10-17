using System.Collections.Concurrent;
using System.Data.Common;
using System.Drawing;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc;
using PaintStore.Models.Enums;
using PaintStore.Models.Methods;
using PaintStore.API.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Reflection.Metadata;
using PaintStore.Models;
using PaintStore.API.Data;



namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    public UsersController(AppDbContext db) => _db = db;

    [HttpPost]
    public IActionResult CreateUser([FromBody] UserCreateRequest request) 
    {
        if (request == null) 
        {
            return BadRequest("User data is required");
        }

        //假设这里进行了一些业务逻辑, 比如保存到数据库
        var user = new User(request.UserName, request.UserEmail, request.UserPhone, request.Age, userId: 0);
        _db.Users.Add(user);
        _db.SaveChanges();

        return Ok(new UserDTO
        {
            UserId = user.UserId,
            UserName = user.UserName,
            UserEmail = user.UserEmail
        });
    }

    [HttpGet("{id}")]
    public ActionResult<UserDTO> GetUser(int id) 
    {
        //假设从数据库获取了一个用户实体
        var user = _db.Users.FirstOrDefault(u=>u.UserId == id);
        if(user is null) return NotFound();

        //将user转换为UserDTO并返回
        var userDto = new UserDTO 
        {
            UserId = user.UserId,
            UserName= user.UserName,
            UserEmail = user.UserEmail
        };

        return Ok(userDto);




    }






}
