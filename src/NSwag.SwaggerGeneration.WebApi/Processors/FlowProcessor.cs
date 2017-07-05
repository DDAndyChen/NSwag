using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.WebApi.Processors
{
    /// <summary>Flow Processor</summary>
    public class FlowProcessor : IOperationProcessor
    {
        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public async Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            await AddDescription(context).ConfigureAwait(false);

            return true;
        }

        private async Task AddDescription(OperationProcessorContext context)
        {
            dynamic flowDescAttr = context.MethodInfo.GetCustomAttributes()
                .SingleOrDefault(a => a.GetType().Name == "FlowOperationDescriptionAttribute");

            if (flowDescAttr != null)
                context.OperationDescription.Operation.Description = flowDescAttr.Description;
            else {
                var remarks = await context.MethodInfo.GetXmlRemarksAsync().ConfigureAwait(false);
                if (remarks != string.Empty)
                    context.OperationDescription.Operation.Description = remarks;
            }
        }
    }
}