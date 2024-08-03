using Presistence.Repositories.ChatRepo;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<bool> EndSessionAsync(int sessionId)
        {
            var session = await _chatRepository.GetSessionByIdAsync(sessionId);

            if (session == null)
            {
                return false;
            }

            if (session.EndedAt.HasValue)
            {
                return false;
            }

            session.EndedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();

            return true;
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

        

    }
}
