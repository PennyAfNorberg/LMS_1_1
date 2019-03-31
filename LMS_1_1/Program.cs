using LMS_1_1.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LMS_1_1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                context.Database.Migrate();

                var config = host.Services.GetRequiredService<IConfiguration>();

                // dotnet user-secrets set  "GYM:AdminPW" "FooBar77!"
                /*  {
  "Teachers": [
    {
      "UserName": "Penny@lysator.liu.se",
      "PassWord": "LmS2019Penny#"
    }
  ],
  "Students": [
    {
      "UserName": "elev@lexicon.se",
      "PassWord": "Qwe12rTy#"
    }
  ]
                 * 
                 * 
                 */
                var Teachers = new List<UserData>
                {
                    new UserData{ UserName = config["Teachers:0:UserName"],
                            PassWord = config["Teachers:0:PassWord"],
                            FirstName=config["Teachers:0:FirstName"],
                            LastName=config["Teachers:0:LastName"],

                    }
                    

                };
                int i = 1;
                string s = config["Teachers:" + i.ToString() + ":UserName"];
                while (!String.IsNullOrEmpty(s))
                {
                    Teachers.Add(new UserData
                    {
                        UserName = s,
                        PassWord = config["Teachers:" + i.ToString() + ":PassWord"],
                        FirstName = config["Teachers:" + i.ToString() + ":FirstName"],
                        LastName = config["Teachers:" + i.ToString() + ":LastName"]
                    }

                        );

                    i++;
                    s = config["Teachers:" + i.ToString() + ":UserName"];
                }

                var Students = new List<UserData>
                {
                   new UserData{ UserName = config["Students:0:UserName"],
                        PassWord = config["Students:0:PassWord"],
                        FirstName=config["Student:0:FirstName"],
                        LastName=config["Student:0:LastName"],

                   }
                };
                i = 1;
                s = config["Students:" + i.ToString() + ":UserName"];
                while (!String.IsNullOrEmpty(s))
                {
                    Students.Add(new UserData
                    {
                        UserName = s,
                        PassWord = config["Students:" + i.ToString() + ":PassWord"],
                        FirstName = config["Students:" + i.ToString() + ":FirstName"],
                        LastName = config["Students:" + i.ToString() + ":LastName"]
                    }

                        );

                    i++;
                    s = config["Students:" + i.ToString() + ":UserName"];
                }


                try
                {
                    SeedData.Initialize(services, Teachers.ToArray(), Students.ToArray()).Wait();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex.Message, "An error occurred seeding the DB.");
                }
            }



            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }



}
