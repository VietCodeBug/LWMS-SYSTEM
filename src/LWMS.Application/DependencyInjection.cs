using Microsoft.Extensions.DependencyInjection;
using MediatR;
using FluentValidation;
using AutoMapper;
    namespace LWMS.Application
{
     public static class DependencyInjection
{
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Sau nay có thể thêm các service khác vào đây, ví dụ như MediatR, AutoMapper, v.v.
            // MediatR v12+ chỉ cần đăng ký như thế này
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>),typeof(Common.Behaviors.ValidationBehavior<,>));
            return services;
        }
    }
    } 
       
   

  


