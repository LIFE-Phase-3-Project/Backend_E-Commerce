using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Chat
{


    public interface IChatService
    {
        Task SaveMessageAsync(ChatMessage message);
        Task<ChatSession> StartSessionAsync(string customerEmail, string adminEmail);
        Task<bool> EndSessionAsync(int sessionId);
        Task<List<ChatMessage>> GetMessagesAsync(int sessionId);

        Task UpdateSessionStatusAsync(int sessionId);

        Task<List<ChatSession>> GetPendingSessions();
    }

}
