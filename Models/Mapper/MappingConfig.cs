using AutoMapper;
using PhotoGallery.Models.Entities;
using PhotoGallery.Models.ServiceDTOs;
using PhotoGallery.Models.VMs;

namespace PhotoGallery.Models.Mapper;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        CreateMap<RegistrationVM, UserServiceDTO>();
        CreateMap<LoginVM, UserServiceDTO>();
        CreateMap<RefreshTokenVM, RefreshTokenServiceDTO>();

        CreateMap<Photo, PhotoDTO>().ReverseMap();
        CreateMap<AlbumDto, Album>().ReverseMap();
        CreateMap<UserServiceDTO, User>().ReverseMap();
    }
}
