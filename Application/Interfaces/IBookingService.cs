using Application.DTOs;

namespace Application.Interfaces
{
    public interface IBookingService
    {
        // получает бронирование по ID
        // Возвращает BookingDto или null если не найдено
        Task<BookingDto?> GetBookingByIdAsync(int id);

        //  получает все бронирования конкретного пользователя
        Task<List<BookingDto>> GetUserBookingsAsync(string userId);

        // получает все бронирования и в системе
        Task<List<BookingDto>> GetAllBookingsAsync();

        // создает новое бронирование
        Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto, string userId);

        // отменяет бронирование
        Task CancelBookingAsync(int id, string userId);

        // проверяет свободен ли номер в указанные даты
        // возвращает true если номер свободен false если занят
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate);
    }
}