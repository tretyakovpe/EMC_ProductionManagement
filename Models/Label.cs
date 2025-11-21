namespace ProductionManagement.Models
{
    internal class Label
    {
        public static double MM { get; } = 2.85;

        public static List<int[]> Borders { get; } =
        [
            [5, 5, 205, 5], // Горизонтальные
            [5, 15, 205, 15],
            [5, 40, 205, 40],
            [5, 65, 205, 65],
            [5, 90, 205, 90],
            [5, 115, 205, 115],
            [5, 142, 205, 142],
            [105, 103, 205, 103],
            [5, 5, 5, 142], // Вертикальные
            [105, 5, 105, 65],
            [138, 90, 138, 103],
            [171, 90, 171, 103],
            [105, 90, 105, 142],
            [205, 5, 205, 142],
        ];

        private readonly Dictionary<string, Field> _fieldsByName = new();

        public List<Field> LabelFields { get; }

        public Label()
        {
            LabelFields = new List<Field>()
            {
                new Field(6,139,"Destination","АвтоВАЗ", false),                // 0
                new Field(106,139,"Delivery place", "TEST", false),             // 1
                new Field(6,114,"Document #", "TEST", false),                   // 2
                new Field(106,114,"Supplier address","Северная, 6а", false),    // 3
                new Field(106,102,"Netto","0", false),                          // 4
                new Field(141,102,"Brutto","0", false),                         // 5
                new Field(172,102,"Boxes","0", false),                          // 6
                new Field(6,89,"Product", "TEST", false),                       // 7
                new Field(6,64,"Quantity","TEST", false),                       // 8
                new Field(106,64,"Part name","TEST", false, 106,39, 36, "3OS"),  // 9
                new Field(6,39,"Label number","TEST", false),                   // 10
                new Field(106,39,"Supplier","", true, 106,12,36),               // 11
                new Field(6,14,"Date","01.01.2022", false),                     // 12
                new Field(106,14,"Packing type","TEST", false),                 // 13
                new Field(70,89,"Description", "Стеклоподъемник электрический", false),   // 14
            };

            foreach (var field in LabelFields)
            {
                _fieldsByName[field.Name] = field;
            }
        }

        // Метод установки нового значения поля по его имени
        public void SetField(string name, string value)
        {
            if (_fieldsByName.TryGetValue(name, out var field))
            {
                field.Text = value;
            }
        }
    }

    class Field(int X, int Y, string Name, string Text, bool Barcode, int bcX = 0, int bcY = 0, int bcSize = 0, string Code = "")
    {
        public int X { get; set; } = X;
        public int Y { get; set; } = Y;
        public string Name { get; set; } = Name;
        public string Text { get; set; } = Text;
        public int bcX { get; set; } = bcX;
        public int bcY { get; set; } = bcY;
        public int bcSize { get; set; } = bcSize;
        public bool Barcode { get; set; } = Barcode;
        public string Code { get; set; } = Code;
    }
}