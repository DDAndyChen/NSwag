using System;

namespace NSwag.Annotations
{
    /// <summary>Specifies the requet type of a HTTP operation to correctly generate a Swagger definition.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerRequestAttribute : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="SwaggerRequestAttribute"/> class.</summary>
        /// <param name="requestType">The JSON result type of the MVC or Web API action method.</param>
        /// <param name="name">Name of the body request.</param>
        /// <param name="description">The description.</param>
        public SwaggerRequestAttribute(Type requestType, string name = null, string description = null)
        {
            RequestType = requestType;
            Name = name;
            Description = description;
        }

        /// <summary>Gets or sets the request description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the request type.</summary>
        public Type RequestType { get; set; }

        /// <summary>Gets or sets the request name.</summary>
        public string Name { get; set; }
    }
}