using SoftBook.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Interfaces
{
    public interface ITaskTemplateService
    {
        Task<List<TaskTemplateModel>> GetTemplates(int companyId);
        Task<TaskTemplateModel> GetTemplateById(int companyId,long templateId);
        Task<long> AddTemplate(int companyId,TaskTemplateModel model);
        Task<bool> UpdateTemplate(int companyId,TaskTemplateModel model);
        Task<bool> DeleteTemplate(int companyId,long templateId);
    }
}
