namespace MicroRabbit.Banking.Application.Dto;

public class AccountTransfer
{
    public int FromAccount { get; set; }
    public int ToAccount { get; set; }
    public decimal TransferAmount { get; set; }
}