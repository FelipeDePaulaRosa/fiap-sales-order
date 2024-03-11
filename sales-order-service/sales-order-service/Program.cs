using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sales_order_service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbAdvetureWorksContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/sales-orders/{id}", async (int id, [FromServices] DbAdvetureWorksContext context) =>
    {
        var options = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve
        };
        
        var salesOrder = await context.SalesOrderHeaders
            .Include(so => so.SalesOrderDetails)
            .Include(so => so.Customer)
            .ThenInclude(c => c.CustomerAddresses)
            .ThenInclude(ca => ca.Address)
            .FirstOrDefaultAsync(so => so.SalesOrderId == id);
        return JsonSerializer.Serialize(salesOrder, options);;
    })
    .WithName("GetSalesOrder")
    .WithOpenApi();

app.Run();