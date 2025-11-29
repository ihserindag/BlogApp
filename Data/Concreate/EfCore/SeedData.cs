using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concreate.EfCore
{
    public static class SeedData
    {
        public static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();
            if (context != null)
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
                if (!context.Tags.Any())
                {
                    context.Tags.AddRange(
                        new Tag { Text = "Web programlama",Url="web-programlama",Color=TagColors.primary },
                        new Tag { Text = "backend" ,Url="backend", Color=TagColors.secondary },
                        new Tag { Text = "frontend", Url="frontend", Color=TagColors.danger },
                        new Tag { Text = "fullstack", Url="fullstack",Color=TagColors.info },
                        new Tag { Text = "php", Url="php",Color=TagColors.dark }
                    );
                    context.SaveChanges();
                }
                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new User {
                            UserName = "ibrahim",
                            Name = "ibrahim",
                            Email="iserindag@msn.com",
                            Password = "123456",
                            Image = "p1.jpg"
                        },
                        new User {
                            UserName = "halil",
                            Name = "halil",
                            Email="serindag@msn.com",
                            Password = "123456",
                            Image = "p2.jpg"
                        }
                    );
                    context.SaveChanges();
                }
                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Post
                        {
                            Title = "Asp.net Core",
                            Description="Asp.net core dersleri",
                            Content = "Asp.net core dersleri",
                            Url = "asp-net-core",
                            IsActive = true,
                            Image = "1.jpg",
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(3).ToList(),
                            UserId = 1,
                            Comments = new List<Comment>
                            {
                                // Örneğin, o anki tarihten biraz geriye alarak:
                                new Comment { Text = "İyi bir kurs", PublishedOn = DateTime.Now.AddDays(-5), UserId = 1 },
                                new Comment { Text = "Çok Faydalandığım bir kurs", PublishedOn = DateTime.Now.AddDays(-3), UserId = 2 }
                            },


                        },
                                                new Post
                            {
                                Title = "Php",
                                Description = "Php dersleri",
                                Content = "Php dersleri",
                                Url = "php",
                                IsActive = true,
                                Image = "2.jpg",
                                PublishedOn = DateTime.Now.AddDays(-20),
                                Tags = context.Tags.Take(2).ToList(),
                                UserId = 1,
                            },

                            new Post
                            {
                                Title = "Django",
                                Description = "Django dersleri",
                                Content = "Django dersleri",
                                Url = "django",
                                IsActive = true,
                                Image = "3.jpg",
                                PublishedOn = DateTime.Now.AddDays(-30),
                                Tags = context.Tags.Take(4).ToList(),
                                UserId = 1,
                            },

                            new Post
                            {
                                Title = "React",
                                Description = "React dersleri",
                                Content = "React dersleri",
                                Url = "react",
                                IsActive = true,
                                Image = "3.jpg",
                                PublishedOn = DateTime.Now.AddDays(-40),
                                Tags = context.Tags.Take(4).ToList(),
                                UserId = 1,
                            },
                            new Post
                            {
                                Title = "Angular",
                                Description = "Angular dersleri",
                                Content = "Angular dersleri",
                                Url = "angular",
                                IsActive = true,
                                Image = "3.jpg",
                                PublishedOn = DateTime.Now.AddDays(-50),
                                Tags = context.Tags.Take(4).ToList(),
                                UserId = 1,
                            },

                            new Post
                            {
                                Title = "Web Tasarım",
                                Description = "Web Tasarım dersleri",
                                Content = "Web Tasarım dersleri",
                                Url = "web-tasarım",
                                IsActive = true,
                                Image = "3.jpg",
                                PublishedOn = DateTime.Now.AddDays(-60),
                                Tags = context.Tags.Take(4).ToList(),
                                UserId = 1,
                            }


                    );
                    context.SaveChanges();
                }
            }
        }
    }
}