using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Football.Application.DTOs;
using Football.Domain.Entities;

namespace Football.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
           
            CreateMap<Team, TeamDto>().ReverseMap();

            CreateMap<Player, PlayerDto>().ReverseMap();

            CreateMap<Weather, WeatherDto>().ReverseMap();

            CreateMap<LineUp, LineUpDto>();
            CreateMap<LineUpDto, LineUp>()
                //.ForMember(dest => dest.GameId, opt => opt.Ignore()) // se asigna manualmente en el servicio
                .ForMember(dest => dest.TeamId, opt => opt.MapFrom(src => src.TeamId))
                .ForMember(dest => dest.PlayerId, opt => opt.MapFrom(src => src.Player.Id))
                .ForMember(dest => dest.Team, opt => opt.Ignore())
                .ForMember(dest => dest.Player, opt => opt.Ignore())
                .ForMember(dest => dest.Game, opt => opt.Ignore());

            CreateMap<Game, GameDto>()
                .ForMember(dest => dest.HomeTeam, opt => opt.MapFrom(src => src.HomeTeam))
                .ForMember(dest => dest.AwayTeam, opt => opt.MapFrom(src => src.AwayTeam))
                .ForMember(dest => dest.LineUps, opt => opt.MapFrom(src => src.LineUps))
                .ForMember(dest => dest.Weather, opt => opt.MapFrom(src => src.Weather));

            CreateMap<GameDto, Game>()
                .ForMember(dest => dest.HomeTeamId, opt => opt.MapFrom(src => src.HomeTeam.Id))
                .ForMember(dest => dest.AwayTeamId, opt => opt.MapFrom(src => src.AwayTeam.Id))
                .ForMember(dest => dest.LineUps, opt => opt.MapFrom(src => src.LineUps))
                .ForMember(dest => dest.HomeTeam, opt => opt.Ignore())
                .ForMember(dest => dest.AwayTeam, opt => opt.Ignore())
                .ForMember(dest => dest.LineUps, opt => opt.Ignore())
                .ForMember(dest => dest.Weather, opt => opt.Ignore());

        }
    }
}