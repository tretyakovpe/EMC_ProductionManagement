namespace ProductionManagement.Models;

/// <summary>
/// Описание формата бирки с полями значений и линиями.
/// </summary>
internal class Label
{
    public static double MM { get; } = 2.85;

    public static List<int[]> borders { get; } =
    [
        [5, 5, 205, 5],//Горизонтальные
        [5, 15, 205, 15],
        [5, 40, 205, 40],
        [5, 65, 205, 65],
        [5, 90, 205, 90],
        [5, 115, 205, 115],
        [5, 142, 205, 142],
        [105, 103, 205, 103],
        [5, 5, 5, 142],//Вертикальные
        [105, 5, 105, 65],
        [138, 90, 138, 103],
        [171, 90, 171, 103],
        [105, 90, 105, 142],
        [205, 5, 205, 142],
    ];
    /// <summary>
    /// Список полей бирки с координатами размещения и дефолтными значениями
    /// </summary>
    public List<Field> LabelFields { get; set; } =
    [
        new Field(6,139,"Destination","AvtoVAZ", false),               //      0
        new Field(106,139,"Delivery place", "TEST", false),            //      1
        new Field(6,114,"Document #", "TEST", true, "N"),              //      2
        new Field(106,114,"Supplier address","Severnaya, 6a", false),  //      3
        new Field(106,102,"Netto","0", false),                         //      4
        new Field(141,102,"Brutto","0", false),                        //      5
        new Field(172,102,"Boxes","0", false),                         //      6
        new Field(6,89,"Product", "TEST", true,"P"),                   //      7
        new Field(6,64,"Quantity","TEST", true,"Q"),                   //      8
        new Field(106,64,"Part name","TEST", true,"3OS"),              //      9
        new Field(6,39,"Label number","TEST", true,"V"),               //      10
        new Field(106,39,"Supplier","", true,"S"),                     //      11
        new Field(6,14,"Date","01.01.2022", false),                    //      12
        new Field(106,14,"Packing type","TEST", false),                //      13
        new Field(70,89," ","Стеклоподъемник электрический", false),   //      14
    ];
}

/// <summary>
/// Описание формата поля бирки
/// </summary>
/// <param name="X">Положение по горизонтали</param>
/// <param name="Y">Положение по вертикали</param>
/// <param name="Name">Название поля</param>
/// <param name="Value">Значение поля</param>
/// <param name="Barcode">Нужен ли баркод</param>
/// <param name="Code">Префикс для баркода, добавляется к началу поля Значение</param>
class Field(int X, int Y, string Name, string Value, bool Barcode, string Code = "")
{
    public int X { get; set; } = X;
    public int Y { get; set; } = Y;
    public string Name { get; set; } = Name;
    public string Value { get; set; } = Value;
    public bool Barcode { get; set; } = Barcode;
    public string Code { get; set; } = Code;
}
