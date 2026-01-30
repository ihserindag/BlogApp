using System.Data;
using System.Security.Claims;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concreate.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{


    public class PostsController : Controller
    {
        private IPostRepository _postRepository;
        private ITagRepository _tagRepository;
        private ICommentRepository _commentRepository;


        public PostsController(IPostRepository postRepository, ITagRepository tagRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _tagRepository = tagRepository;
            _commentRepository = commentRepository;
        }


        public async Task<IActionResult> Index(string? tag=null, int page=1, int pageSize=5)
        {
            var clais = User.Claims;
            IQueryable<Post> posts = _postRepository.Posts
                .Where(x => x.IsActive == true)
                .Include(t => t.Tags)
                .Include(x => x.User)
                .Include(x => x.Comments);
            ViewBag.Tag = "Popiler Postlar";

            if (!string.IsNullOrEmpty(tag))
            {
                
                posts = posts.Where(x => x.Tags.Any(y => y.Url == tag));
                 var tags = await _tagRepository.Tags.FirstOrDefaultAsync(x => x.Url == tag);
                ViewBag.Tag = tags.Text+" ile ilgili Postlar";
            }

            var totalItems=await posts.CountAsync();

            var postsOnPage=await posts
            .OrderByDescending(x=>x.PostId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var model = new PostsViewModel
            {
                Posts = postsOnPage,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                CurrentTag = tag


            };

            return View(model);
        }

        public async Task<IActionResult> Details(string? url)
        {
            return View(await _postRepository.
            Posts.
            Include(x => x.User).
            Include(x => x.Tags).
            Include(y => y.Comments).
            ThenInclude(u => u.User).
            FirstOrDefaultAsync(x => x.Url == url));
        }

        [HttpPost]
        public JsonResult AddComment(int PostId, string Text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var image = User.FindFirstValue(ClaimTypes.UserData);
            var name = User.FindFirstValue(ClaimTypes.GivenName);

            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIdInt))
            {
                return Json(new { error = "Kullanıcı girişi gerekli" });
            }

            var entity = new Comment
            {
                Text = Text,
                PublishedOn = DateTime.Now,
                PostId = PostId,
                UserId = userIdInt

            };
            _commentRepository.CreateComment(entity);


            return Json(new
            {
                userName,
                name,
                Text,
                entity.PublishedOn,
                image
            });
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
                {
                    TempData["ToastType"] = "error";
                    TempData["ToastMessage"] = "Kullanıcı bilgisi alınamadı.";
                    return RedirectToAction("Login", "Users");
                }
                
                _postRepository.CreatePost(
                    new Post
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Content = model.Content,
                        Url = model.Url,
                        UserId = userId,
                        PublishedOn = DateTime.Now,
                        Image = "1.jpg",
                        IsActive = false,

                    }
                );
                 TempData["ToastType"] = "success";
                TempData["ToastMessage"] = "Post başarıyla eklendi.";
                return RedirectToAction("Index", "Posts");
            }
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Post eklenemedi";
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return RedirectToAction("Login", "Users");
            }
            
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = _postRepository.Posts;
            if (string.IsNullOrEmpty(role))
            {
                posts = posts.Where(x => x.UserId == userId);
            }

            return View(await posts.ToListAsync());

        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = _postRepository.Posts.Include(x => x.Tags).FirstOrDefault(i => i.PostId == id);
            if (post == null)
            {
                return NotFound();
            }
            ViewBag.Tags = _tagRepository.Tags.ToList();
            return View(new PostCreateViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Url = post.Url,
                IsActive = post.IsActive ?? false,
                Tags = post.Tags



            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(PostCreateViewModel model, int[] tagIds)
        {
            if (ModelState.IsValid)
            {
                var entityToUpdate = new Post
                {
                    PostId = model.PostId,
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    Url = model.Url,


                };
                if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                {
                    entityToUpdate.IsActive = model.IsActive;
                }
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Post başarıyla düzenlendi.";
                _postRepository.EditPost(entityToUpdate, tagIds);
                return RedirectToAction("List", "Posts");
            }
             TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "İşlem tamamlanmadı";
            ViewBag.Tags = _tagRepository.Tags.ToList();
            return View(model);

        }
        [Authorize] 
        public IActionResult Delete(int? id)
        {
            
            if (id == null)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Silinecek kayıt bulunamadı.";
                return NotFound(); // id gelmediyse işlem yapılmaz
            }

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Post başarıyla silindi.";
            _postRepository.DeletePost(id.Value); // kesin int gönderiyoruz

            return RedirectToAction("List", "Posts");
        }

    }


}
