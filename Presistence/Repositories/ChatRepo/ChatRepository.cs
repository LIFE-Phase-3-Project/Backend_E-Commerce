using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Presistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presistence.Repositories.ChatRepo
{
    public class ChatRepository : IChatRepository
    {
        private readonly APIDbContext _context;

        public ChatRepository(APIDbContext context)
        {
            _context = context;
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            await _context.ChatMessages.AddAsync(message);
        }

        public async Task AddSessionAsync(ChatSession session)
        {
            await _context.ChatSessions.AddAsync(session);
        }

       

        public async Task<ChatSession> GetSessionByIdAsync(int sessionId)
        {
            return await _context.ChatSessions.FindAsync(sessionId);
        }

        public async Task<List<ChatMessage>> GetMessagesBySessionIdAsync(int sessionId)
        {
            return await _context.ChatMessages.Where(m => m.SessionId == sessionId).ToListAsync();
        }

        public async Task UpdateSessionAsync(ChatSession session)
        {
            _context.ChatSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatSession>> GetSessionsByStatusAsync()
        {
            return await _context.ChatSessions
                .Where(cs => cs.Status == "pending")
                .ToListAsync();
        }
    }
}
