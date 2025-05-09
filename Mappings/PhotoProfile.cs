using AutoMapper;
using FlickrApp.Entities;
using FlickrApp.Models;

namespace FlickrApp.Mappings;

public class PhotoProfile : Profile
{
    public PhotoProfile()
    {
        CreateMap<PhotoEntity, FlickrPhoto>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dto => dto.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dto => dto.Secret, opt => opt.MapFrom(src => src.Secret))
            .ForMember(dto => dto.Server, opt => opt.MapFrom(src => src.Server))
            ;

        CreateMap<FlickrPhoto, PhotoEntity>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dto => dto.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dto => dto.Secret, opt => opt.MapFrom(src => src.Secret))
            .ForMember(dto => dto.Server, opt => opt.MapFrom(src => src.Server))
            ;
    }
}