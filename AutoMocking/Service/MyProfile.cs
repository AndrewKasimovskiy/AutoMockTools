using AutoMapper;

namespace Service
{
	public class MyProfile : Profile
    {
        public MyProfile()
        {
            CreateMap<Item, DealTask>(MemberList.Destination)
                .ForMember(dst => dst.DealSer, m => m.MapFrom(i => i.DealSer))
                .ForMember(dst => dst.TypeId, m => m.MapFrom(i => i.TypeId))
                .ForMember(dst => dst.ProductId, m => m.MapFrom(i => i.ProductId))
                .ForMember(dst => dst.Cid, m => m.MapFrom(i => i.Cid))
                .ForMember(dst => dst.TaskStatus, m => m.MapFrom(i => i.TaskStatus))
                .ForMember(dst => dst.Context, m => m.MapFrom(i => i.Context));

            CreateMap<DealTask, Item>(MemberList.Destination)
                .BeforeMap((s, d) => d.TimeSent = null)
                .BeforeMap((s, d) => d.TimeRecv = null)
                .ForMember(dst => dst.Id, m => m.Ignore())
                .ForMember(dst => dst.DealSer, m => m.MapFrom(i => i.DealSer))
                .ForMember(dst => dst.TypeId, m => m.MapFrom(i => i.TypeId))
                .ForMember(dst => dst.ProductId, m => m.MapFrom(i => i.ProductId))
                .ForMember(dst => dst.Cid, m => m.MapFrom(i => i.Cid))
                .ForMember(dst => dst.TaskStatus, m => m.MapFrom(i => i.TaskStatus))
                .ForMember(dst => dst.TimeSent, m => m.Ignore())
                .ForMember(dst => dst.TimeRecv, m => m.Ignore())
                .ForMember(dst => dst.Context, m => m.MapFrom(i => i.Context));

            CreateMap<Item, ItemArch>(MemberList.Destination);
            CreateMap<Item, ItemCanceled>(MemberList.Destination);
        }
    }
}
