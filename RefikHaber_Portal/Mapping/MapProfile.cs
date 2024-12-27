using AutoMapper;
using RefikHaber.Models;
using RefikHaber.ViewModels;

namespace RefikHaber.Mapping;

public class MapProfile : Profile
{
    public MapProfile()
    {
        CreateMap<Haber, HaberModel>().ReverseMap();
        CreateMap<HaberTuru, HaberTuruModel>().ReverseMap();
    }
}
