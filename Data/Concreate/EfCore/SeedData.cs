using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions; // Regex için ekleyin

namespace BlogApp.Data.Concreate.EfCore
{
    public static class SeedData
    {
        // Title'dan URL oluşturan yardımcı bir metod ekleyebilirsiniz
        private static string GenerateSlug(string title)
        {
            var slug = title.ToLower();
            slug = Regex.Replace(slug, @"&+", "and");
            slug = slug.Replace(" ", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); // Sadece harf, rakam, boşluk ve tire kalır
            slug = Regex.Replace(slug, @"\s+", "-"); // Birden fazla boşluğu tek tireye çevir
            slug = slug.Trim('-'); // Baş ve sondaki tireleri kaldır
            return slug;
        }

        public static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();
            if (context != null)
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }

                List<Tag> tags = new List<Tag>();
                if (!context.Tags.Any())
                {
                    tags.AddRange(new Tag[]
                    {
                        new Tag { Text = "Web programlama", Url = GenerateSlug("Web programlama"), Color = TagColors.primary },
                        new Tag { Text = "backend", Url = GenerateSlug("backend"), Color = TagColors.secondary },
                        new Tag { Text = "frontend", Url = GenerateSlug("frontend"), Color = TagColors.danger },
                        new Tag { Text = "fullstack", Url = GenerateSlug("fullstack"), Color = TagColors.info },
                        new Tag { Text = "php", Url = GenerateSlug("php"), Color = TagColors.dark }
                    });
                    context.Tags.AddRange(tags);
                    context.SaveChanges();
                } else {
                    tags = context.Tags.ToList(); // Eğer zaten varsa, mevcut etiketleri al
                }

                List<User> users = new List<User>();
                if (!context.Users.Any())
                {
                    users.AddRange(new User[]
                    {
                        new User {
                            UserName = "ibrahim",
                            Name = "ibrahim",
                            Email = "iserindag@msn.com",
                            Password = "123456", // Gerçek uygulamada hash'lenmelidir
                            Image = "p1.jpg"
                        },
                        new User {
                            UserName = "halil",
                            Name = "halil",
                            Email = "serindag@msn.com",
                            Password = "123456", // Gerçek uygulamada hash'lenmelidir
                            Image = "p2.jpg"
                        }
                    });
                    context.Users.AddRange(users);
                    context.SaveChanges();
                } else {
                    users = context.Users.ToList(); // Eğer zaten varsa, mevcut kullanıcıları al
                }

                if (!context.Posts.Any())
                {
                    var ibrahimUser = users.FirstOrDefault(u => u.UserName == "ibrahim");
                    var halilUser = users.FirstOrDefault(u => u.UserName == "halil");

                    context.Posts.AddRange(
                        new Post
                        {
                            Title = "Asp.net Core",
                            Description = "Asp.net core dersleri",
                            Content = "Asp.net core dersleri",
                            Url = GenerateSlug("Asp.net Core"),
                            IsActive = true,
                            Image = "1.jpg",
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = tags.Take(3).ToList(),
                            UserId = ibrahimUser?.UserId ?? 1, // Dinamik olarak kullanıcı ID'sini al
                            Comments = new List<Comment>
                            {
                                new Comment { Text = "İyi bir kurs", PublishedOn = DateTime.Now.AddDays(-5), UserId = ibrahimUser?.UserId ?? 1 },
                                new Comment { Text = "Çok Faydalandığım bir kurs", PublishedOn = DateTime.Now.AddDays(-3), UserId = halilUser?.UserId ?? 2 }
                            },
                        },
                        new Post
                        {
                            Title = "Php",
                            Description = "Php dersleri",
                            Content = "Php dersleri",
                            Url = GenerateSlug("Php"),
                            IsActive = true,
                            Image = "2.jpg",
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = tags.Take(2).ToList(),
                            UserId = ibrahimUser?.UserId ?? 1,
                        },
                        new Post
                        {
                            Title = "Django",
                            Description = "Django dersleri",
                            Content = "Django dersleri",
                            Url = GenerateSlug("Django"),
                            IsActive = true,
                            Image = "3.jpg",
                            PublishedOn = DateTime.Now.AddDays(-30),
                            Tags = tags.Skip(1).Take(4).ToList(), // Farklı etiketler kullanmak için
                            UserId = ibrahimUser?.UserId ?? 1,
                        },
                        new Post
                        {
                            Title = "React",
                            Description = "React dersleri",
                            Content = "React dersleri",
                            Url = GenerateSlug("React"),
                            IsActive = true,
                            Image = "3.jpg",
                            PublishedOn = DateTime.Now.AddDays(-40),
                            Tags = tags.Skip(2).Take(4).ToList(),
                            UserId = ibrahimUser?.UserId ?? 1,
                        },
                        new Post
                        {
                            Title = "Angular",
                            Description = "Angular dersleri",
                            Content = "Angular dersleri",
                            Url = GenerateSlug("Angular"),
                            IsActive = true,
                            Image = "3.jpg",
                            PublishedOn = DateTime.Now.AddDays(-50),
                            Tags = tags.Take(4).ToList(),
                            UserId = ibrahimUser?.UserId ?? 1,
                        },
                        new Post
                        {
                            Title = "Web Tasarım",
                            Description = "Web Tasarım dersleri",
                            Content = "Web Tasarım dersleri",
                            Url = GenerateSlug("Web Tasarım"),
                            IsActive = true,
                            Image = "3.jpg",
                            PublishedOn = DateTime.Now.AddDays(-60),
                            Tags = tags.Skip(1).Take(3).ToList(),
                            UserId = ibrahimUser?.UserId ?? 1,
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}