namespace Lykke.Service.TradesAdapter.Core.IncomingMessages.Fees
{
    public class FeeInstruction
    {
        public string Type { get; set; }

        public string SourceClientId { get; set; }

        public string TargetClientId { get; set; }

        public string SizeType { get; set; }

        public double? Size { get; set; }
    }
}
