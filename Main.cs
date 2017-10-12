using System;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Alejo_Bot_V4_Core
{
    public class TwitchAPIConnection
    {
        private string CID;
        public TwitchAPIConnection(string CID)
        {
            this.CID = CID;

            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/v5/");
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
            }
            catch (WebException Ex)
            {
                throw new AIOExceptions.InvalidCustomerIDException(Ex);
            }
        }

        /// <summary>
        /// Gets the channel object for the given ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="OAuth"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Channel Channels(string ID, string OAuth = null)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + ID);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            if (OAuth != null)
            {
                Request.Headers.Add("Authorization: OAuth " + OAuth);
            }
            Request.Method = "GET";

            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                return JsonConvert.DeserializeObject<AIOObjects.Twitch.Channel>(Response.ReadToEnd());
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUserIDException(Ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the users that follow the account with the given ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="cursor"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Followers Followers(string ID, string cursor = "", int count = 100)
        {
            HttpWebRequest Request;
            if (cursor != "")
            {
                Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + ID + "/follows?limit=" + count + "&cursor=" + cursor);
            }
            else
            {
                Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + ID + "/follows?limit=" + count);
            }
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                string ResponseString = Response.ReadToEnd();
                return JsonConvert.DeserializeObject<AIOObjects.Twitch.Followers>(ResponseString);
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUserIDException(Ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the subscribers of the account with the given ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="OAuth"></param>
        /// <param name="cursor"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Subscribers Subscribers(string ID, string OAuth, string cursor = "", int count = 100)
        {
            HttpWebRequest Request;
            if (cursor != "")
            {
                Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + ID + "/subscriptions?cursor=" + cursor + "&limit=" + count);
            }
            else
            {
                Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + ID + "/subscriptions?limit=" + count);
            }
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: {0}" + CID);
            Request.Headers.Add("Authorization: OAuth " + OAuth);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                string ResponseString = Response.ReadToEnd();
                return JsonConvert.DeserializeObject<AIOObjects.Twitch.Subscribers>(ResponseString);
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUserIDException(Ex);
            }
            return null;
        }

        /// <summary>
        /// Gets the user ID for the given community
        /// </summary>
        /// <param name="CommunityName"></param>
        /// <returns></returns>
        public string GetIdFromCommunityName(string CommunityName)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/communities?name=" + CommunityName);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                return JObject.Parse(Response.ReadToEnd())["_id"].ToString();
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUsernameException(Ex);
                else Console.WriteLine(Ex.Message);
            }
            return null;
        }
        /// <summary>
        /// Gets the community object of the given community name
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Community GetCommunityByName(string Name)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/communities?name=" + Name);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";

            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                return JsonConvert.DeserializeObject<AIOObjects.Twitch.Community>(Response.ReadToEnd());
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUserIDException(Ex);
            }
            return null;
        }
        /// <summary>
        /// Gets the community object of the given community ID
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Community GetCommunityByID(string ID)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/communities/" + ID);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";

            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                return JsonConvert.DeserializeObject<AIOObjects.Twitch.Community>(Response.ReadToEnd());
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUserIDException(Ex);
            }
            return null;
        }
        
        /// <summary>
        /// Gets the stream of the given user ID
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Streams GetStreamFromUser(string ID)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams/" + ID);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                List<JObject> Streams = new List<JObject>();
                return JsonConvert.DeserializeObject<AIOObjects.Twitch.Streams>(Response.ReadToEnd());
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUsernameException(Ex);
                else Console.WriteLine(Ex.Message);
            }
            return null;
        }
        /// <summary>
        /// Gets the stream objects for the given usernames in a bulk request
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Streams GetStreamsFromUsernames(string[] Usernames)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/streams?channel=" + string.Join(",", Usernames.Select(x => x)).Replace(" ", ""));
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                List<JObject> Streams = new List<JObject>();
                return  JsonConvert.DeserializeObject<AIOObjects.Twitch.Streams>(Response.ReadToEnd());
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUsernameException(Ex);
                else Console.WriteLine(Ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets the user ID for the given username
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public string GetIdFromUsername(string Username)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users?login=" + Username);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                return JObject.Parse(Response.ReadToEnd())["users"][0]["_id"].ToString();
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUsernameException(Ex);
                else Console.WriteLine(Ex.Message);
            }
            return null;
        }
        /// <summary>
        /// Gets the user IDs for the given usernames in a bulk request
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetIdsFromUsernames(string[] Usernames)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users?login=" + string.Join(",", Usernames.Select(x => x)).Replace(" ", ""));
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                Dictionary<string, string> IDs = new Dictionary<string, string>();
                foreach(JObject user in JObject.Parse(Response.ReadToEnd())["users"])
                {
                    IDs.Add(user["name"].ToString(), user["_id"].ToString());
                }

                return IDs;
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUsernameException(Ex);
                else Console.WriteLine(Ex.Message);
            }
            return null;
        }
        /// <summary>
        /// Gets the user object for the given ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="OAuth"></param>
        /// <returns></returns>
        public JObject Users(string ID, string OAuth = null)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users/" + ID);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            if (OAuth != null)
            {
                Request.Headers.Add("Authorization: OAuth " + OAuth);
            }
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                return JObject.Parse(Response.ReadToEnd());
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUserIDException(Ex);
            }
            return null;
        }
        /// <summary>
        /// Gets the channels that the account with the given ID is following
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="cursor"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public AIOObjects.Twitch.Follows Following(string ID, string cursor = "", int count = 100)
        {
            HttpWebRequest Request;
            if (cursor != "")
            {
                Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users/" + ID + "/follows/channels?limit=" + count + "&cursor=" + cursor);
            }
            else
            {
                Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users/" + ID + "/follows/channels?limit=" + count);
            }
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                string ResponseString = Response.ReadToEnd();
                return JsonConvert.DeserializeObject<AIOObjects.Twitch.Follows>(ResponseString);
            }
            catch (WebException Ex)
            {
                if (Ex.HResult == 404)
                    throw new AIOExceptions.InvalidUserIDException(Ex);
            }
            return null;
        }

        /// <summary>
        /// Checks if the user is following the channel
        /// </summary>
        /// <param name="Channel">Account being followed</param>
        /// <param name="User">Account following</param>
        /// <returns></returns>
        public bool IsUserFollowing(string Channel, string User)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/users/" + User + "/follows/channels/" + Channel);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Method = "GET";
            try
            {
                StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
                string ResponseString = Response.ReadToEnd();
                return true;
            }
            catch (WebException Ex)
            {
                JObject Response = JObject.Parse(new StreamReader(Ex.Response.GetResponseStream()).ReadToEnd());
                if ((string)Response["status"] == "404")
                {
                    if (new Regex("^(\\w+?) is not following (\\w+?)$").IsMatch((string)Response["message"]))
                    {
                        return false;
                    }
                    else if (new Regex("^User (\\w+?) does not exist$").IsMatch((string)Response["message"]))
                    {
                        throw new AIOExceptions.InvalidUserIDException(Ex);
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// Checks if the user is subscribed to the channel by checking the subscribers of the channel
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="User"></param>
        /// <param name="OAuth"></param>
        /// <returns></returns>
        public bool IsUserSubscribed(string Channel, string User, string OAuth)
        {
            HttpWebRequest Request = (HttpWebRequest)WebRequest.Create("https://api.twitch.tv/kraken/channels/" + Channel + "/subscriptions/" + User);
            Request.Accept = "application/vnd.twitchtv.v5+json";
            Request.Headers.Add("Client-ID: " + CID);
            Request.Headers.Add("Authorization: OAuth  " + OAuth);
            Request.Method = "GET";
            try
            {
              StreamReader Response = new StreamReader(Request.GetResponse().GetResponseStream());
              string ResponseString = Response.ReadToEnd();
              return true;
            }
            catch (WebException Ex)
            {
                JObject Response = JObject.Parse(new StreamReader(Ex.Response.GetResponseStream()).ReadToEnd());
                if ((string)Response["status"] == "404")
                {
                    if (new Regex("^(\\w+?) has no subscriptions to (\\w+?)$").IsMatch((string)Response["message"]))
                    {
                        return false;
                    }
                    else if (new Regex("^User \'(\\w+?)\' does not exist$").IsMatch((string)Response["message"]))
                    {
                        throw new AIOExceptions.InvalidUserIDException(Ex);
                    }
                }
            }
            return false;
        }

    }

    public class IRCClient
    {
        public Dictionary<string, AIOObjects.IRC.Channel> Channels = new Dictionary<string, AIOObjects.IRC.Channel>();
        private bool connected = true;
        private static TcpClient TcpC;
        private static StreamReader Reader;
        private static StreamWriter Writer;
        public static List<string> JoinedChannels = new List<string>();


        public IRCClient(string Username, string Token)
        {
            connected = true;
            TcpC.Connect("irc.chat.twitch.tv", 6667);
            Reader = new StreamReader(TcpC.GetStream());
            Writer = new StreamWriter(TcpC.GetStream())
            {
                AutoFlush = true
            };
            Writer.WriteLine("CAP REQ :twitch.tv/membership");
            Writer.WriteLine("CAP REQ :twitch.tv/tags");
            Writer.WriteLine("CAP REQ :twitch.tv/commands");
            Writer.WriteLine("PASS " + Token);
            Writer.WriteLine("NICK " + Username);

            BufferReader();
        }
        public IRCClient(string Username, string Token, string[] Channels)
        {
            connected = true;
            TcpC = new TcpClient();
            TcpC.Connect("irc.chat.twitch.tv", 6667);
            Reader = new StreamReader(TcpC.GetStream());
            Writer = new StreamWriter(TcpC.GetStream())
            {
                AutoFlush = true
            };
            Writer.WriteLine("CAP REQ :twitch.tv/membership");
            Writer.WriteLine("CAP REQ :twitch.tv/tags");
            Writer.WriteLine("CAP REQ :twitch.tv/commands");
            Writer.WriteLine("PASS " + Token);
            Writer.WriteLine("NICK " + Username);

            foreach (string Chan in Channels)
            {
                Join(Chan);
            }

            BufferReader();
        }
        public void Reconnect(string Username, string Token, string[] Channels)
        {
            SendRaw("QUIT");
            connected = false;
            Writer.Dispose();
            Reader.Dispose();
            TcpC.Close();
            connected = true;
            TcpC = new TcpClient();
            TcpC.Connect("irc.chat.twitch.tv", 6667);
            Reader = new StreamReader(TcpC.GetStream());
            Writer = new StreamWriter(TcpC.GetStream())
            {
                AutoFlush = true
            };
            Writer.WriteLine("CAP REQ :twitch.tv/membership");
            Writer.WriteLine("CAP REQ :twitch.tv/tags");
            Writer.WriteLine("CAP REQ :twitch.tv/commands");
            Writer.WriteLine("PASS " + Token);
            Writer.WriteLine("NICK " + Username);

            foreach (string Chan in Channels)
            {
                Writer.WriteLine("JOIN #" + Chan);
            }
            BufferReader();
        }

        #region Events
        public event EventHandler<AIOEventArgs.MessageReceivedEventArgs> MessageReceived = (sender, e) => { };
        public event EventHandler<AIOEventArgs.UserReSubscriptionEventArgs> UserReSubscription = (sender, e) => { };
        public event EventHandler<AIOEventArgs.HostStartedEventArgs> HostStarted = (sender, e) => { };
        public event EventHandler<AIOEventArgs.HostEndedEventArgs> HostEnded = (sender, e) => { };
        public event EventHandler<AIOEventArgs.UserJoinedChannelEventArgs> UserJoined = (sender, e) => { };
        public event EventHandler<AIOEventArgs.UserPartedChannelEventArgs> UserParted = (sender, e) => { };
        public event EventHandler<AIOEventArgs.UserTimedOutEventArgs> UserTimedOut = (sender, e) => { };
        public event EventHandler<AIOEventArgs.ChannelChatClearedEventArgs> ChannelChatCleared = (sender, e) => { };
        public event EventHandler<AIOEventArgs.IRCConnectionClosedEventArgs> IRCConnectionClosed = (sender, e) => { };
        public event EventHandler<AIOEventArgs.ClientJoinedChannelEventArgs> ClientJoinedChannel = (sender, e) => { };
        public event EventHandler<AIOEventArgs.PingReceivedEventArgs> PingReceived = (sender, e) => { };
        public event EventHandler<AIOEventArgs.PongSentEventArgs> PongSent = (sender, e) => { };
        #endregion

        private async Task BufferReader()
        {
            for (string Buffer = await Reader.ReadLineAsync(); !string.IsNullOrWhiteSpace(Buffer) || connected; Buffer = Reader.ReadLine())
            {
                if (Buffer != null)
                {
                    if (Buffer.StartsWith("PING"))
                    {
                        {
                            AIOEventArgs.PingReceivedEventArgs Arguments = new AIOEventArgs.PingReceivedEventArgs()
                            {
                                time = DateTime.Now
                            };
                            PingReceived(this, Arguments);
                        }
                        Writer.WriteLine(Buffer.Replace("PING", "PONG"));
                        {
                            AIOEventArgs.PongSentEventArgs Arguments = new AIOEventArgs.PongSentEventArgs()
                            {
                                time = DateTime.Now
                            };
                            PongSent(this, Arguments);
                        }
                    }
                    else if (Regex.IsMatch(Buffer, "^@(([a-z-_]+=[^; ]{0,})(;|))+ :[a-zA-Z0-9_]{3,25}![a-zA-Z0-9_]{3,25}@[a-zA-Z0-9_]{3,25}.tmi.twitch.tv PRIVMSG #[a-zA-Z0-9_]{3,25} :"))
                    {
                        AIOEventArgs.MessageReceivedEventArgs Arguments = new AIOEventArgs.MessageReceivedEventArgs();
                        {
                            Arguments.Message = Regex.Replace(Buffer, "^@(.*?) :[a-zA-Z0-9_]{3,25}![a-zA-Z0-9_]{3,25}@[a-zA-Z0-9_]{3,25}.tmi.twitch.tv PRIVMSG #[a-zA-Z0-9_]{3,25} :", "");
                            Arguments.Channel = Buffer.Split(' ')[3].Remove(0, 1);
                            Arguments.Username = Regex.Replace(Buffer, "^@(.*?) :[a-zA-Z0-9_]{3,25}!", "").Split('@')[0];
                            Arguments.Tags = new Dictionary<string, string>();
                            foreach (string Tag in Regex.Match(Buffer, "^@(.*?) :").Value.Remove(0, 1).Replace(" :", "").Split(';'))
                            {
                                Arguments.Tags.Add(Tag.Split('=')[0], Tag.Split('=')[1]);
                            }
                        }
                        MessageReceived(this, Arguments);
                    }
                    else if (Regex.IsMatch(Buffer, "^@(([a-z-_]+=[^; ]{0,})(;|))+ :tmi.twitch.tv USERNOTICE #[a-zA-Z0-9_]{3,25}"))
                    {
                        AIOEventArgs.UserReSubscriptionEventArgs Arguments = new AIOEventArgs.UserReSubscriptionEventArgs();
                        {
                            Arguments.Message = Regex.Replace(Buffer, "^@(.*?) :[a-zA-Z0-9_]{3,25}![a-zA-Z0-9_]{3,25}@[a-zA-Z0-9_]{3,25}.tmi.twitch.tv PRIVMSG #[a-zA-Z0-9_]{3,25} :", "");
                            Arguments.Channel = Buffer.Split(' ')[3].Remove(0, 1);
                            Arguments.Username = Regex.Replace(Buffer, "^@(.*?) :[a-zA-Z0-9_]{3,25}!", "").Split('@')[0];
                            Arguments.Tags = new Dictionary<string, string>();
                            foreach (string Tag in Regex.Match(Buffer, "^@(.*?) :").Value.Remove(0, 1).Replace(" :", "").Split(';'))
                            {
                                Arguments.Tags.Add(Tag.Split('=')[0], Tag.Split('=')[1]);
                            }
                        }
                        UserReSubscription(this, Arguments);
                    }
                    else if (Regex.IsMatch(Buffer, "^:[a-zA-Z0-9_]{3,25}![a-zA-Z0-9_]{3,25}@[a-zA-Z0-9_]{3,25}.tmi.twitch.tv JOIN #[a-zA-Z0-9_]{3,25}$"))
                    {
                        AIOEventArgs.UserJoinedChannelEventArgs Arguments = new AIOEventArgs.UserJoinedChannelEventArgs()
                        {
                            Username = Buffer.Split('!')[0].Remove(0, 1),
                            Channel = Buffer.Split('#')[1]
                        };
                        UserJoined(this, Arguments);
                    }
                    else if (Regex.IsMatch(Buffer, "^:[a-zA-Z0-9_]{3,25}![a-zA-Z0-9_]{3,25}@[a-zA-Z0-9_]{3,25}.tmi.twitch.tv PART #[a-zA-Z0-9_]{3,25}$"))
                    {
                        AIOEventArgs.UserPartedChannelEventArgs Arguments = new AIOEventArgs.UserPartedChannelEventArgs()
                        {
                            Username = Buffer.Split('!')[0].Remove(0, 1),
                            Channel = Buffer.Split('#')[1]
                        };
                        UserParted(this, Arguments);
                    }
                    else if (Regex.IsMatch(Buffer, "^@(([a-z-_]+=[^; ]{0,})(;|))+ :tmi.twitch.tv CLEARCHAT #[a-zA-Z0-9_]{3,25} :[a-zA-Z0-9_]{3,25}$"))
                    {
                        AIOEventArgs.UserTimedOutEventArgs Arguments = new AIOEventArgs.UserTimedOutEventArgs();
                        {
                            Arguments.Username = Buffer.Split(' ')[4].Remove(0, 1);
                            Arguments.Channel = Buffer.Split(' ')[3].Remove(0, 1);
                            Arguments.Tags = new Dictionary<string, string>();
                            foreach (string Tag in Regex.Match(Buffer, "^@(.*?) :").Value.Remove(0, 1).Replace(" :", "").Split(';'))
                            {
                                Arguments.Tags.Add(Tag.Split('=')[0], Tag.Split('=')[1]);
                            }
                        }
                        UserTimedOut(this, Arguments);
                    }
                    else if (Regex.IsMatch(Buffer, "^@(([a-z-_]+=[^; ]{0,})(;|))+ :tmi.twitch.tv CLEARCHAT #[a-zA-Z0-9_]{3,25}$"))
                    {
                        AIOEventArgs.ChannelChatClearedEventArgs Arguments = new AIOEventArgs.ChannelChatClearedEventArgs();
                        {
                            Arguments.Channel = Buffer.Split(' ')[3].Remove(0, 1);
                        }
                        ChannelChatCleared(this, Arguments);
                    }
                    else if (Regex.IsMatch(Buffer, "^@(([a-z-_]+=[^; ]{0,})(;|))+ :tmi.twitch.tv ROOMSTATE #[a-zA-Z0-9_]{3,25}$"))
                    {
                        try
                        {
                            Dictionary<string, string> Tags = new Dictionary<string, string>();
                            AIOObjects.IRC.Channel Channel = new AIOObjects.IRC.Channel();
                            {
                                foreach (string Tag in Regex.Match(Buffer, "^@(.*?) :").Value.Remove(0, 1).Replace(" :", "").Split(';'))
                                {
                                    Tags.Add(Tag.Split('=')[0], Tag.Split('=')[1]);
                                }

                                if (Tags["subs-only"] == "1")
                                {
                                    Channel.subsOnly = true;
                                }
                                else
                                {
                                    Channel.subsOnly = false;
                                }

                                if (Tags["emote-only"] == "1")
                                {
                                    Channel.emoteOnly = true;
                                }
                                else
                                {
                                    Channel.emoteOnly = false;
                                }

                                if (Tags["r9k"] == "1")
                                {
                                    Channel.r9k = true;
                                }
                                else
                                {
                                    Channel.r9k = false;
                                }

                                if (!string.IsNullOrWhiteSpace(Tags["broadcaster-lang"]))
                                {
                                    Channel.lang = Tags["broadcaster-lang"];
                                }
                                else
                                {
                                    Channel.lang = "UNDEFINED";
                                }

                                Channel.followersOnly = Convert.ToInt32(Tags["followers-only"]);
                                Channel.slow = Convert.ToInt32(Tags["slow"]);
                                Channels.Add(Buffer.Split(' ')[3].Remove(0, 1), Channel);
                                AIOEventArgs.ClientJoinedChannelEventArgs Arguments = new AIOEventArgs.ClientJoinedChannelEventArgs()
                                {
                                    channel = Buffer.Split(' ')[3].Remove(0, 1),
                                    channelData = Channel,
                                    ID = Tags["room-id"]
                                };
                                ClientJoinedChannel(this, Arguments);
                                JoinedChannels.Add(Buffer.Split(' ')[3].Remove(0, 1));
                            }
                        }
                        catch (Exception Ex)
                        {
                            Console.WriteLine(Ex.Message);
                        }
                    }
                    else if (Regex.IsMatch(Buffer, "^@msg-id=host_on :tmi.twitch.tv NOTICE #[a-zA-Z0-9_]{3,25} :Now hosting [a-zA-Z0-9_]{3,25}"))
                    {
                        AIOEventArgs.HostStartedEventArgs Arguments = new AIOEventArgs.HostStartedEventArgs();
                        {
                            Arguments.Host = Buffer.Split('#')[1].Split(' ')[0];
                            Arguments.Guest = Regex.Replace(Buffer.Remove(Buffer.Length - 1, 1), "^@msg-id=host_on :tmi.twitch.tv NOTICE #[a-zA-Z0-9_]{3,25} :Now hosting ", "");
                        }
                        HostStarted(this, Arguments);
                    }
                    else if (Regex.IsMatch(Buffer, "^@msg-id=host_off :tmi.twitch.tv NOTICE #[a-zA-Z0-9_]{3,25} :Exited host mode."))
                    {
                        AIOEventArgs.HostEndedEventArgs Arguments = new AIOEventArgs.HostEndedEventArgs();
                        {
                            Arguments.Host = Buffer.Split('#')[1].Split(' ')[0];
                        }
                        HostEnded(this, Arguments);
                    }
                    else
                    {
                        Console.WriteLine(Buffer);
                    }
                }
            }
        }

        public async Task SendRawAsync(string Text)
        {
            if (connected)
            {
                await Writer.WriteLineAsync(Text);
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public async Task SendAsync(string Channel, string Message)
        {
            if (connected)
            {
                await Writer.WriteLineAsync("PRIVMSG #" + Channel + " :" + Message);
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public async Task JoinAsync(string Channel)
        {
            if (connected)
            {
                if (!JoinedChannels.Contains(Channel))
                {
                    await Writer.WriteLineAsync("JOIN #" + Channel);
                    ClientJoinedChannel(this, new AIOEventArgs.ClientJoinedChannelEventArgs()
                    {
                        channel = Channel
                    });
                }
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public async Task PartAsync(string Channel)
        {
            if (connected)
            {
                if (JoinedChannels.Contains(Channel))
                {
                    JoinedChannels.Remove(Channel);
                    await Writer.WriteLineAsync("PART #" + Channel);
                }
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public void SendRaw(string Text)
        {
            if (connected)
            {
                Writer.WriteLine(Text);
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public void Send(string Channel, string Message)
        {
            if (connected)
            {
                Writer.WriteLine("PRIVMSG #" + Channel + " :" + Message);
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public void Join(string Channel)
        {
            Channel = Channel.ToLower();
            if (connected)
            {
                if (!JoinedChannels.Contains(Channel))
                {
                    JoinedChannels.Add(Channel);
                    Writer.WriteLine("JOIN #" + Channel);
                }
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public void Part(string Channel)
        {
            if (connected)
            {
                if (JoinedChannels.Contains(Channel))
                {
                    JoinedChannels.Remove(Channel);
                    Writer.WriteLine("PART #" + Channel);
                }
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }

        public void Disconnect()
        {
            if (connected)
            {
                SendRaw("QUIT");
                connected = false;
                TcpC.Close();
                IRCConnectionClosed(this, new AIOEventArgs.IRCConnectionClosedEventArgs());
            }
            else
            {
                throw new AIOExceptions.IRCConnectionClosed();
            }
        }
    }

    #region Exceptions
    public class AIOExceptions
    {
        public class InvalidCustomerIDException : Exception
        {
            internal InvalidCustomerIDException()
            {

            }
            internal InvalidCustomerIDException(WebException Inner)
            {
                InnerException = Inner;
            }

            internal new WebException InnerException { get; set; }
        }
        public class InvalidUsernameException : Exception
        {
            internal InvalidUsernameException()
            {

            }
            internal InvalidUsernameException(WebException Inner)
            {
                InnerException = Inner;
            }

            internal new WebException InnerException { get; set; }
        }
        public class InvalidUserIDException : Exception
        {
            internal InvalidUserIDException()
            {

            }
            internal InvalidUserIDException(WebException Inner)
            {
                InnerException = Inner;
            }

            internal new WebException InnerException { get; set; }
        }
        public class IRCConnectionClosed : Exception
        {
        }
    }
    #endregion

    #region Events
    public class AIOEventArgs
    {
        public class MessageReceivedEventArgs : EventArgs
        {
            public string Channel { get; set; }
            public string Message { get; set; }
            public string Username { get; set; }
            public Dictionary<string, string> Tags { get; set; }
        }
        public class UserReSubscriptionEventArgs : EventArgs
        {
            public string Channel { get; set; }
            public string Message { get; set; }
            public string Username { get; set; }
            public Dictionary<string, string> Tags { get; set; }
        }
        public class HostStartedEventArgs : EventArgs
        {
            public string Host { get; set; }
            public string Guest { get; set; }
        }
        public class HostEndedEventArgs : EventArgs
        {
            public string Host { get; set; }
        }
        public class UserJoinedChannelEventArgs : EventArgs
        {
            public string Channel { get; set; }
            public string Username { get; set; }
        }
        public class UserPartedChannelEventArgs : EventArgs
        {
            public string Channel { get; set; }
            public string Username { get; set; }
        }
        public class ChannelChatClearedEventArgs : EventArgs
        {
            public string Channel { get; set; }
        }
        public class UserTimedOutEventArgs : EventArgs
        {
            public string Channel { get; set; }
            public string Username { get; set; }
            public Dictionary<string, string> Tags { get; set; }
        }
        public class IRCConnectionClosedEventArgs : EventArgs
        {
            public DateTime time { get; set; }
        }
        public class ClientJoinedChannelEventArgs : EventArgs
        {
            public string channel { get; set; }
            public AIOObjects.IRC.Channel channelData { get; set; }
            public string ID { get; set; }
        }
        public class PingReceivedEventArgs : EventArgs
        {
            public DateTime time { get; set; }
        }
        public class PongSentEventArgs : EventArgs
        {
            public DateTime time { get; set; }
        }
    }
    #endregion

    #region Objects
    public class AIOObjects
    {
        public class Twitch
        {
            public class Channel
            {
                [JsonProperty("_id")]
                public string id { get; set; }
                public string created_at { get; set; }
                public string display_name { get; set; }
                public int followers { get; set; }
                public string game { get; set; }
                public string name { get; set; }
                public string status { get; set; }
                public string updated_at { get; set; }
                public string url { get; set; }
                public int views { get; set; }
            }
            public class Followers
            {
                [JsonProperty("_total")]
                public int total { get; set; }
                public string cursor { get; set; }
                [JsonProperty("follows")]
                public List<Follower> followers { get; set; }
                public class Follower
                {
                    public string created_at { get; set; }
                    public User user { get; set; }
                    public class User
                    {
                        [JsonProperty("_id")]
                        public string id { get; set; }
                        public string display_name { get; set; }
                        public string name { get; set; }
                        public string type { get; set; }
                        public string created_at { get; set; }
                    }
                }
            }
            public class Follows
            {
                [JsonProperty("_total")]
                public int total { get; set; }
                public string cursor { get; set; }
                [JsonProperty("follows")]
                public List<Follower> follows { get; set; }
                public class Follower
                {
                    public string created_at { get; set; }
                    public User channel { get; set; }
                    public class User
                    {
                        [JsonProperty("_id")]
                        public string id { get; set; }
                        public string display_name { get; set; }
                        public string name { get; set; }
                        public string type { get; set; }
                        public string created_at { get; set; }
                    }
                }
            }
            public class Subscribers
            {
                [JsonProperty("_total")]
                public int total { get; set; }
                public List<Subscriber> subscriptions { get; set; }
                public class Subscriber
                {
                    public string created_at { get; set; }
                    [JsonProperty("_id")]
                    public string id { get; set; }
                    public User user { get; set; }
                    public class User
                    {
                        [JsonProperty("_id")]
                        public string id { get; set; }
                        public string display_name { get; set; }
                        public string name { get; set; }
                        public string type { get; set; }
                        public string created_at { get; set; }
                    }

                }
            }
            public class Community
            {
                [JsonProperty("_id")]
                public string id { get; set; }
                public string owner_id { get; set; }
                public string name { get; set; }
                public string summary { get; set; }
                public string description { get; set; }
                public string description_html { get; set; }
                public string rules { get; set; }
                public string rules_html { get; set; }
                public string language { get; set; }
                public string avatar_image_url { get; set; }
                public string cover_image_url { get; set; }
            }
            public class Streams
            {
                public Stream stream { get; set; }
                public class Stream
                {
                    [JsonProperty("_id")]
                    public string id { get; set; }
                    public string game { get; set; }
                    public int viewers { get; set; }
                    public double average_fps { get; set; }
                    public int delay { get; set; }
                    public string created_at { get; set; }
                    public Channel channel { get; set; }
                }
            }
        }
        public class IRC
        {
            public class Channel
            {
                public bool subsOnly { get; set; }
                public bool emoteOnly { get; set; }
                public bool r9k { get; set; }
                public string lang { get; set; }
                public int followersOnly { get; set; }
                public int slow { get; set; }
            }
        }
        public class channelData
        {
            public string token { get; set; }
            public string ID { get; set; }
            public string channelname { get; set; }
            public List<command> commands { get; set; }
            public List<quote> quotes { get; set; }
            public pointSystem points { get; set; }
            public class command
            {
                public string trigger { get; set; }
                public string reply { get; set; }
                public string level { get; set; }
                public int count { get; set; }
                public Dictionary<string, int> countPair { get; set; }
            }
            public class quote
            {
                public string person;
                public string phrase;
                public string date;
            }
            public class pointSystem
            {
                public int interval { get; set; }
                public int pointsPerTick { get; set; }
                public string pointsName { get; set; }
                public Dictionary<string, int> userPoints { get; set; }
            }
        }
        public class usernamesDatabase
        {
            public List<user> users { get; set; }
            public class user
            {
                public string username { get; set; }
                public string ID { get; set; }
            }
        }
    }
    #endregion
}
