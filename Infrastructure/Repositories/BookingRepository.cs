using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    // Репозиторий для работы с бронированиями в базе данных
    public class BookingRepository : IBookingRepository
    {
        // Контекст БД
        private readonly ApplicationDbContext _context;

        // Конструктор получает контекст базы через Dependency Injection
        public BookingRepository(ApplicationDbContext context)
        {
            _context = context; // Сохраняем переданный контекст
        }

        // Получить бронирование по ID или вернуть null если не найдено
        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings
                .Include(b => b.Room)  // Загружаем связанные данные о номере
                .FirstOrDefaultAsync(b => b.Id == id);  // Загружаем связанные данные о номере
        }

        // Получить все бронирования конкретного пользователя
        public async Task<List<Booking>> GetUserBookingsAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Room)               // Загружаем данные о номере
                .Where(b => b.UserId == userId)     // Фильтруем по ID пользователя
                .ToListAsync();                     // Преобразуем в список
        }

        // Получить вск бронирования конкретного номера
        public async Task<List<Booking>> GetRoomBookingsAsync(int roomId)
        {
            return await _context.Bookings
                .Where(b => b.RoomId == roomId)     // Фильтруем по ID номера
                .ToListAsync();                     // Без включения данных о номере 
        }

        // Получить все бронирования из базы
        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Room)               // Загружаем данные о номере для каждого бронирования
                .ToListAsync();                     // Возвращаем полный список
        }

        // Добавить новое бронирование в базу
        public async Task<Booking> AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);          // Добавляем объект в контекст
            await _context.SaveChangesAsync();       // Сохраняем изменения в базе
            return booking;                          // Возвращаем созданный объект
        }

        // Обновить существующее бронирование
        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);      // Помечаем объект как измененный
            await _context.SaveChangesAsync();      // Сохраняем изменения в базе
        }

        // Проверить свободен ли номер в указанные даты
        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate, int? excludeBookingId = null)
        {
            // Создаем запрос для поиска конфликтующих бронирований:
            var query = _context.Bookings
                .Where(b => b.RoomId == roomId &&                   // Для указанного номера
                           b.Status == BookingStatus.Confirmed &&   // Только подтвержденные брони
                           b.StartDate < endDate &&                 // Начало брони раньше конца проверяемого периода
                           b.EndDate > startDate);                  // Конец брони позже начала проверяемого периода

            // Если указан excludeBookingId - исключаем его из проверки
            if (excludeBookingId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookingId.Value);
            }

            // Если не найдено ни одного конфликтующего бронирования - номер свободен
            return !await query.AnyAsync();
        }
    }
}