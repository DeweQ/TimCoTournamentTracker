using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.AutoMapperConverters;
using TrackerLibrary.Models;

namespace TrackerLibrary;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<PrizeModel, string[]>()
            .ConstructUsing(
            x => new string[] { x.Id.ToString(), x.PlaceNumber.ToString(), x.PlaceName, x.PrizeAmount.ToString(), x.PrizePercentage.ToString() })
            .ReverseMap()
            .ForPath(s => s.Id, opt => opt.MapFrom(d => d[0]))
            .ForPath(s => s.PlaceNumber, opt => opt.MapFrom(d => d[1]))
            .ForPath(s => s.PlaceName, opt => opt.MapFrom(d => d[2]))
            .ForPath(s => s.PrizeAmount, opt => opt.MapFrom(d => d[3]))
            .ForPath(s => s.PrizePercentage, opt => opt.MapFrom(d => d[4]));

        CreateMap<PersonModel, string[]>()
            .ConstructUsing(
            x => new string[] { x.Id.ToString(), x.FirstName, x.LastName, x.EmailAddress, x.CellphoneNumber })
            .ReverseMap()
            .ForPath(s => s.Id, opt => opt.MapFrom(d => d[0]))
            .ForPath(s => s.FirstName, opt => opt.MapFrom(d => d[1]))
            .ForPath(s => s.LastName, opt => opt.MapFrom(d => d[2]))
            .ForPath(s => s.EmailAddress, opt => opt.MapFrom(d => d[3]))
            .ForPath(s => s.CellphoneNumber, opt => opt.MapFrom(d => d[4]));

        CreateMap<TeamModel, string[]>()
            .ConstructUsing(
            x => new string[] { x.Id.ToString(), x.TeamName, string.Join('|', x.TeamMembers.Select(m => m.Id)) })
            .ReverseMap()
            .ForPath(s => s.Id, opt => opt.MapFrom(d => d[0]))
            .ForPath(s => s.TeamName, opt => opt.MapFrom(d => d[1]))
            .ForPath(s => s.TeamMembers, opt => opt.MapFrom(d => d[2]));

        CreateMap<TournamentModel, string[]>()
            .ConstructUsing(
            x => new string[]
            {
                x.Id.ToString(),//0
                x.TournamentName,//Great Tournament Name
                x.EntryFee.ToString(),//10.00
                string.Join('|',x.EnteredTeams.Select(t => t.Id)),//1|2|3|4|5|6|7|8
                string.Join('|',x.Prizes.Select(p => p.Id)),//1|2
                string.Join('|',x.Rounds.Select( r => string.Join('^',r.Select(m => m.Id))))//1^2^3^4|5^6|7
            })
            .ReverseMap()
            .ForPath(s => s.Id, opt => opt.MapFrom(d => d[0]))
            .ForPath(s => s.TournamentName, opt => opt.MapFrom(d => d[1]))
            .ForPath(s => s.EntryFee, opt => opt.MapFrom(d => d[2]))
            .ForPath(s => s.EnteredTeams, opt => opt.MapFrom(d => d[3]))
            .ForPath(s => s.Prizes, opt => opt.MapFrom(d => d[4]))
            .ForPath(s => s.Rounds, opt => opt.MapFrom(d => d[5]));

        CreateMap<MatchupModel, string[]>()
            .ConstructUsing(
            x => new string[]
            {
                x.Id.ToString(),
                string.Join('|', x.Entries.Select(e => e.Id)),
                x.Winner==null?"":x.Winner.Id.ToString(),
                x.MatchupRound.ToString()
            })
            .ReverseMap()
            .ForPath(s => s.Id, opt => opt.MapFrom(d => d[0]))
            .ForPath(s => s.Entries, opt => opt.MapFrom(d => d[1]))
            .ForPath(s => s.Winner, opt => opt.MapFrom(d => d[2]))
            .ForPath(s => s.MatchupRound, opt => opt.MapFrom(d => d[3]));

        CreateMap<MatchupEntryModel, string[]>()
            .ConstructUsing(
            x => new string[]
            {
                x.Id.ToString(),
                x.TeamCompeting==null?"":x.TeamCompeting.Id.ToString(),
                x.Score.ToString(),
                x.ParentMatchup ==null?"":x.ParentMatchup.Id.ToString()
            })
            .ReverseMap()
            .ForPath(s => s.Id, opt => opt.MapFrom(d => d[0]))
            .ForPath(s => s.TeamCompeting, opt => opt.MapFrom(d => d[1]))
            .ForPath(s => s.Score, opt => opt.MapFrom(d => d[2]))
            .ForPath(s => s.ParentMatchup, opt => opt.MapFrom(d => d[3]));

        CreateMap<string, List<PrizeModel>>()
            .ConvertUsing(new PrizeModelConverter());

         CreateMap<string, List<TeamModel>>()
            .ConvertUsing(new TeamModelConverter());

         CreateMap<string, List<List<MatchupModel>>>()
            .ConvertUsing(new MatchupModelConverter());

        CreateMap<string, List<PersonModel>>()
            .ConvertUsing(new PersonModelConverter());

        CreateMap<string, List<MatchupEntryModel>>()
            .ConvertUsing(new MatchupEntryConverter());

        CreateMap<string, PersonModel>()
            .ConvertUsing(new PersonModelConverter());

        CreateMap<string, MatchupModel>()
            .ConvertUsing(new MatchupModelConverter());

        CreateMap<string, TeamModel>()
            .ConvertUsing(new TeamModelConverter());
    }
}
