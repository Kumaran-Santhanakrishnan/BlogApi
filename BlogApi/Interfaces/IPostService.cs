using System;
using System.Collections.Generic;
using BlogApi.Models;

namespace BlogApi.Interfaces
{

    public interface IPostService
    {
        internal List<Blog> getAllPosts();

        public Blog createPost(PostDTO post);

        public Blog EditPost(PostDTO post);

        public Blog? GetPostById(string id);

        public Boolean LikePostById(string id,string userId);

        public string UnlikePostById(string id, string userId);

        public List<Comments> GetCommentsByPostId(string id);

        public List<Comments> GetRepliesByCommentId(string id);

        public Comments AddCommentByPostId(CommentDTO comment);

        public Comments EditComment(CommentDTO comment);

        public Boolean DeleteCommentById(string id);
    }


}