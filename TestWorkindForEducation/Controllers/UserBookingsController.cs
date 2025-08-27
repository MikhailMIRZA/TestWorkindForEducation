using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTOs;

namespace TestWorkindForEducation.WebAPI.Controllers.User;

[ApiController]                  // Указывает что это контроллер API
[Route("api/user/[controller]")] // Базовый маршрут

// Базовый класс для API
public class UserBookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;    // Сервис для работы с бронированиями


    // Конструктор с внедрением зависимости сервиса
    public UserBookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // GET api/bookings/my?userId=test123 - Получить бронирования пользователя
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings(string userId)
    {
        var bookings = await _bookingService.GetUserBookingsAsync(userId);
        return Ok(bookings);    // 200 OK со списком бронирований
    }

    // GET api/bookings/5 - Получить конкретное бронирование по ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(int id)
    {
        var booking = await _bookingService.GetBookingByIdAsync(id);
        if (booking == null) return NotFound();     // 404 если не найдено
        return Ok(booking);                         // 200 OK с данными бронирования
    }

    // POST api/bookings  - Создать новое бронирование
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto createBookingDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);          // 400 если невалидные данные

        // Проверяем доступность номера на указанные даты
        var isAvailable = await _bookingService.IsRoomAvailableAsync(
            createBookingDto.RoomId,
            createBookingDto.StartDate,
            createBookingDto.EndDate
        );

        if (!isAvailable) return BadRequest("Комната уже забронирована на выбранные даты.");

        // Создаем бронирование
        var createdBooking = await _bookingService.CreateBookingAsync(createBookingDto, createBookingDto.UserId);

        // 201 Created
        return CreatedAtAction(nameof(GetBooking), new { id = createdBooking.Id }, createdBooking);
    }

    // DELETE api/bookings/5?userId=test123  - Отменить бронирование
    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelBooking(int id, string userId)
    {
        try
        {
            await _bookingService.CancelBookingAsync(id, userId);
            return NoContent();              // 204 No Content при успешном удалении
        }
        catch (KeyNotFoundException)         // Если бронирование не найдено
        {
            return NotFound();               // 404 Not Found
        }
        catch (UnauthorizedAccessException)  // Если пользователь не имеет прав
        {
            return Forbid();                 // 403 Forbidden
        }
        catch (Exception ex)                  // Другие ошибки
        {
            return BadRequest(ex.Message);   // 400 Bad Request
        }
    }
}