using CMS.Domain.Entities.Orders;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CMS.Server.Documents;

public sealed class OrderInvoiceDocument : IDocument
{
    private readonly Order _order;

    public OrderInvoiceDocument(Order order)
    {
        _order = order;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(24);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(11));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Pesyong Invoice");
                text.Span(" • ");
                text.CurrentPageNumber();
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("Pesyong").FontSize(24).Bold().FontColor("#E8661F");
                column.Item().Text("Order Invoice").FontSize(13).SemiBold();
            });

            row.ConstantItem(220).Column(column =>
            {
                column.Item().AlignRight().Text($"Order #: {_order.OrderNumber}");
                column.Item().AlignRight().Text($"Ordered At: {_order.OrderedAtUtc.ToLocalTime():MMM dd, yyyy hh:mm tt}");
                column.Item().AlignRight().Text($"Status: {_order.Status}");
                column.Item().AlignRight().Text($"Payment: {_order.PaymentStatus}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(18);

            column.Item().Element(ComposeCustomerSection);
            column.Item().Element(ComposeAddressSection);
            column.Item().Element(ComposeItemsTable);
            column.Item().AlignRight().Width(260).Element(ComposeTotalsSection);
        });
    }

    private void ComposeCustomerSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(4);
            column.Item().Text("Customer").Bold().FontSize(13);
            column.Item().Text(_order.ContactNameSnapshot ?? string.Empty);
            column.Item().Text(_order.ContactEmailSnapshot ?? string.Empty);
            column.Item().Text(_order.ContactMobileSnapshot ?? string.Empty);
        });
    }

    private void ComposeAddressSection(IContainer container)
    {
        var addressText = _order.Address is null
            ? "No delivery address saved."
            : $"{_order.Address.StreetAddress}, {_order.Address.Barangay}, {_order.Address.City}";

        container.Column(column =>
        {
            column.Spacing(4);
            column.Item().Text("Delivery Address").Bold().FontSize(13);
            column.Item().Text(addressText);

            if (!string.IsNullOrWhiteSpace(_order.Address?.Landmark))
                column.Item().Text($"Landmark: {_order.Address.Landmark}");
        });
    }

    private void ComposeItemsTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.RelativeColumn(2);
                columns.ConstantColumn(50);
                columns.ConstantColumn(90);
                columns.ConstantColumn(95);
            });

            table.Header(header =>
            {
                static IContainer HeaderCell(IContainer c) =>
                    c.Background("#FFF4EC").PaddingVertical(8).PaddingHorizontal(6);

                header.Cell().Element(HeaderCell).Text("Item").SemiBold();
                header.Cell().Element(HeaderCell).Text("Size").SemiBold();
                header.Cell().Element(HeaderCell).AlignCenter().Text("Qty").SemiBold();
                header.Cell().Element(HeaderCell).AlignRight().Text("Unit Price").SemiBold();
                header.Cell().Element(HeaderCell).AlignRight().Text("Line Total").SemiBold();
            });

            foreach (var item in _order.Items.OrderBy(x => x.Id))
            {
                table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(6).Text(item.PackageTitleSnapshot);
                table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(6).Text(item.SizeLabelSnapshot);
                table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(6).AlignCenter().Text(item.Quantity.ToString());
                table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(6).AlignRight().Text($"₱{item.UnitPrice:N2}");
                table.Cell().BorderBottom(1).BorderColor("#EEEEEE").Padding(6).AlignRight().Text($"₱{item.LineTotal:N2}");
            }
        });
    }

    private void ComposeTotalsSection(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(6);

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Subtotal");
                row.ConstantItem(100).AlignRight().Text($"₱{_order.SubTotal:N2}");
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Delivery Fee");
                row.ConstantItem(100).AlignRight().Text($"₱{_order.DeliveryFee:N2}");
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Tax");
                row.ConstantItem(100).AlignRight().Text($"₱{_order.TaxAmount:N2}");
            });

            column.Item().Row(row =>
            {
                row.RelativeItem().Text("Discount");
                row.ConstantItem(100).AlignRight().Text($"₱{_order.DiscountAmount:N2}");
            });

            column.Item().PaddingTop(8).BorderTop(1).BorderColor("#DDDDDD").Row(row =>
            {
                row.RelativeItem().Text("Grand Total").Bold().FontSize(13);
                row.ConstantItem(100).AlignRight().Text($"₱{_order.GrandTotal:N2}").Bold().FontSize(13);
            });
        });
    }
}