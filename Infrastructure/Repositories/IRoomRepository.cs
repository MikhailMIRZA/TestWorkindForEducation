using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Infrastructure.Repositories
{
    public interface IRoomRepository
    {
        // Интерфейс репозитория для работы с номерами отеля
        Task<Room?> GetByIdAsync(int id);

        // Получить номер по ID или возвращает null если не найден
        Task<List<Room>> GetAllAsync();

        // Получить все номера из базы
        Task<Room> AddAsync(Room room);

        // Добавить новый номер в базу
        Task UpdateAsync(Room room);

        // Обновить существующий номер
        Task DeleteAsync(int id);

        // Удалить номер по ID
        Task<bool> ExistsAsync(int id);
    }
}