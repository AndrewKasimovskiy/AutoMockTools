using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public static class MyMapper
    {
        private static IMapper InnerMapper { get; set; }

        public static void Initialize()
        {
            InnerMapper = new MapperConfiguration(cfg => cfg.AddProfile<MyProfile>()).CreateMapper();
            InnerMapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        public static TDestination Map<TDestination>(object source)
        {
            return InnerMapper.Map<TDestination>(source);
        }
    }
}
