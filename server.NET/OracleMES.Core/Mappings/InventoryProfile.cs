using AutoMapper;
using OracleMES.Core.Entities;
using OracleMES.Core.DTOs;

namespace OracleMES.Core.Mappings
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            // Entity to Response DTO
            CreateMap<Inventory, InventoryResponseDTO>()
                .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.Itemid.ToString()))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ReorderLevel, opt => opt.MapFrom(src => src.Reorderlevel))
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.Supplierid.HasValue ? src.Supplierid.Value.ToString() : null))
                .ForMember(dest => dest.LeadTime, opt => opt.MapFrom(src => src.Leadtime))
                .ForMember(dest => dest.Cost, opt => opt.MapFrom(src => src.Cost))
                .ForMember(dest => dest.LotNumber, opt => opt.MapFrom(src => src.Lotnumber))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location))
                .ForMember(dest => dest.LastReceivedDate, opt => opt.MapFrom(src => src.Lastreceiveddate));
        }
    }
}