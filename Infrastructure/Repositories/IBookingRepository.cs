using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IBookingRepository
    {
        // Получить бронирование по ID
        // Возвращает Booking или null если не найдено
        Task<Booking?> GetByIdAsync(int id);

        // Получить все бронирования конкретного пользователя
        // Возвращает список бронирований
        Task<List<Booking>> GetUserBookingsAsync(string userId);

        // Получить все бронирования конкретного номера
        // Возвращает список бронирований для этого номера
        Task<List<Booking>> GetRoomBookingsAsync(int roomId);

        // Получить все бронирования из системы
        // Возвращает полный список всех бронирований
        Task<List<Booking>> GetAllAsync();

        // Добавить новое бронирование
        // Возвращает созданное бронирование с заполненным id
        Task<Booking> AddAsync(Booking booking);

        // Обновить существующее бронирование
        Task UpdateAsync(Booking booking);

        // Проверить доступен ли номер в указанные даты
        // Возвращает true если номер свободен, false если занят
        Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeBookingId = null);
    }
}