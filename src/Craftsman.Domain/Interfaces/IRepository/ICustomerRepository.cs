using System.Threading.Tasks;
using Craftsman.Domain.Entities;
using Craftsman.Domain.ValueObjects;

namespace Craftsman.Domain.Interfaces.Repository
{
    public interface ICustomerRepository : IRepositoryBase<Customer>, IUnitOfWork
    {
        Task<bool> CheckIfCustomerAlreadyExistsByCpf(Cpf cpf);
    }
}