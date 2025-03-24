namespace ProductionManagement.Models
{
    /// <summary>
    /// Описание тарного места готовой продукции (ящика)
    /// </summary>
    /// <param name="Number">Серийный номер ящика</param>
    /// <param name="Type">Код продукции</param>
    /// <param name="Amount">Количество продукции</param>
    public class HandlingUnit(long Number, string Type, int Amount, string Description = "Стеклоподъемник")
    {
        public long Number { get; set; } = Number;
        public string Type { get; set; } = Type;
        public string Description { get; set; } = Description;
        public int Amount { get; set; } = Amount;
    }
}
