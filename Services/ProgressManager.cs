using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProductionManagement.Data;
using ProductionManagement.Models;
using System.Numerics;

namespace ProductionManagement.Services
{
    public class ProgressManager
    {
        private readonly ApplicationDbContext _context;

        public ProgressManager(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Добавляет либо обновляет существующий тип продукции для текущей смены
        /// </summary>
        /// <param name="material">Тип продукции</param>
        /// <param name="description">Человечье название</param>
        /// <param name="result">Количество в штуках</param>
        /// <returns></returns>
        public async Task AddOrUpdateProgressAsync(string material, string description, int result)
        {
            var volumes = GetPlannedAndBacklog(material);
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.AddOrUpdateProgress @Material, @Description, @Backlog, @Planned, @Result",
                new SqlParameter("@Material", material),
                new SqlParameter("@Description", description),
                new SqlParameter("@Backlog", volumes.Backlog),
                new SqlParameter("@Planned", volumes.Planned),
                new SqlParameter("@Result", result)
            );
        }

        //#TO DO
        //Ищем в адовом экселе ячейку с текущей датой, она отформатирована как dd.MM, но внутри вроде как полная.
        //Ищем строку с нужным типом начиная с 10 строки
        //Ищем значение плана для текущей смены, от ячейки с датой - для смены А= -12 стобцов, B= -8, C= -4 столбца.

        //Получение плана и бэклога из экселя
        public (int? Planned, int Backlog) GetPlannedAndBacklog(string material)
        {
            // Определяем путь к файлу
            string filePath = @"\\NAS\Office\050_MF\010_Production_Steering\120_Файл планирования и нормы\Production plan and OEE_202510.xlsm";

            // Сегодняшняя дата
            DateTime today = DateTime.Today;

            // Открываем файл Excel
            IWorkbook workbook;
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // Определяем тип файла (XLS или XLSX)
                if (filePath.EndsWith(".xls"))
                    workbook = new HSSFWorkbook(fileStream);
                else
                    workbook = new XSSFWorkbook(fileStream);
            }

            // Лист "Сборка"
            ISheet worksheet = workbook.GetSheet("Сборка");
            if (worksheet == null)
                throw new InvalidOperationException("Лист 'Сборка' не найден.");

            // Получаем строку с датами (строка №7)
            IRow datesRow = worksheet.GetRow(6); // Индексация с 0, поэтому 6-я строка — это фактически 7-я

            // Проходим по столбцам строки с датами
            for (int colIndex = 0; colIndex < datesRow.Cells.Count; colIndex++)
            {
                var cell = datesRow.GetCell(colIndex);
                if (cell != null && cell.CellType == CellType.Numeric && DateUtil.IsCellDateFormatted(cell))
                {
                    var cellDate = cell.DateCellValue;
                    if (cellDate == today.Date)
                    {
                        // Ячейка с сегодняшней датой найдена, определяем сдвиг в зависимости от смены
                        string currentShift = GetCurrentShift();
                        int offsetColumns;
                        switch (currentShift)
                        {
                            case "A":
                                offsetColumns = -12;
                                break;
                            case "B":
                                offsetColumns = -8;
                                break;
                            default: // C
                                offsetColumns = -4;
                                break;
                        }

                        // Координаты ячейки с планом
                        int plannedColIndex = colIndex + offsetColumns;

                        // Теперь проходим по материалам и ищем нужный материал
                        for (int rowIndex = 9; rowIndex <= worksheet.LastRowNum; rowIndex++)
                        {
                            IRow currentRow = worksheet.GetRow(rowIndex);
                            if (currentRow == null || currentRow.Cells.Count == 0) continue;

                            // Колонка с материалом (предположим, первая колонка)
                            var materialCell = currentRow.GetCell(0);
                            if (materialCell != null && materialCell.CellType == CellType.String && materialCell.StringCellValue.Equals(material))
                            {
                                // Нашли нужный материал, смотрим значение плана
                                var plannedCell = currentRow.GetCell(plannedColIndex);
                                int? planned = plannedCell != null && plannedCell.CellType == CellType.Numeric ?
                                                (int?)Convert.ToInt32(plannedCell.NumericCellValue) :
                                                null;

                                // Бэклог возьмем как значение соседнего столбца (пример)
                                //int backlog = currentRow.GetCell(colIndex - 1)?.NumericCellValue.ConvertToInt32() ?? 0;

                                return (Planned: planned, Backlog: 0);
                            }
                        }
                    }
                }
            }

            // Если материал не найден
            return (Planned: -1, Backlog: -1);
        }
        // Метод для определения текущей смены
        private string GetCurrentShift()
        {
            var now = DateTime.Now;
            if (now.TimeOfDay >= new TimeSpan(23, 30, 0) || now.TimeOfDay < new TimeSpan(6, 30, 0))
                return "A";
            else if (now.TimeOfDay >= new TimeSpan(6, 30, 0) && now.TimeOfDay < new TimeSpan(15, 0, 0))
                return "B";
            else
                return "C";
        }

        // Получение списка всех записей Progress
        public async Task<List<Progress>> GetAllProgressAsync()
        {
            return await _context.Progress.ToListAsync();
        }

        // Получение записей Progress за текущую смену через хранимую процедуру
        //Смены определяются в хранимой процедуре!!!
        public async Task<List<Progress>> GetCurrentShiftProgressAsync()
        {
            return await _context.Progress
                .FromSqlRaw("EXEC GetCurrentShiftProgress")
                .ToListAsync();
        }
    }
}