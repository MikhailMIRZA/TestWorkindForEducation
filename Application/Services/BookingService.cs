using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Repositories;

namespace Application.Services
{
    public class BookingService : IBookingService
    {
        // Зависимости которые нам нужны для работы
        private readonly IBookingRepository _bookingRepository; // Для работы с бронированиями
        private readonly IRoomRepository _roomRepository;       // Для работы с номерами


        // Конструктор - получает зависимости через Dependency Injection
        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository)
        {
            _bookingRepository = bookingRepository; // Сохраняем переданный репозиторий бронирований
            _roomRepository = roomRepository;       // Сохраняем переданный репозиторий бронирований
        }

        // Получить бронирование по ID
        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id); // Получаем список броней пользователя
            return booking != null ? MapToDto(booking) : null;       // Преобразуем каждую бронь в DTO и возвращаем список
        }

        public async Task<List<BookingDto>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _bookingRepository.GetUserBookingsAsync(userId);
            return bookings.Select(MapToDto).ToList();
        }

        // Получить бронирования
        public async Task<List<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepository.GetAllAsync();  // Получаем все брони из базы
            return bookings.Select(MapToDto).ToList();              // Преобразуем в DTO и возвращаем
        }

        // создать новое бронирование
        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto createBookingDto, string userId)
        {
            // Проверяем существует ли номер
            var room = await _roomRepository.GetByIdAsync(createBookingDto.RoomId);
            if (room == null) throw new ArgumentException("Комната не найдена");

            // Проверяем доступен ли номер вообще
            if (!room.IsAvailable) throw new InvalidOperationException("Комната не доступна");

            // Проверяем свободен ли номер в указанные даты
            if (!await IsRoomAvailableAsync(createBookingDto.RoomId, createBookingDto.StartDate, createBookingDto.EndDate))
            {
                throw new InvalidOperationException("Комната уже забронирована на указанную дату");
            }

            // Проверяем что даты корректны
            if (createBookingDto.StartDate >= createBookingDto.EndDate)
                throw new ArgumentException("Дата окончания должна быть после даты начала");

            // Создаем объект бронирования
            var booking = new Booking
            {
                RoomId = createBookingDto.RoomId,
                UserId = userId,
                UserName = createBookingDto.UserName,
                UserEmail = createBookingDto.UserEmail,
                StartDate = createBookingDto.StartDate,
                EndDate = createBookingDto.EndDate,
                Status = BookingStatus.Confirmed
            };

            // Сохраняем бронь в базу
            var createdBooking = await _bookingRepository.AddAsync(booking);

            // Возвращаем созданную бронь в виде DTO
            return MapToDto(createdBooking);
        }

        // отменить бронирование
        public async Task CancelBookingAsync(int id, string userId)
        {
            // Находим бронь
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) throw new ArgumentException("Бронь не найдена");

            // Проверяем права пользователя
            if (booking.UserId != userId) throw new UnauthorizedAccessException("Доступ запрещен");

            // Меняем статус на Отменено
            booking.Status = BookingStatus.Cancelled;

            // Сохраняем изменения в базе
            await _bookingRepository.UpdateAsync(booking);
        }

        // Проверить свободен ли номер в указанные даты
        public async Task<bool> IsRoomAvailableAsync(int roomId, DateTime startDate, DateTime endDate)
        {
            // Преобразуем даты в UTC
            var utcStartDate = startDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(startDate, DateTimeKind.Utc)
            : startDate.ToUniversalTime();

            var utcEndDate = endDate.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(endDate, DateTimeKind.Utc)
            : endDate.ToUniversalTime();

            // Делегируем проверку репозиторию
            return await _bookingRepository.IsRoomAvailableAsync(roomId, utcStartDate, utcEndDate);
        }

        //  Метод для преобразования Booking в BookingDto
        private static BookingDto MapToDto(Booking booking) => new()
        {
            Id = booking.Id,
            RoomId = booking.RoomId,
            RoomName = booking.Room?.Name ?? string.Empty,
            UserId = booking.UserId,
            UserName = booking.UserName,
            UserEmail = booking.UserEmail,
            StartDate = booking.StartDate,
            EndDate = booking.EndDate,
            Status = booking.Status.ToString(),
            CreatedAt = booking.CreatedAt
        };
    }
}