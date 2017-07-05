using System;
using System.Collections.Generic;
using System.Linq;

namespace NSwag.SwaggerGeneration
{
    /// <summary>
    /// Attribute Extensions
    /// </summary>
    public static class AttributeExtension
    {
        /// <summary>
        /// Get description in FlowTitleAttribute or empty if not found
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static string GetFlowTitle(this IEnumerable<Attribute> attributes)
        {
            if (attributes == null) return string.Empty;

            dynamic flowDescAttr = attributes.SingleOrDefault(a => a.GetType().Name == "FlowTitleAttribute");

            return flowDescAttr != null ? flowDescAttr.Description : string.Empty;
        }
    }
}