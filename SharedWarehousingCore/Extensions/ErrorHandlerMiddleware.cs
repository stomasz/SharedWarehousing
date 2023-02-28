using System.Security.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SharedWarehousingCore.Helpers;

namespace SharedWarehousingCore.Extensions;
public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
           
            try
            {
                await _next(context);
            }
            catch (BasicException ex)
            {
                await PrepareError(context, ex, ex.Message);
            }
            catch (FormatException ex)
            {
                await PrepareError(context, ex, "Wprowadzone dane są niezgodne");

            }
            catch (ArgumentOutOfRangeException ex)
            {
                await PrepareError(context, ex, "Wprowadzone dane są niezgodne");
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException)
                {
                    switch (sqlException.Number)
                    {
                        case 2601:
                        case 2627:
                            await PrepareError(context, ex, "Wprowadzone dane są niezgodne");
                            break;
                    }
                }

                throw;
            }
            catch (InvalidCredentialException ex)
            {
                await PrepareError(context, ex, "Invalid credentials");
            }
            catch (NullReferenceException ex)
            {
                await PrepareError(context, ex, "Wewnętrzny błąd aplikacji");
            }
            
            catch (ArgumentNullException ex)
            {
                await PrepareError(context, ex, "Wewnętrzny błąd aplikacji");
            }

            // catch (Exception ex)
            // {
            //     await PrepareError(context, ex, "Wewnętrzny błąd aplikacji");
            // }
        }

        private async Task PrepareJson(HttpContext context, string message ,string stack)
        {
            context.Response.Clear();
            context.Response.Headers.Add("Content-Type", "application/json");
            context.Response.StatusCode = 500;
            string json = JsonConvert.SerializeObject(new { message = message, statusCode = 500, stack = stack });


            await context.Response.WriteAsync(json);
        }

        private async Task PrepareError(HttpContext context, Exception ex, string message)
        {
            if (!_env.IsDevelopment())
            {
                await PrepareJson(context, message, null);
                _logger.LogError(ex, ex.ToString());
            }
            else
            {
                await PrepareJson(context, message, ex.ToString());
                _logger.LogError(ex, ex.ToString());
            }
        }
    }
