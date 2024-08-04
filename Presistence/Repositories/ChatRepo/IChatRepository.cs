using Domain.Entities;

namespace Presistence.Repositories.ChatRepo
{
    public interface IChatRepository
    {
        Task AddMessageAsync(ChatMessage message);
        Task AddSessionAsync(ChatSession session);
        Task<ChatSession> GetSessionByIdAsync(int sessionId);
        Task<List<ChatMessage>> GetMessagesBySessionIdAsync(int sessionId);

        Task UpdateSessionAsync(ChatSession session);

        Task<List<ChatSession>> GetSessionsByStatusAsync();

        Task<ChatSession> GetSession(string User1, string User2);


    }
}
