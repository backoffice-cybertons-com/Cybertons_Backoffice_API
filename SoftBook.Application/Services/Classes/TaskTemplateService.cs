using SoftBook.Application.Services.Interfaces;
using SoftBook.Domain.DTOs;
using SoftBook.Domain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftBook.Application.Services.Classes
{
    public class TaskTemplateService : ITaskTemplateService
    {
        private readonly ITaskTemplateRepository _repository;

        public TaskTemplateService(ITaskTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<TaskTemplateModel>> GetTemplates(int companyId)
        {
            return await _repository.GetTemplates(companyId);
        }

        public async Task<TaskTemplateModel> GetTemplateById(int companyId,long templateId)
        {
            return await _repository.GetTemplateById(companyId,templateId);
        }

        public async Task<long> AddTemplate(int companyId,TaskTemplateModel model)
        {
            return await _repository.AddTemplate(companyId,model);
        }

        public async Task<bool> UpdateTemplate(int companyId,TaskTemplateModel model)
        {
            return await _repository.UpdateTemplate(companyId,model);
        }

        public async Task<bool> DeleteTemplate(int companyId,long templateId)
        {
            return await _repository.DeleteTemplate(companyId, templateId);
        }
    }
}
