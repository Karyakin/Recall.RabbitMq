using MicroRabbit.Banking.Data.Context;
using MicroRabbit.Banking.Domain.Interfaces;
using MicroRabbit.Banking.Domain.Models;

namespace MicroRabbit.Banking.Data.Repository;

public class AccountRepository : IAccountRepository
{
    private BankingDbContext _bankingDbContext;

    public AccountRepository(BankingDbContext bankingDbContext)
    {
        _bankingDbContext = bankingDbContext;
    }

    public IEnumerable<Account> GetAccount()
    {
        return _bankingDbContext.Accounts;
    }
}