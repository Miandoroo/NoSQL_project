using NoSQL_project.Models;

namespace NoSQL_project.Services.Interfaces
{
    public interface ITicketActionService
    {
        void Escalate(string id);
        void Close(string id);
        void Resolve(string id);
        Ticket GetTicketOrThrow(string id);
    }
}
