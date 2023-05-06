using AutoMapper;
using TheRoost.API.Models.DTOs;
using TheRoost.API.Models.Entities;
using TheRoost.API.Models.Entities.PropertyTypes;

namespace TheRoost.API.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<AccommodationCreationDTO, Hotel>();
            CreateMap<AccommodationCreationDTO, Hostel>();
            CreateMap<AccommodationCreationDTO, Apartment>();
            CreateMap<AccommodationCreationDTO, Guesthouse>();
            CreateMap<ReturnAccommodationDTO, Accommodation>().ReverseMap()
                .ForMember(dto => dto.TypeOfProperty, opt => opt.MapFrom(src => src.GetType().Name)); ;
            CreateMap<AccommodationUpdateDTO, Hotel>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AccommodationUpdateDTO, Hostel>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AccommodationUpdateDTO, Apartment>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<AccommodationUpdateDTO, Guesthouse>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<AccountEditDTO, User>();
            CreateMap<AccountEditDTO, User>()
                .ForMember(dest => dest.Password, input => input.MapFrom(i => i.NewPassword))
                .ForMember(dest => dest.ID, input => input.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Accommodation, AccommodationSearchDTO>()
                .ForMember(dto => dto.TypeOfProperty, opt => opt.MapFrom(src => src.GetType().Name));
            CreateMap<Room, ReturnRoomDTO>()
                .ForMember(dto => dto.AccomodationName, opt => opt.MapFrom(src => src.Accommodation.Name))
                .ForMember(dto => dto.RoomType, opt => opt.MapFrom(src => src.RoomType.Name))
                .ForMember(dto => dto.TypeOfProperty, opt => opt.MapFrom(src => src.Accommodation.GetType().Name));
            CreateMap<Reservation, ReturnUserReservation>();
            CreateMap<Room, ReturnRoomDtoWithoutReservations>();
            CreateMap<Reservation, ReturnReservationsDTO>()
               .ForMember(dto => dto.AccommodationName, opt => opt.MapFrom(src => src.Accommodations.Name))
               .ForMember(dto => dto.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
               .ForMember(dto => dto.RoomType, opt => opt.MapFrom(src => src.Room.RoomType.Name));
        }
    }
}
