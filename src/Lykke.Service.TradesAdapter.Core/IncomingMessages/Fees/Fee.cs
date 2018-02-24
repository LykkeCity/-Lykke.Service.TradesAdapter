namespace Lykke.Service.TradesAdapter.Core.IncomingMessages.Fees
{
    public class Fee
    {
        public FeeInstruction Instruction { get; set; }

        public FeeTransfer Transfer { get; set; }
    }
}
