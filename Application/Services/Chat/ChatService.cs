using Application.Repositories.ChatRepo;
using Domain.Entities;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;

        public ChatService(IUnitOfWork unitOfWork, IChatRepository chatRepository)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = chatRepository;
        }

        public async Task SaveMessageAsync(ChatMessage message)
        {
            await _chatRepository.AddMessageAsync(message);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<ChatSession> StartSessionAsync(string customerEmail, string adminEmail)
        {
            var session = new ChatSession
            {
                CustomerEmail = customerEmail,
                AdminEmail = adminEmail,
                StartedAt = DateTime.UtcNow,
                //Messages = new List<ChatMessage>()
            };
            await _chatRepository.AddSessionAsync(session);
            await _unitOfWork.CompleteAsync();
            return session;
        }

        public async Task EndSessionAsync(int sessionId)
        {
            var session = await _chatRepository.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.EndedAt = DateTime.UtcNow;
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<List<ChatMessage>> GetMessagesAsync(int sessionId)
        {
            return await _chatRepository.GetMessagesBySessionIdAsync(sessionId);
        }

        public async Task UpdateSessionStatusAsync(int sessionId)
        {
            var session = await _chatRepository.GetSessionByIdAsync(sessionId);
            if (session != null)
            {
                session.Status = "processed";
                await _chatRepository.UpdateSessionAsync(session);
            }
        }

        public async Task<List<ChatSession>> GetPendingSessions()
        {
            return await _chatRepository.GetSessionsByStatusAsync();
        }

        //MessagesBySessionId

    }
}
