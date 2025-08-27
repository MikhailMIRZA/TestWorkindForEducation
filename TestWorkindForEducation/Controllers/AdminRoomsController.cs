using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;

namespace TestWorkindForEducation.WebAPI.Controllers.Admin;

[ApiController]                     // Помечаем что это контроллер API
[Route("api/admin/[controller]")]   // Базовый маршрут
public class AdminRoomsController : ControllerBase          // Базовый класс для API контроллеров
{
    private readonly IRoomService _roomService;             // Сервис для работы с номерами

    // Конструктор с внедрением зависимости сервиса
    public AdminRoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    // GET: api/admin/adminrooms - Получить все номера
    [HttpGet]
    public async Task<IActionResult> GetAllRooms()
    {
        var rooms = await _roomService.GetAllRoomsAsync();  // Вызов сервиса
        return Ok(rooms);                                   // Возвращаем 2OK с данными
    }

    // GET api/admin/adminrooms/5 - Получить номер по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(int id)
    {
        var room = await _roomService.GetRoomByIdAsync(id);
        if (room == null)                                   // Если номер не найден
        {
            return NotFound();                              // Возвращаем 404 Not Found
        }
        return Ok(room);                                    // Возвращаем OK с данными
    }

    // POST api/admin/adminrooms - Создать новый номер
    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDto createRoomDto)
    {
        if (!ModelState.IsValid)                            // Проверка валидации модели
        {
            return BadRequest(ModelState);                  // 400 Bad Request с ошибками
        }

        try
        {
            var createdRoom = await _roomService.CreateRoomAsync(createRoomDto);

            // Возвращаем 201 Created с ссылкой на созданный ресурс
            return CreatedAtAction(nameof(GetRoom), new { id = createdRoom.Id }, createdRoom);
        }
        catch (Exception ex) // Ловим возможные исключения из сервиса
        {
            return BadRequest(ex.Message);                  // 400 Bad Request с сообщением
        }
    }

    // PUT api/admin/adminrooms/5 - Обновить номер
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(int id, [FromBody] CreateRoomDto updateRoomDto)
    {
        try
        {
            var isUpdated = await _roomService.UpdateRoomAsync(id, updateRoomDto);

            if (!isUpdated)                                 // Если обновление не удалось
            {
                return NotFound();                          // 404 Not Found
            }
            return NoContent();                             // 204 No Content
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);                  // 400 Bad Request
        }
    }

    // DELETE api/admin/adminrooms/5 - Удалить номер
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(int id)
    {
        var isDeleted = await _roomService.DeleteRoomAsync(id);

        // Если удаление не удалось
        if (!isDeleted)
        {
            return NotFound();  // 404 Not Found
        }
        return NoContent();    // 204 No Content
    }
}