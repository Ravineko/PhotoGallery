using AutoMapper;
using PhotoGallery.Models.RepositoryDTOs;
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

        CreateMap<UserServiceDTO, User>().ReverseMap();
    }
}
