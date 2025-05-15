using AutoMapper;
using FlickrApp.Entities;
using FlickrApp.Models;

namespace FlickrApp.Mappings;

public class DetailProfile : Profile
{
    public DetailProfile()
    {
        /*
        CreateMap<DetailEntity, FlickrDetails>()
            .ForMember(dto => dto.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dto => dto.Description!.Content, opt => opt.MapFrom(src => src.Description))
            .ForMember(dto => dto.Tags!.Tag, opt => opt.Ignore())
            .ForMember(dto => dto.Owner!.Nsid, opt => opt.MapFrom(src => src.OwnerNsid))
            .ForMember(dto => dto.Owner!.Username, opt => opt.MapFrom(src => src.OwnerUsername))
            .ForMember(dto => dto.Farm, opt => opt.MapFrom(src => src.Farm))
            .ForMember(dto => dto.License, opt => opt.MapFrom(src => src.License))
            .ForMember(dto => dto.DateUploaded, opt => opt.MapFrom(src => src.DateUploaded))
            .ForMember(dto => dto.Views, opt => opt.MapFrom(src => src.Views))
            ;
        */
        CreateMap<FlickrDetails, DetailEntity>()
            .ForMember(entity => entity.Id, opt => opt.MapFrom(dto => dto.Id))
            .ForMember(entity => entity.Description, opt => opt.MapFrom(dto =>
                dto.Description != null ? dto.Description.Content : null))
            .ForMember(entity => entity.OwnerNsid, opt => opt.MapFrom(dto =>
                dto.Owner != null ? dto.Owner.Nsid : null))
            .ForMember(entity => entity.OwnerUsername, opt => opt.MapFrom(dto =>
                dto.Owner != null ? dto.Owner.Username : null))
            .ForMember(entity => entity.Farm, opt => opt.MapFrom(dto => dto.Farm))
            .ForMember(entity => entity.License, opt => opt.MapFrom(dto => dto.License))
            .ForMember(entity => entity.DateUploaded,
                opt => opt.MapFrom(dto => DateTimeOffset.FromUnixTimeSeconds(long.Parse(dto.DateUploaded!))))
            .ForMember(entity => entity.Views, opt => opt.MapFrom(dto => dto.Views))
            .ForMember(entity => entity.Tags, opt => opt.MapFrom(dto =>
                dto.Tags == null || dto.Tags.Tag == null || dto.Tags.Tag.Count == 0
                    ? string.Empty
                    : dto.Tags.ToString()))
            .ForMember(entity => entity.Photo, opt => opt.MapFrom(dto => new PhotoEntity
            {
                Id = dto!.Id,
                Title = dto.Title!.Content ?? string.Empty,
                Secret = dto.Secret,
                Server = dto.Server
            }))
            .AfterMap((dto, entity) =>
            {
                if (entity.Photo != null) entity.Photo.Detail = entity;
            })
            ;
    }
}