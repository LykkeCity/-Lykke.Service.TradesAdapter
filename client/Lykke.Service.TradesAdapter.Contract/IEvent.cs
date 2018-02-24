using System;

namespace Lykke.Service.TradesAdapter.Contract
{
    public interface IEvent
    {
        string Id { set; get; }
        DateTime DateTime { set; get; }
    }
}
