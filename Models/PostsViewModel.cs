using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApp.Entity;

namespace BlogApp.Models
{
    public class PostsViewModel
    {
        public List<Post> Posts { get; set; } = new();
        
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems  { get; set; }

        public int TotalPages =>(int)Math.Ceiling((decimal)TotalItems/PageSize);
        //filitre bilgisi
        public string? CurrentTag { get; set; }    
    }
}