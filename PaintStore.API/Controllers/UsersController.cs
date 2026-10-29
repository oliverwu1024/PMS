
using Microsoft.AspNetCore.Mvc;

using PaintStore.API.Dtos;

using PaintStore.API.Services;



namespace PaintStore.API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateRequest request) 
    {



        var user = await _userService.CreateAsync(request);

        return Ok(user.ToDto());

    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUser(int id) 
    {
        //假设从数据库获取了一个用户实体
        var user = await _userService.GetByIdAsync(id);

        //将user转换为UserDTO并返回

        return Ok(user.ToDto());





    }






}
