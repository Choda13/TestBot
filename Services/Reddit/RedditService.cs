using System;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord;


namespace TestBot.Services.Reddit
{
    public enum ReturnMessage { SubDoesntExist, ListFull, Successful, AlreadyAdded };
    public class Subreddit
    {
        bool ToUpdate;
        public string Name { get; }
        public List<RedditPost> Feed;
            
        public Subreddit()
        {
            Name = null;
            Feed = new List<RedditPost>();
            ToUpdate = true;
        }
        public Subreddit(string name)
        {
            this.Name = name;
            Feed = new List<RedditPost>();
            GetFeed();
        }
        public Subreddit(string name, List<RedditPost> Feed)
        {
            this.Name = name;
            this.Feed = Feed;
            if (Feed.Count == 0) ToUpdate = true;
            else ToUpdate = false;
        }
        void GetFeed(int limit=100)
        {
            if (limit > 1000) limit = 1000;
            StreamReader streamReader = new StreamReader(
                WebRequest.Create("https://reddit.com/r/" + Name + "/new.json?limit=" + limit)
                .GetResponse().GetResponseStream());
            PostListing response = new PostListing();
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, MissingMemberHandling = MissingMemberHandling.Ignore };
            response = JsonConvert.DeserializeObject<PostListing>(streamReader.ReadToEnd(), settings);
            /*try
            {
                response = JsonSerializer.Deserialize<PostListing>(streamReader.ReadToEnd());
            }
            catch
            {
                if (response == null || response.data == null)
                {
                    ToUpdate = true;
                    return;
                }
            }*/
            foreach (var it in response.data.children)
                Feed.Add(it);
            ToUpdate = false;
        }
        public RedditPost GetPost()
        {
            if (ToUpdate) GetFeed();
            var post = Feed[0];
            Feed.RemoveAt(0);
            if (Feed.Count == 0) ToUpdate = true;
            return post;
        }
    }
    public class RedditService
    {
        int limit;
        Subreddit mainSubreddit;
        List<Subreddit> Subreddits;
        public List<Subreddit> list { get => Subreddits; }
        public Subreddit Main { get => mainSubreddit;}

        public RedditService()
        {
            limit = 100;
            Subreddits = new List<Subreddit>();
            ConfigService();
        }

        public ReturnMessage AddSubreddit(string name)
        {
            if(Subreddits.Count() == limit) return ReturnMessage.ListFull;
            HttpWebResponse response;
            try
            {
                response = HttpWebRequest.Create("https://www.reddit.com/r/" + name).GetResponse() as HttpWebResponse;
            }
            catch
            {
                return ReturnMessage.SubDoesntExist;
            }
            if (response.StatusCode != HttpStatusCode.OK) return ReturnMessage.SubDoesntExist;
            if (Subreddits.Where(x => x.Name == name).Count() == 0)
                Subreddits.Add(new Subreddit(name));
            else
                return ReturnMessage.AlreadyAdded;
            if (Subreddits.Count == 1)
                mainSubreddit = Subreddits[0];
            return ReturnMessage.Successful;
        }
        public ReturnMessage RemoveSubreddit(string name)
        {
            for (int i = 0; i < Subreddits.Count; i++)
                if (Subreddits[i].Name == name)
                {
                    Subreddits.RemoveAt(i);
                    return ReturnMessage.Successful;
                }
            return ReturnMessage.SubDoesntExist;
        }
        public List<SubredditData> SearchSubbredit(string query)
        {
            StreamReader streamReader = new StreamReader(
               WebRequest.Create("https://www.reddit.com/search.json?q=" + query + "&type=sr&limit=10")
               .GetResponse().GetResponseStream());
            var listing = System.Text.Json.JsonSerializer.Deserialize<SubredditListing>(streamReader.ReadToEnd());
            List<SubredditData> list = new List<SubredditData>();
            foreach (var it in listing.data.children) list.Add(it.data);
            return list;
        }
        public ReturnMessage SetMainSubbredit(string name)
        {
            for (int i = 0; i < Subreddits.Count; i++)
                if (Subreddits[i].Name == name)
                {
                    if(Subreddits.Count == limit)
                    {
                        if (Subreddits[0] == mainSubreddit)
                            Subreddits.RemoveAt(1);
                        else
                            Subreddits.RemoveAt(0);
                    }
                    mainSubreddit = Subreddits[i];
                    return ReturnMessage.Successful;
                }
            return ReturnMessage.SubDoesntExist;
        }
        public RedditPost GetPost(string name = null)
        {
            if (Subreddits.Count == 0) return null;
            if(name == null)
            {
                var post = mainSubreddit.Feed[0];
                mainSubreddit.Feed.RemoveAt(0);
                return post;
            }
            var subreddit = Subreddits.First(x => x.Name == name);
            if (subreddit == null) return null;
            return subreddit.GetPost();
        }
        void ConfigService()
        {
            StreamReader reader;
            try {reader = new StreamReader("C:\\Users\\JOHN_SNOW\\source\\repos\\TestBot\\subreddits.txt");}
            catch { return; }

            string line;
            while((line = reader.ReadLine()) != null)
            {
                var message = AddSubreddit(line);
                if (message != ReturnMessage.Successful)
                    Console.WriteLine("" + Enum.GetName(typeof(ReturnMessage), message));
            }
        }

        public static EmbedBuilder CreateEmbedBuilderPost(RedditPost post)
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithColor(Color.Red)
                .WithTitle(post.data.title)
                .WithAuthor(new EmbedAuthorBuilder()
                {
                    Name = post.data.author,
                    //IconUrl = post.data.thumbnail,
                    Url = "https://www.reddit.com" + post.data.permalink
                })
                .WithDescription(post.data.selftext)
                .WithImageUrl(post.data.url)
                .WithFooter("Kurac")
                .WithCurrentTimestamp();
                
            return builder;
        }
    }
}
