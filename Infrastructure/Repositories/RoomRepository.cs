using Domain.Entities;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace Infrastructure.Repositories
{
    // Репозиторий для работы с номерами отеля в базе данных
    public class RoomRepository : IRoomRepository

    {
        // Контекст БД
        private readonly ApplicationDbContext _context;

        // Конструктор получает контекст через Dependency Injection
        public RoomRepository(ApplicationDbContext context)
        {
            _context = context; // Сохраняем переданный контекст в поле класса
        }


        // Получить номер по ID или возвращает null если не найден
        public async Task<Room?> GetByIdAsync(int id) =>
            await _context.Rooms.FindAsync(id);            // FindAsync ищет по id

        // Получить все номера из базы
        public async Task<List<Room>> GetAllAsync() =>
            await _context.Rooms.ToListAsync();             // ToListAsync возвращает все записи из таблицы Rooms

        // Добавить новый номер в базу
        public async Task<Room> AddAsync(Room room)
        {
            _context.Rooms.Add(room);                       // Добавляем объект в DbSet
            await _context.SaveChangesAsync();              // Сохраняем изменения в базе 
            return room;                                    // Возвращаем созданный объект с заполненным ID
        }

        // Обновить существующий номер
        public async Task UpdateAsync(Room room)
        {
            _context.Rooms.Update(room);                    // Помечаем объект как измененный
            await _context.SaveChangesAsync();              // Сохраняем изменения в базе 
        }

        // Удалить номер по ID
        public async Task DeleteAsync(int id)
        {
            var room = await GetByIdAsync(id);              // Сначала находим номер
            if (room != null)                               // Если номер существует
            {
                _context.Rooms.Remove(room);                // Помечаем для удаления
                await _context.SaveChangesAsync();          // Помечаем для удаления
            }

            // Если номер не найден ничего не происходит(может исключение сделаешь или тебе лень?)
        }

        // Проверить существует ли номер с указанным ID
        public async Task<bool> ExistsAsync(int id) =>
            await _context.Rooms.AnyAsync(r => r.Id == id);  // AnyAsync проверяет есть ли хотя бы одна запись
    }
}