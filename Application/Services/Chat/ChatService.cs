using Presistence.Repositories.ChatRepo;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public ChatService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task SaveMessageAsync(ChatMessage message)
        {
            var messageKey = $"chat:messages:{message.SessionId}";
            var serializedMessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);

            await _database.ListRightPushAsync(messageKey, serializedMessage);
        }

        public async Task<ChatSession> StartSessionAsync(string customerEmail, string adminEmail)
        {
            var sessionKey = $"chat:sessions:{customerEmail}:{adminEmail}";
            var serializedSession = await _database.StringGetAsync(sessionKey);

            if (serializedSession.IsNullOrEmpty)
            {
                var session = new ChatSession
                {
                    CustomerEmail = customerEmail,
                    AdminEmail = adminEmail,
                    StartedAt = DateTime.UtcNow,
                    Status = "pending"
                };

                serializedSession = Newtonsoft.Json.JsonConvert.SerializeObject(session);
                await _database.StringSetAsync(sessionKey, serializedSession);

                return session;
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<ChatSession>(serializedSession);
        }

        public async Task<bool> EndSessionAsync(int sessionId)
        {
            var sessionKey = $"chat:sessions:{sessionId}";
            var serializedSession = await _database.StringGetAsync(sessionKey);

            if (serializedSession.IsNullOrEmpty)
                return false;

            var session = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatSession>(serializedSession);
            if (session.EndedAt.HasValue)
                return false;

            session.EndedAt = DateTime.UtcNow;
            serializedSession = Newtonsoft.Json.JsonConvert.SerializeObject(session);
            await _database.StringSetAsync(sessionKey, serializedSession);

            return true;
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(int sessionId)
        {
            var messageKey = $"chat:messages:{sessionId}";
            var messages = await _database.ListRangeAsync(messageKey);

            var chatMessages = new List<ChatMessage>();
            foreach (var message in messages)
            {
                chatMessages.Add(Newtonsoft.Json.JsonConvert.DeserializeObject<ChatMessage>(message));
            }

            return chatMessages;
        }

        public async Task UpdateSessionStatusAsync(int sessionId)
        {
            var sessionKey = $"chat:sessions:{sessionId}";
            var serializedSession = await _database.StringGetAsync(sessionKey);

            if (!serializedSession.IsNullOrEmpty)
            {
                var session = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatSession>(serializedSession);
                session.Status = "processed";

                serializedSession = Newtonsoft.Json.JsonConvert.SerializeObject(session);
                await _database.StringSetAsync(sessionKey, serializedSession);
            }
        }

        public async Task<List<ChatSession>> GetPendingSessions()
        {
            var endpoints = _redis.GetEndPoints();
            var server = _redis.GetServer(endpoints.First());

            var pendingSessions = new List<ChatSession>();
            foreach (var key in server.Keys(pattern: "chat:sessions:*"))
            {
                var serializedSession = await _database.StringGetAsync(key);
                var session = Newtonsoft.Json.JsonConvert.DeserializeObject<ChatSession>(serializedSession);

                if (session.Status == "pending")
                {
                    pendingSessions.Add(session);
                }
            }

            return pendingSessions;
        }
    }
}
