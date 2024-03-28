#nullable disable
using HogWildSystem.BLL;
using HogWildSystem.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HogWildSystem
{
    //  your class needs to be public so it can be used outside of 
    //      this project
    //  this class also needs to be static
    public static class HogWildExtension
    {
        //  method name can be anything, however it must match
        //  the builder.Services.XXXX(options =>...)
        //      statement in your program.cs

        //  the first parameter is the class that you are attempting to extend
        //  the second parameter is the options value in your call statement
        //  is is receiving the connection string for your application
        public static void AddBackendDependencies(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            //  register the DBContext class in HogWildSystem with the service collection
            services.AddDbContext<HogWildContext>(options);

            //  adding any services that you create in the class library (BLL)
            //  using .AddTransient<t>(...)
            //  customer
            services.AddTransient<CustomerService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<HogWildContext>();
                return new CustomerService(context);
            });

            //  employee
            services.AddTransient<EmployeeService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<HogWildContext>();
                return new EmployeeService(context);
            });

            //  category & lookup
            services.AddTransient<CategoryLookupService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<HogWildContext>();
                return new CategoryLookupService(context);
            });

            //  inventory
            services.AddTransient<PartService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<HogWildContext>();
                return new PartService(context);
            });
            //  invoice
            services.AddTransient<InvoiceService>((ServiceProvider) =>
            {
                var context = ServiceProvider.GetService<HogWildContext>();
                return new InvoiceService(context);
            });
        }
    }
}
