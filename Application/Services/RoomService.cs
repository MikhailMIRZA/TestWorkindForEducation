using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    // Сервис для работы с номерами отеля - реализует интерфейс IRoomService
    public class RoomService : IRoomService
    {
        // Репозиторий для работы с номерами в БД
        private readonly IRoomRepository _roomRepository;

        // Конструктор получает репозиторий через Dependency Injection
        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;   // Сохраняем переданный репозиторий
        }

        // Получить номер по ID - возвращает DTO или null если не найден
        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id); // Ищем номер в базе
            return room != null ? MapToDto(room) : null;       // Если есть - преобразуем в DTO, если нет - возвращаем null
        }

        // Получить номера отеля
        public async Task<List<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync(); // Получаем все номера из базы
            return rooms.Select(MapToDto).ToList();          // Преобразуем каждый номер в DTO и возвращаем список
        }

        // создать новый номер
        public async Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto)
        {
            // Создаем новый объект номера на основе данных из DTO
            var room = new Room
            {
                Name = createRoomDto.Name,
                Class = createRoomDto.Class,
                Price = createRoomDto.Price,
                Description = createRoomDto.Description,
                Capacity = createRoomDto.Capacity,
                IsAvailable = true
            };

            // Сохраняем номер в базу и получаем созданный объект с ID
            var createdRoom = await _roomRepository.AddAsync(room);

            // Возвращаем созданный номер в виде DTO
            return MapToDto(createdRoom);
        }

        // Обновить существующий номер
        public async Task<bool> UpdateRoomAsync(int id, CreateRoomDto updateRoomDto)
        {
            // Находим номер в базе
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) throw new ArgumentException("Комната не найдена");

            // Обновляем все поля номера
            room.Name = updateRoomDto.Name;
            room.Class = updateRoomDto.Class;
            room.Price = updateRoomDto.Price;
            room.Description = updateRoomDto.Description;
            room.Capacity = updateRoomDto.Capacity;

            // Сохраняем изменения в базе
            await _roomRepository.UpdateAsync(room);

            // Возвращаем true
            return true;
        }

        // удалить номер
        public async Task<bool> DeleteRoomAsync(int id)
        {
            // Проверяем существует ли номер
            if (!await _roomRepository.ExistsAsync(id))
                return false;

            // Удаляем номер из базы
            await _roomRepository.DeleteAsync(id);

            // Возвращаем true
            return true;
        }

        // Метод для преобразования Room в RoomDto
        private static RoomDto MapToDto(Room room) => new()
        {
            Id = room.Id,
            Name = room.Name,
            Class = room.Class,
            Price = room.Price,
            Description = room.Description,
            Capacity = room.Capacity,
            IsAvailable = room.IsAvailable
        };
    }
}
