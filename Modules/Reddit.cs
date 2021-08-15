using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using TestBot.Services;
using TestBot.Services.Reddit;

namespace TestBot.Modules
{
    public class Reddit : ModuleBase<SocketCommandContext>
    {
        public RedditService RedditService { get; set; }
        [Command("meme")]
        public async Task SendMemeAsync(int num=1)
        {
            if (num > 10) num = 10;
            for(int i=0; i<num; i++)
            {
                var post = RedditService.GetPost();
                if (post.data.spoiler)
                {
                    var response = WebRequest.Create(post.data.url).GetResponse();
                    await ReplyAsync(post.data.title + "\n" + post.data.selftext);
                    await Context.Channel.SendFileAsync(stream: response.GetResponseStream(), filename:"meme.png", isSpoiler:true);
                    return;
                }
                var builder = RedditService.CreateEmbedBuilderPost(post);
                await ReplyAsync(embed: builder.Build());
            }
        }
        [Command("dodaj")]
        public async Task AddSubredditAsync(string name)
        {
            await ReplyAsync("Lol");
            var msg = await Task.Run(() => RedditService.AddSubreddit(name));
            await ReplyAsync("Lol");
            if (msg != ReturnMessage.Successful)
                await ReplyAsync("Rođače ne mogu da dodam ovaj kurac [" + Enum.GetName(typeof(ReturnMessage), msg) + "]");
            else
                await ReplyAsync("Dodao sam gazda");
        }
        [Command("salji")]
        public async Task GetPost(string name=null)
        {
            var post = RedditService.GetPost(name);
            if(post == null)
            {
                await ReplyAsync("Idiote taj subreddit nije dodat");
            }
            var embedPost = RedditService.CreateEmbedBuilderPost(post).Build();
            await ReplyAsync(embed:embedPost) ;
        }
        [Command("listaj")]
        public async Task ListSubreddits()
        {
            var embed = new EmbedBuilder();
            int j = 0;
            string msg=string.Empty;
            foreach(var i in RedditService.list)
            {
                msg += i.Name + "\n";
            }
            embed.Description = msg;
            await ReplyAsync(embed: embed.Build());
        }
    }
}
