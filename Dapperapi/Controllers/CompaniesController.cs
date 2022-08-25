using Dapperapi.Contracts;
using Dapperapi.Dto;
using Dapperapi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dapperapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyRepo companyRepo;

        public CompaniesController(ICompanyRepo companyRepo) => this.companyRepo = companyRepo;

        [HttpGet]
        public async Task<IActionResult> GetCompaines()
        {
            var companies = await companyRepo.GetCompanies();
            return Ok(companies);
        }
        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            var company = await companyRepo.GetCompanyById(id);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyCreationDto company)
        {
            var createdcompany = await companyRepo.CreateCompany(company);
            return CreatedAtRoute("CompanyById", new { id = createdcompany.Id }, createdcompany);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> updateCompany(int id, [FromBody] CompanyUpdateDto company)
        {
            var dbCompany = await companyRepo.GetCompanyById(id);
            if (dbCompany == null)
                return NotFound();

            await companyRepo.UpdateCompany(id, company);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var dbCompany = await companyRepo.GetCompanyById(id);
            if (dbCompany == null)
                return NotFound();

            await companyRepo.DeleteCompany(id);
            return NoContent();
        }
        [HttpGet("ByEmployeeId/{id}")]
        public async Task<IActionResult> GetCompanyForEmployee(int id)
        {
            var company = await companyRepo.GetCompanyByEmployeeId(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }
        [HttpGet("{id}/MultipleResult")]
        public async Task<IActionResult> GetMultipleResults(int id)
        {
            var company = await companyRepo.GetMultipleResults(id);
            if (company is null)
                return NotFound();

            return Ok(company);
        }
        [HttpGet("MultipleMapping")]
        public async Task<IActionResult> GetMultipleMapping()
        {
            var company = await companyRepo.MultipleMapping();

       

            return Ok(company);
        }
        [HttpPost("multiple")]
        public async Task<IActionResult> CreateCompany(List<CompanyForCreationDto> companies)
        {
            try
            {
                await _companyRepo.CreateMultipleCompanies(companies);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                //log error
                return StatusCode(500, ex.Message);
            }
        }
}
