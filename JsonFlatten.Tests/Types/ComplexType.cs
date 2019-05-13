using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonFlatten.Tests.Types
{
    public class Author
    {

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("email")]
        public bool Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("nice_name")]
        public string NiceName { get; set; }

        [JsonProperty("URL")]
        public string URL { get; set; }

        [JsonProperty("avatar_URL")]
        public string AvatarURL { get; set; }

        [JsonProperty("profile_URL")]
        public string ProfileURL { get; set; }

        [JsonProperty("ip_address")]
        public bool IpAddress { get; set; }

        [JsonProperty("site_ID")]
        public int SiteID { get; set; }

        [JsonProperty("site_visible")]
        public bool SiteVisible { get; set; }
    }

    public class Tags
    {
    }

    public class Links
    {
        [JsonProperty("self")]
        public string Self { get; set; }
        [JsonProperty("help")]
        public string Help { get; set; }
        [JsonProperty("site")]
        public string Site { get; set; }
        [JsonProperty("replies")]
        public Uri Replies { get; set; }
        [JsonProperty("likes")]
        public Uri Likes { get; set; }
    }

    public class Meta
    {
        [JsonProperty("links")]
        public Links Links { get; set; }
    }

    public class Goat
    {

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("post_count")]
        public int PostCount { get; set; }

        [JsonProperty("feed_url")]
        public string FeedUrl { get; set; }

        [JsonProperty("parent")]
        public int Parent { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class Goatchild
    {

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("post_count")]
        public int PostCount { get; set; }

        [JsonProperty("feed_url")]
        public string FeedUrl { get; set; }

        [JsonProperty("parent")]
        public int Parent { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class Hobo
    {

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("post_count")]
        public int PostCount { get; set; }

        [JsonProperty("feed_url")]
        public string FeedUrl { get; set; }

        [JsonProperty("parent")]
        public int Parent { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class Uncategorized
    {

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("post_count")]
        public int PostCount { get; set; }

        [JsonProperty("feed_url")]
        public string FeedUrl { get; set; }

        [JsonProperty("parent")]
        public int Parent { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class Categories
    {

        [JsonProperty("goat")]
        public Goat Goat { get; set; }

        [JsonProperty("goatchild")]
        public Goatchild Goatchild { get; set; }

        [JsonProperty("hobo")]
        public Hobo Hobo { get; set; }

        [JsonProperty("Uncategorized")]
        public Uncategorized Uncategorized { get; set; }
    }

    public class Attachments
    {
    }

    public class CurrentUserCan
    {

        [JsonProperty("publish_post")]
        public bool PublishPost { get; set; }

        [JsonProperty("delete_post")]
        public bool DeletePost { get; set; }

        [JsonProperty("edit_post")]
        public bool EditPost { get; set; }
    }

    public class Capabilities
    {

        [JsonProperty("publish_post")]
        public bool PublishPost { get; set; }

        [JsonProperty("delete_post")]
        public bool DeletePost { get; set; }

        [JsonProperty("edit_post")]
        public bool EditPost { get; set; }
    }

    public class Post
    {

        [JsonProperty("ID")]
        public int ID { get; set; }

        [JsonProperty("site_ID")]
        public int SiteID { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("modified")]
        public DateTime Modified { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("URL")]
        public string URL { get; set; }

        [JsonProperty("short_URL")]
        public string ShortURL { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("excerpt")]
        public string Excerpt { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("guid")]
        public string Guid { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("sticky")]
        public bool Sticky { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("parent")]
        public bool Parent { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("comments_open")]
        public bool CommentsOpen { get; set; }

        [JsonProperty("pings_open")]
        public bool PingsOpen { get; set; }

        [JsonProperty("likes_enabled")]
        public bool LikesEnabled { get; set; }

        [JsonProperty("sharing_enabled")]
        public bool SharingEnabled { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("like_count")]
        public int LikeCount { get; set; }

        [JsonProperty("i_like")]
        public bool ILike { get; set; }

        [JsonProperty("is_reblogged")]
        public bool IsReblogged { get; set; }

        [JsonProperty("is_following")]
        public bool IsFollowing { get; set; }

        [JsonProperty("global_ID")]
        public string GlobalID { get; set; }

        [JsonProperty("featured_image")]
        public string FeaturedImage { get; set; }

        [JsonProperty("post_thumbnail")]
        public object PostThumbnail { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("geo")]
        public bool Geo { get; set; }

        [JsonProperty("menu_order")]
        public int MenuOrder { get; set; }

        [JsonProperty("publicize_URLs")]
        public IList<object> PublicizeURLs { get; set; }

        [JsonProperty("tags")]
        public Tags Tags { get; set; }

        [JsonProperty("categories")]
        public Categories Categories { get; set; }

        [JsonProperty("attachments")]
        public Attachments Attachments { get; set; }

        [JsonProperty("metadata")]
        public bool Metadata { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }

        [JsonProperty("current_user_can")]
        public CurrentUserCan CurrentUserCan { get; set; }

        [JsonProperty("capabilities")]
        public Capabilities Capabilities { get; set; }
    }

    public class ComplexType
    {

        [JsonProperty("found")]
        public int Found { get; set; }

        [JsonProperty("posts")]
        public IList<Post> Posts { get; set; }
    }
}
