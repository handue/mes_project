using AutoMapper;
using OracleMES.Core.Entities;
using OracleMES.Core.DTOs;

namespace OracleMES.Core.Mappings
{
    public class MachineProfile : Profile
    {
        public MachineProfile()
        {
            // Entity to Response DTO
            CreateMap<Machine, MachineResponseDTO>()
                .ForMember(dest => dest.MachineId, opt => opt.MapFrom(src => src.Machineid.ToString()))
                .ForMember(dest => dest.WorkcenterId, opt => opt.MapFrom(src => src.Workcenterid.HasValue ? src.Workcenterid.Value.ToString() : null))
                .ForMember(dest => dest.NominalCapacity, opt => opt.MapFrom(src => src.Nominalcapacity))
                .ForMember(dest => dest.CapacityUOM, opt => opt.MapFrom(src => src.Capacityuom))
                .ForMember(dest => dest.SetupTime, opt => opt.MapFrom(src => src.Setuptime))
                .ForMember(dest => dest.EfficiencyFactor, opt => opt.MapFrom(src => src.Efficiencyfactor))
                .ForMember(dest => dest.MaintenanceFrequency, opt => opt.MapFrom(src => src.Maintenancefrequency))
                .ForMember(dest => dest.LastMaintenanceDate, opt => opt.MapFrom(src => src.Lastmaintenancedate))
                .ForMember(dest => dest.NextMaintenanceDate, opt => opt.MapFrom(src => src.Nextmaintenancedate))
                .ForMember(dest => dest.ProductChangeoverTime, opt => opt.MapFrom(src => src.Productchangeovertime))
                .ForMember(dest => dest.CostPerHour, opt => opt.MapFrom(src => src.Costperhour))
                .ForMember(dest => dest.InstallationDate, opt => opt.MapFrom(src => src.Installationdate))
                .ForMember(dest => dest.ModelNumber, opt => opt.MapFrom(src => src.Modelnumber));

            // Create DTO to Entity
            CreateMap<CreateMachineDTO, Machine>()
                .ForMember(dest => dest.Machineid, opt => opt.MapFrom(src => decimal.Parse(src.MachineId)))
                .ForMember(dest => dest.Workcenterid, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.WorkcenterId) ? decimal.Parse(src.WorkcenterId) : (decimal?)null))
                .ForMember(dest => dest.Nominalcapacity, opt => opt.MapFrom(src => src.NominalCapacity))
                .ForMember(dest => dest.Capacityuom, opt => opt.MapFrom(src => src.CapacityUOM))
                .ForMember(dest => dest.Setuptime, opt => opt.MapFrom(src => src.SetupTime))
                .ForMember(dest => dest.Efficiencyfactor, opt => opt.MapFrom(src => src.EfficiencyFactor))
                .ForMember(dest => dest.Maintenancefrequency, opt => opt.MapFrom(src => src.MaintenanceFrequency))
                .ForMember(dest => dest.Productchangeovertime, opt => opt.MapFrom(src => src.ProductChangeoverTime))
                .ForMember(dest => dest.Costperhour, opt => opt.MapFrom(src => src.CostPerHour))
                .ForMember(dest => dest.Installationdate, opt => opt.MapFrom(src => src.InstallationDate))
                .ForMember(dest => dest.Modelnumber, opt => opt.MapFrom(src => src.ModelNumber))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ?? "Available"))
                .ForMember(dest => dest.Lastmaintenancedate, opt => opt.Ignore())
                .ForMember(dest => dest.Nextmaintenancedate, opt => opt.Ignore());
        }
    }
}