using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Icomm.AspNetCore.DTO.Requests;
using Icomm.AspNetCore.Extensions;
using Icomm.Hosting.Contracts;
using Icomm.Infrastructure.Filters;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Icomm.Infrastructure.Installers
{
	internal class RegisterSwagger : IServiceRegistration
	{
		public void RegisterAppServices(IServiceCollection services, IConfiguration config)
		{
			services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());
			services.AddSwaggerExamplesFromAssemblies(Assembly.GetAssembly(typeof(ListQueryExample)));
			//Register Swagger
			//See: https://www.scottbrady91.com/Identity-Server/ASPNET-Core-Swagger-UI-Authorization-using-IdentityServer4
			services.AddSwaggerGen(options =>
			{
				options.SwaggerDoc("v1", new OpenApiInfo { Title = "Icomm.API.SmartCity.Sensor API", Version = "v1" });
				options.SchemaFilter<SwaggerIgnoreFilter>();
				options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Scheme = "bearer",
					Description = "Enter 'Bearer' following by space and JWT.",
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					In = ParameterLocation.Header,
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					}
				});

				// Set the comments path for the Swagger JSON and UI.
				foreach (var fi in Directory.EnumerateFiles(System.AppContext.BaseDirectory, "*.xml"))
				{
					options.IncludeXmlComments(fi, true);
				}

				options.DocumentFilter<SwaggerAddEnumDescriptions>();
				// [SwaggerRequestExample] & [SwaggerResponseExample]
				// version < 3.0 like this: c.OperationFilter<ExamplesOperationFilter>();
				// version 3.0 like this: c.AddSwaggerExamples(services.BuildServiceProvider());
				// version > 4.0 like this:
				options.ExampleFilters();

				options.EnableAnnotations();

				//options.AddFluentValidationRules();

				options.AddSecurityDefinitionFromConfiguration(config);

				//options.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();
				options.OperationFilter<AuthorizeCheckOperationFilter>();
			});
			services.AddSwaggerGenNewtonsoftSupport();
		}
	}

	public class SwaggerAddEnumDescriptions : IDocumentFilter
	{
		public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
		{
			// add enum descriptions to result models
			foreach (var property in swaggerDoc.Components.Schemas.Where(x => x.Value?.Enum?.Count > 0))
			{
				IList<IOpenApiAny> propertyEnums = property.Value.Enum;
				if (propertyEnums != null && propertyEnums.Count > 0)
				{
					property.Value.Description += DescribeEnum(propertyEnums, property.Key);
				}
			}

			// add enum descriptions to input parameters
			foreach (var pathItem in swaggerDoc.Paths.Values)
			{
				DescribeEnumParameters(pathItem.Operations, swaggerDoc);
			}
		}

		private void DescribeEnumParameters(IDictionary<OperationType, OpenApiOperation> operations, OpenApiDocument swaggerDoc)
		{
			if (operations != null)
			{
				foreach (var oper in operations)
				{
					foreach (var param in oper.Value.Parameters)
					{
						var paramEnum = swaggerDoc.Components.Schemas.FirstOrDefault(x => x.Key == param.Name);
						if (paramEnum.Value != null)
						{
							param.Description += DescribeEnum(paramEnum.Value.Enum, paramEnum.Key);
						}
					}
				}
			}
		}

		private Type GetEnumTypeByName(string enumTypeName)
		{
			return AppDomain.CurrentDomain
				.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.FirstOrDefault(x => x.Name == enumTypeName);
		}

		private string DescribeEnum(IList<IOpenApiAny> enums, string proprtyTypeName)
		{
			List<string> enumDescriptions = new();
			var enumType = GetEnumTypeByName(proprtyTypeName);
			if (enumType == null)
				return null;

			foreach (OpenApiInteger enumOption in enums)
			{
				int enumInt = enumOption.Value;

				enumDescriptions.Add(string.Format("{0} = {1}", enumInt, Enum.GetName(enumType, enumInt)));
			}

			return string.Join(", ", enumDescriptions.ToArray());
		}
	}
}