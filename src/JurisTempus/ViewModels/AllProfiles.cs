using System;
using AutoMapper;
using JurisTempus.Data.Entities;

namespace JurisTempus.ViewModels
{
    class AllDTOProfiles : Profile
    {
        public AllDTOProfiles()
        {
            // CONFIGURO MAPPING Datetime -> UTC -> ISOString
            CreateMap<string, DateTime?>().ConvertUsing(new DateTimeUTCTypeConverter());
            CreateMap<DateTime, string>().ConvertUsing(new ISOStringTypeConverter());

            // CONFIGURO TUTTI I MAPPING - PS: Potrei fare anche 1 profile per ogni DTO
            CreateMap<Client, ClientDTO>()
                .ForMember(d => d.ContactName, o => o.MapFrom(s => s.Contact)) // Configuro mapping con nomi prop diverse
                .ForMember(d => d.Address1, o => o.MapFrom(s => s.Address.Address1)) // Configuro mapping FLAT con nomi specifici
                                                                                     // POI USO FLAT AUTOMATICO BY CONVENTION: PREFIX Address<NOMEPROP> <-- ENTITY Address.<NOMEPROP>
                .ForMember(d => d.FullAddress, o => o.MapFrom((s, d) => d.Address1 + s.Address?.Address2 + s.Address?.Address3)) // Configuro mapping con FORMULA CALCOLO FullAddress
                .ReverseMap() // Configuro regola mapping inverso DTO -> Entity (FA IN AUTOMATICO INVERSO REGOLA PROPRIETA)
                .ForMember(rd => rd.Address, o => o.Ignore()); // Nel ReverseMap IGNORO il mapping x campo Address 

            CreateMap<Case, CaseDTO>() // Mapping automatico nomi/tipi corrispondenti (compresi gli ENUM -> string)
                .ForMember(d => d.StatusId, o => o.MapFrom(s => (int)s.Status)) // Configuro mapping ENUM -> int VALORE
                .ReverseMap();

            CreateMap<TimeBill, TimeBillDTO>()
                .ForMember(d => d.WorkDateUTC, o => o.MapFrom(s => s.WorkDate)) // QUESTO CAMPO USA IL CONVERTE DateTime -> string
                .ForMember(d => d.CaseId, o => o.MapFrom(s => s.Case.Id))
                .ForMember(d => d.EmployeeId, o => o.MapFrom(s => s.Employee.Id))
                .ReverseMap();
        }
    }
}


public class ISOStringTypeConverter : ITypeConverter<DateTime, string>
{
    const string ISOFORMAT = "yyyy-MM-dd\\THH:mm:ss.fffK"; //ISO-8601 used by Javascript (ALWAYS UTC)
    public string Convert(DateTime source, string destination, ResolutionContext context)
    {
        if (source == null || source == DateTime.MinValue) return null;
        if (source.Kind == DateTimeKind.Utc)
        { //If d is already UTC K format add 'Z' postfix, if d is LT K format add +/-TIMEOFFSET
            return source.ToString(ISOFORMAT);
        }
        else
        {
            //If d is LT or you don't want LocalTime -> convert to UTC and always add K format always add 'Z' postfix
            return source.ToUniversalTime().ToString(ISOFORMAT);
        }
    }
}
public class DateTimeUTCTypeConverter : ITypeConverter<string, DateTime?>
{
    const string ISOFORMAT = "yyyy-MM-dd\\THH:mm:ss.fffK"; //ISO-8601 used by Javascript (ALWAYS UTC)

    public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
    {
        if (string.IsNullOrEmpty(source)) return null;
        //Return a new DateTime parsed used ISOFORMAT - YOU MUST PASS A STRING ENDING WITH 'Z' OR +/-TIMEOFFSET
        var l = DateTime.ParseExact(source, ISOFORMAT, System.Globalization.CultureInfo.InvariantCulture);
        return l.ToUniversalTime(); //If you don't set useLocal returned date is always Kind=UTC
    }
}