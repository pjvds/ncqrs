using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ncqrs;
using Autofac;

namespace Ncqrs.Config.Autofac
{
    public class AutofacConfiguration : IEnvironmentConfiguration
    {
        readonly IContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacConfiguration"/> class.
        /// </summary>
        /// <param name="container">The Autofac container which will provide components to Ncqrs.</param>
        public AutofacConfiguration(IContainer container)
        {
            this.container = container;
        }

        /// <summary>
        /// Tries to get the specified instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance to get.</typeparam>
        /// <param name="result">The result.</param>
        /// <returns>A indication whether the instance could be get or not.</returns>
        public bool TryGet<T>(out T result) where T : class
        {
            result = container.ResolveOptional<T>();

            return result != null;
        }
    }
}


