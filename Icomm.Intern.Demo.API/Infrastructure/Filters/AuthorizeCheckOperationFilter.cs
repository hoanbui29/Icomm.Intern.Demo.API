﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace Icomm.Infrastructure.Filters
{
	public class AuthorizeCheckOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var hasAuthorize =
			  context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
			  || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any();

			if (hasAuthorize)
			{
				operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
				operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

				operation.Security = new List<OpenApiSecurityRequirement>
				{
					new OpenApiSecurityRequirement
					{
						{   new OpenApiSecurityScheme {
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "oauth2"
								}
							}, new List<string>()
						}
					},
					new OpenApiSecurityRequirement
					{
						{
							new OpenApiSecurityScheme {
								Reference = new OpenApiReference
								{
									Type = ReferenceType.SecurityScheme,
									Id = "Bearer"
								}
							}, new List<string>()
						}
					}
				};
			}
		}
	}
}