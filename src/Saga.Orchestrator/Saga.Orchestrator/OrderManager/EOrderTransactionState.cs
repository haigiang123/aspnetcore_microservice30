namespace Saga.Orchestrator.OrderManager
{
    public enum EOrderTransactionState
    {
        NotStarted,
        BasketGot,
        BasketGetFailed,
        OrderCreated,
        OrderGot,
        OrderGetFailed,
        InventoryUpdated,
        InventoryUpdateFailed,
        InventoryRollback,
    }
}
