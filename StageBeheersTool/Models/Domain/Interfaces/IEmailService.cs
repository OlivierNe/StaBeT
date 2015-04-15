using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace StageBeheersTool.Models.Domain
{
    public interface IEmailService
    {
        Task SendAsync(IdentityMessage message);
    }
}
