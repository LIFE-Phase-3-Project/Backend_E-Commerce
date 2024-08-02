using Microsoft.AspNetCore.SignalR;
using Nest;
using System.Collections.Concurrent;
using Domain.DTOs.Chat;
using Domain.Entities;
using Application.Services.Chat;

namespace Life_Ecommerce.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> Users = new ConcurrentDictionary<string, string>();
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            var email = Context.GetHttpContext().Request.Query["email"];

            if (!string.IsNullOrEmpty(email))
            {
                Users[email] = Context.ConnectionId;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var email = Context.GetHttpContext().Request.Query["email"];

            if (!string.IsNullOrEmpty(email))
            {
                Users.TryRemove(email, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendToSpecific(string senderEmail, string messageText, string recipientEmail, int sessionId)
        {
            if (Users.TryGetValue(recipientEmail, out string connectionId))
            {
                // Create a new chat message
                var chatMessage = new ChatMessage
                {
                    Sender = senderEmail,
                    Recipient = recipientEmail,
                    Message = messageText,
                    Timestamp = DateTime.UtcNow,
                    SessionId = sessionId // Use the provided sessionId
                };

                // Save the message to the database
                await _chatService.SaveMessageAsync(chatMessage);

                // Send the message to the recipient
                await Clients.Client(connectionId).SendAsync("broadcastMessage", senderEmail, messageText);
                await Clients.Client(connectionId).SendAsync("newMessageNotification", senderEmail);
            }
            else
            {
                Console.WriteLine($"Recipient {recipientEmail} is not connected. Message cannot be delivered.");
            }
        }

    }
}
