﻿using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pertuk.Business.BunnyCDN;
using Pertuk.Business.Filters;
using Pertuk.Business.Options;
using Pertuk.Business.Services.Abstract;
using Pertuk.Business.Services.Concrete;
using Pertuk.Business.Validators.Auth;
using Pertuk.DataAccess.Repositories.Abstract;
using Pertuk.DataAccess.Repositories.Concrete;
using Pertuk.Dto.Requests.Auth;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Pertuk.Business.Installers
{
    public class MvcInstaller : IBaseInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers()
                .AddMvcOptions(options =>
            {
                options.Filters.Add<ValidationFilter>();
                options.Filters.Add<ExceptionFilter>();
            })
                .AddFluentValidation(config => config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            services.AddAuthorization(configure =>
            {
                configure.AddPolicy("SendEmailConfirmationPolicy", x => x.RequireAuthenticatedUser());
            });

            SwaggerConfiguration(services, configuration);

            JwtConfiguration(services, configuration);

            ConfigureDependencies(services);

            OptionsConfiguration(services, configuration);
        }

        #region Private Functions

        private void SwaggerConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            var swaggerOptions = new SwaggerOption();
            configuration.GetSection(nameof(SwaggerOption)).Bind(swaggerOptions);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerOptions.Version, new OpenApiInfo { Title = swaggerOptions.Title, Version = swaggerOptions.Version, Description = swaggerOptions.Description });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
        }

        private void JwtConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var jwtOption = new JwtOption();
                configuration.GetSection(nameof(JwtOption)).Bind(jwtOption);

                services.AddSingleton(jwtOption);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtOption.Issuer,
                    ValidAudience = jwtOption.Audience,
                    RequireExpirationTime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOption.Secret)),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true
                };
            });
        }

        private void ConfigureDependencies(IServiceCollection services)
        {
            #region Scopes

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUploadImageService, UploadImageService>();

            services.AddScoped<IStudentUsersRepository, StudentUsersRepository>();
            services.AddScoped<ITeacherUsersRepository, TeacherUsersRepository>();
            services.AddScoped<IBannedUsersRepository, BannedUsersRepository>();
            services.AddScoped<IDeletedUsersRepository, DeletedUsersRepository>();

            services.AddScoped<BunnyCDNService>();

            #endregion

            #region Transients

            services.AddTransient<IValidator<LoginRequestModel>, LoginRequestValidator>();
            services.AddTransient<IValidator<ResetPasswordRequestModel>, ResetPasswordValidator>();
            services.AddTransient<IValidator<ConfirmEmailRequestModel>, ConfirmEmailValidator>();
            services.AddTransient<IValidator<ForgotPasswordRequestModel>, ForgotPasswordValidator>();
            services.AddTransient<IValidator<StudentUserRegisterRequestModel>, StudentUserRegisterRequestValidator>();
            services.AddTransient<IValidator<TeacherUserRegisterRequestModel>, TeacherUserRegisterRequestValidator>();

            services.AddTransient<IEmailSender, EmailSender>();

            #endregion
        }

        private void OptionsConfiguration(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MediaOptions>(options => configuration.GetSection(nameof(MediaOptions)).Bind(options));
        }

        #endregion
    }
}
