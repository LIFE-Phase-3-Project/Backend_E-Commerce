using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Presistence.Repositories.ChatRepo;
using Microsoft.Extensions.Caching.Distributed;

namespace Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;
        private readonly IDistributedCache _cache; // Inject the cache

        public ChatService(IUnitOfWork unitOfWork, IChatRepository chatRepository, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = chatRepository;
            _cache = cache;
        }

        public async Task SaveMessageAsync(ChatMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            await _chatRepository.AddMessageAsync(message);
            await _unitOfWork.CompleteAsync();

            // Optional: Cache the message if needed
            var cacheKey = $"chat:{message.SessionId}:messages";
            await _cache.RemoveAsync(cacheKey);
        }

        public async Task<ChatSession> StartSessionAsync(string customerEmail, string adminEmail)
        {
            if (string.IsNullOrEmpty(customerEmail))
                throw new ArgumentException("Customer email cannot be null or empty", nameof(customerEmail));

            if (string.IsNullOrEmpty(adminEmail))
                throw new ArgumentException("Admin email cannot be null or empty", nameof(adminEmail));

            var cacheKey = $"chat:session:{customerEmail}:{adminEmail}";
            var cachedSession = await _cache.GetStringAsync(cacheKey);

            if (cachedSession != null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ChatSession>(cachedSession);
            }

            var session = await _chatRepository.GetSession(customerEmail, adminEmail);

            if (session == null)
            {
                session = new ChatSession
                {
                    CustomerEmail = customerEmail,
                    AdminEmail = adminEmail,
                    StartedAt = DateTime.UtcNow,
                    //Messages = new List<ChatMessage>()
                };
                await _chatRepository.AddSessionAsync(session);
                await _unitOfWork.CompleteAsync();
            }

            // Cache the session
            await _cache.SetStringAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(session));

            return session;
        }

        public async Task<bool> EndSessionAsync(int sessionId)
        {
            var session = await _chatRepository.GetSessionByIdAsync(sessionId);

            if (session == null || session.EndedAt.HasValue)
                return false;

            session.EndedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            // Invalidate the cache
            var cacheKey = $"chat:session:{session.CustomerEmail}:{session.AdminEmail}";
            await _cache.RemoveAsync(cacheKey);

            return true;
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(int sessionId)
        {
            var cacheKey = $"chat:{sessionId}:messages";
            var cachedMessages = await _cache.GetStringAsync(cacheKey);

            if (cachedMessages != null)
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ChatMessage>>(cachedMessages);
            }

            var messages = await _chatRepository.GetMessagesBySessionIdAsync(sessionId);

            // Cache the messages
            await _cache.SetStringAsync(cacheKey, Newtonsoft.Json.JsonConvert.SerializeObject(messages));

            return messages;
        }

        public async Task UpdateSessionStatusAsync(int sessionId)
        {
            var session = await _chatRepository.GetSessionByIdAsync(sessionId);

            if (session != null)
            {
                session.Status = "processed";
                await _chatRepository.UpdateSessionAsync(session);
                await _unitOfWork.CompleteAsync();

                // Invalidate the cache
                var cacheKey = $"chat:session:{session.CustomerEmail}:{session.AdminEmail}";
                await _cache.RemoveAsync(cacheKey);
            }
        }

        public async Task<List<ChatSession>> GetPendingSessions()
        {
            return await _chatRepository.GetSessionsByStatusAsync();
        }
    }
}
