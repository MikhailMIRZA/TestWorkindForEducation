using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IRoomService
    {
        // получает все номера отеля
        // возвращает список DTO объектов комнат
        Task<List<RoomDto>> GetAllRoomsAsync();

        // получает номер по его идентификатору
        // возвращает RoomDto или null если номер не найден
        Task<RoomDto?> GetRoomByIdAsync(int id);

        // создает новый номер в отеле
        // возвращает созданный номер в виде DTO
        Task<RoomDto> CreateRoomAsync(CreateRoomDto createRoomDto);

        // обновляет существующий номер
        // возвращает true если обновление прошло успешно, false если номер не найден
        Task<bool> UpdateRoomAsync(int id, CreateRoomDto updateRoomDto);

        // удаляет номер из системы
        Task<bool> DeleteRoomAsync(int id);
    }
}