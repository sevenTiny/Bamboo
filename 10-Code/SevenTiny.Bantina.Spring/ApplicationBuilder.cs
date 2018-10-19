using SevenTiny.Bantina.Spring.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SevenTiny.Bantina.Spring
{
    public class ApplicationBuilder : IApplicationBuilder
    {
        private IList<Func<RequestDelegate, RequestDelegate>> _components = new List<Func<RequestDelegate, RequestDelegate>>();

        public RequestDelegate Build()
        {
            RequestDelegate app = context =>
            {
                return Task.FromResult<object>(null);
            };

            //reverse
            _components = _components.Reverse().ToList();

            //add dependency control middleware
            this.UseDependencyControl();

            foreach (var component in _components)
            {
                app = component(app);
            }

            return app;
        }

        public IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
        {
            _components.Add(middleware);
            return this;
        }
    }
}
