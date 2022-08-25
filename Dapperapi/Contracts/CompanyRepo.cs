using Dapper;
using Dapperapi.Context;
using Dapperapi.Dto;
using Dapperapi.Entities;
using System.Data;

namespace Dapperapi.Contracts
{
    public class CompanyRepo :ICompanyRepo
    {
        private readonly DapperContext context;

        public CompanyRepo(DapperContext context) => this.context = context;

        public async Task<Company> CreateCompany(CompanyCreationDto company)
        {
            var query = "Insert into companies (Name ,Address,Country) Values (@Name,@Address,@Country)"+"Select Cast (Scope_Identity() as int)";


            var parms = new DynamicParameters();
            parms.Add("Name",company.Name,DbType.String);
            parms.Add("Address", company.Address, DbType.String);

            parms.Add("Country", company.Country, DbType.String);

            using(var connection = context.CreateConnection())
            {
                var id = await connection.QuerySingleAsync<int>(query, parms);
                var createdCompany = new Company
                {
                    Id = id,
                    Name = company.Name,
                    Address = company.Address,
                    Country = company.Country,
                };
                return createdCompany;
         
            }







        }

        public async Task DeleteCompany(int id)
        {

            var q = "delete from Companies where Id =@id";
            using (var connection = context.CreateConnection())
            {
                await connection.ExecuteAsync(q, new { id });

            }
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            var query = "Select * from Companies";
            using(var connection = context.CreateConnection())
            {
                var companies = await connection.QueryAsync<Company>(query);
                return companies.ToList();
            }
        }

        public async Task<Company> GetCompanyByEmployeeId(int id)
        {
            var procedureName = "ShowCompanyByEmployeeId";
            var parms = new DynamicParameters();
            parms.Add("Id",id,DbType.Int32,ParameterDirection.Input);

            using(var conn =context.CreateConnection())
            {
                var company = await conn.QueryFirstOrDefaultAsync<Company>
                    (procedureName, parms, commandType: CommandType.StoredProcedure);
                return company;
            }
        }

        public async Task<Company> GetCompanyById(int Id)
        {
            var query = "select * from companies where Id = @Id";
            using (var connection = context.CreateConnection())
            {
                var company = await connection.QuerySingleOrDefaultAsync<Company>(query ,new {Id});
                return company;
            }
           
       }

        public async Task<Company> GetMultipleResults(int id)
        {
            var query = "SELECT * from companies where Id = @ID;" + 
                "SELECT * FROM Employees where CompanyId =@Id";
            using (var connection = context.CreateConnection())
            using (var multi = await connection.QueryMultipleAsync(query, new {id}))

            {
                var company = await multi.ReadSingleOrDefaultAsync<Company>();
                if (company is not null)
                    company.Employees = (await multi.ReadAsync<Employee>()).ToList();
                return company;
            }

        }

        public async Task<List<Company>> MultipleMapping()
        {
            var query = "select * from Companies c JOIN Employees e ON c.Id = e.CompanyId";
            
            using(var connection = context.CreateConnection())
                    {
                var companyDict = new Dictionary<int, Company>();
                var companies = await connection.QueryAsync<Company, Employee, Company>(
                    query, (company, employee) =>
                    {
                        if (!companyDict.TryGetValue(company.Id, out var currentCompany))
                        {
                            currentCompany = company;
                            companyDict.Add(currentCompany.Id, currentCompany);

                        }
                        currentCompany.Employees.Add(employee);
                        return currentCompany;
                    }
                );
                return companies.Distinct().ToList();
            }
            
        }

        public async Task UpdateCompany(int id, CompanyUpdateDto company)
        {

            var query = "update Companies set Name = @Name ,Address = @Address, Country =@Country WHERE Id = @Id";

            var parms = new DynamicParameters();

            parms.Add("ID", id, DbType.Int32);
            parms.Add("Name", company.Name, DbType.String);
            parms.Add("Address", company.Address, DbType.String);
            parms.Add("Country", company.Country, DbType.String);
            
        }

        public async Task CreateMultipleCompanies(List<CompanyCreationDto> companies)
        {
            var q = "Insert into Companies (Name , Address,Country) values (@Name, @Address, @Country)";

            using (var connection = context.CreateConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    foreach (var company in companies)
                    {
                        var parameters = new DynamicParameters();
                        parameters.Add("Name", company.Name, DbType.String);
                        parameters.Add("Address", company.Address, DbType.String);
                        parameters.Add("Country", company.Country, DbType.String);

                        await connection.ExecuteAsync(q, parameters, transaction: transaction);
                        //throw new Exception();
                    }

                    transaction.Commit();
                }
            }
        }
    }
}
