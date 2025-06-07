using Practica9.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Practica9.PdfReports
{
    public class ProductPdfDocument : IDocument
    {
        private readonly List<Product> _products;

        public ProductPdfDocument(List<Product> products)
        {
            _products = products;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(40);

                page.Header()
                    .Text("Reporte de Productos")
                    .FontSize(20)
                    .Bold()
                    .FontColor(Colors.Blue.Darken2)
                    .AlignCenter();

                page.Content().PaddingVertical(10).Element(BuildTable);

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generado el ").FontSize(10);
                        text.Span(DateTime.Now.ToString("dd/MM/yyyy")).FontSize(10).SemiBold();
                    });
            });
        }

        private void BuildTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Producto
                    columns.RelativeColumn(3); // Categoría
                    columns.RelativeColumn(2); // Precio
                });

                // Encabezado
                table.Header(header =>
                {
                    header.Cell().Element(CellStyleHeader).Text("Producto").FontSize(12).FontColor(Colors.White);
                    header.Cell().Element(CellStyleHeader).Text("Categoría").FontSize(12).FontColor(Colors.White);
                    header.Cell().Element(CellStyleHeader).Text("Precio").FontSize(12).FontColor(Colors.White);
                });

                // Filas
                foreach (var p in _products)
                {
                    table.Cell().Element(CellStyle).Text(p.ProductName);
                    table.Cell().Element(CellStyle).Text(p.Category?.CategoryName ?? "N/A");
                    table.Cell().Element(CellStyle).AlignRight().Text(p.UnitPrice?.ToString("C") ?? "$0.00");
                }

                // Estilos de celda
                static IContainer CellStyle(IContainer container) =>
                    container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);

                static IContainer CellStyleHeader(IContainer container) =>
                    container.Background(Colors.Blue.Darken2).Padding(5);
            });
        }
    }
}
