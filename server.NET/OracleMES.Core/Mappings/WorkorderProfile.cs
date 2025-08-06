using AutoMapper;
using OracleMES.Core.Entities;
using OracleMES.Core.DTOs;

namespace OracleMES.Core.Mappings
{
    public class WorkorderProfile : Profile
    {
        public WorkorderProfile()
        {
            // Entity to Response DTO 
            CreateMap<Workorder, WorkorderResponseDTO>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Orderid.ToString()))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Productid.ToString()))
                .ForMember(dest => dest.WorkcenterId, opt => opt.MapFrom(src => src.Workcenterid.ToString()))
                .ForMember(dest => dest.MachineId, opt => opt.MapFrom(src => src.Machineid.ToString()))
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Employeeid.ToString()))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => (int)src.Quantity))
                .ForMember(dest => dest.PlannedStartTime, opt => opt.MapFrom(src => src.Plannedstarttime))
                .ForMember(dest => dest.PlannedEndTime, opt => opt.MapFrom(src => src.Plannedendtime))
                .ForMember(dest => dest.ActualStartTime, opt => opt.MapFrom(src => src.Actualstarttime))
                .ForMember(dest => dest.ActualEndTime, opt => opt.MapFrom(src => src.Actualendtime))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => (int)src.Priority))
                .ForMember(dest => dest.LeadTime, opt => opt.MapFrom(src => (int)src.Leadtime))
                .ForMember(dest => dest.LotNumber, opt => opt.MapFrom(src => src.Lotnumber))
                .ForMember(dest => dest.ActualProduction, opt => opt.MapFrom(src => src.Actualproduction))
                .ForMember(dest => dest.Scrap, opt => opt.MapFrom(src => src.Scrap))
                .ForMember(dest => dest.SetupTimeActual, opt => opt.MapFrom(src => src.Setuptimeactual));
        }
    }
}