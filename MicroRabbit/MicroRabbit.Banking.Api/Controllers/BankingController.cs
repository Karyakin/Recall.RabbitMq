using MicroRabbit.Banking.Application.Dto;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MicroRabbit.Banking.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BankingController : ControllerBase
{
    private readonly IAccountService _accountService;

    public BankingController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public ActionResult<List<Account>> Get()
    {
        var res = _accountService.GetAccounts().ToList();
        return Ok(res);
    }

    [HttpPost]
    public ActionResult<List<Account>> Post([FromBody] AccountTransfer accountTransfer)
    {
        _accountService.Transfer(accountTransfer);
        return Ok();
    }
}