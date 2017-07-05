using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NJsonSchema.Generation;
using NJsonSchema.Infrastructure;
using NSwag.SwaggerGeneration.Processors;
using NSwag.SwaggerGeneration.Processors.Contexts;

namespace NSwag.SwaggerGeneration.WebApi.Processors
{
    /// <summary>Flow Processor</summary>
    public class FlowProcessor : IOperationProcessor
    {
        private readonly WebApiToSwaggerGeneratorSettings _settings;

        /// <summary>Initializes a new instance of the <see cref="FlowProcessor"/> class.</summary>
        /// <param name="settings">The settings.</param>
        public FlowProcessor(WebApiToSwaggerGeneratorSettings settings)
        {
            _settings = settings;
        }

        /// <summary>Processes the specified method information.</summary>
        /// <param name="context"></param>
        /// <returns>true if the operation should be added to the Swagger specification.</returns>
        public async Task<bool> ProcessAsync(OperationProcessorContext context)
        {
            await AddParameterFromAnnotation(context).ConfigureAwait(false);
            await AddDescription(context).ConfigureAwait(false);

            return true;
        }

        #region AddDescription

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

        #endregion AddDescription

        #region AddParameterFromAnnotation

        /// <summary>
        /// Add parameter from the annotation of the method
        /// </summary>
        /// <returns></returns>
        private async Task AddParameterFromAnnotation(OperationProcessorContext context)
        {
            var attributes = context.MethodInfo.GetCustomAttributes().ToList();

            var requestAttribute = attributes.SingleOrDefault(a => a.GetType().Name == "SwaggerRequestAttribute");
            if (requestAttribute == null) {
                return;
            }

            var requestType = GetRequestType(requestAttribute);
            var parameterName = requestType.Name.ToLowerInvariant();

            string bodyParameterName = TryGetStringPropertyValue(requestAttribute, "Name") ?? parameterName;

            var operationParameter = await CreateBodyParameterAsync(context.SwaggerGenerator, bodyParameterName, requestType, attributes).ConfigureAwait(false);

            context.OperationDescription.Operation.Parameters.Add(operationParameter);
        }

        private Type GetRequestType(Attribute attribute)
        {
            dynamic responseTypeAttribute = attribute;
            var attributeType = attribute.GetType();

            var returnType = typeof(void);
            if (attributeType.GetRuntimeProperty("RequestType") != null)
                returnType = responseTypeAttribute.RequestType;
            else if (attributeType.GetRuntimeProperty("Type") != null)
                returnType = responseTypeAttribute.Type;

            if (returnType == null)
                returnType = typeof(void);

            return returnType;
        }

        /// <summary>Creates a primitive parameter for the given parameter information reflection object.</summary>
        private async Task<SwaggerParameter> CreateBodyParameterAsync(SwaggerGenerator swaggerGenerator, string name, Type type, List<Attribute> attributes)
        {
            var isRequired = true;

            var typeDescription = JsonObjectTypeDescription.FromType(type, swaggerGenerator.ResolveContract(type), attributes, _settings.DefaultEnumHandling);
            var operationParameter = new SwaggerParameter {
                Name = name,
                Kind = SwaggerParameterKind.Body,
                IsRequired = isRequired,
                IsNullableRaw = typeDescription.IsNullable,
                Schema = await swaggerGenerator.GenerateAndAppendSchemaFromTypeAsync(type, !isRequired, attributes).ConfigureAwait(false),
            };

            operationParameter.Description = $"{name} request";

            return operationParameter;
        }

        private string TryGetStringPropertyValue(dynamic obj, string propertyName)
        {
            return ((object)obj)?.GetType().GetRuntimeProperty(propertyName) != null && !string.IsNullOrEmpty(obj.Name) ? obj.Name : null;
        }

        #endregion AddParameterFromAnnotation
    }
}