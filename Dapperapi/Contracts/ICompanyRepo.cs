using Dapperapi.Context;
using Dapperapi.Dto;
using Dapperapi.Entities;

namespace Dapperapi.Contracts
{
    public interface ICompanyRepo
    {
        public Task<IEnumerable<Company>> GetCompanies();
        public Task<Company> GetCompanyById(int companyId);
        public Task<Company> CreateCompany(CompanyCreationDto company);
        public Task UpdateCompany(int id,CompanyUpdateDto company);
        public Task DeleteCompany (int id);
        public Task<Company> GetCompanyByEmployeeId(int id);

        public Task<Company> GetMultipleResults(int id);

        public Task<List<Company>> MultipleMapping();
        public Task CreateMultipleCompanies(List<CompanyCreationDto> companies);
    }
}
