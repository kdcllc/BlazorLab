using BlazingPizza;
using BlazingPizza.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient();
builder.Services.AddDbContext<PizzaStoreContext>(options =>
    options.UseSqlite("Data Source=pizza.db"));

builder.Services.AddScoped<OrderState>();

var app = builder.Build();

// Initialize the database
var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
using (var scope = scopeFactory.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaStoreContext>();
    if (db.Database.EnsureCreated())
    {
        SeedData.Initialize(db);
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapGet("/specials", async (PizzaStoreContext db) =>
{
    return (await db.Specials.ToListAsync()).OrderByDescending(s => s.BasePrice).ToList();
});

app.MapGet("/orders", async (PizzaStoreContext db, CancellationToken cancellationToken) =>
{
    var orders = await db.Orders
    .Include(o => o.Pizzas).ThenInclude(p => p.Special)
    .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
    .OrderByDescending(o => o.CreatedTime)
    .ToListAsync(cancellationToken);

    return orders.Select(o => OrderWithStatus.FromOrder(o)).ToList();
});

app.MapPost("/orders", async (PizzaStoreContext db, Order order, CancellationToken cancellationToken) =>
{
    order.CreatedTime = DateTime.Now;

    // Enforce existence of Pizza.SpecialId and Topping.ToppingId
    // in the database - prevent the submitter from making up
    // new specials and toppings
    foreach (var pizza in order.Pizzas)
    {
        pizza.SpecialId = pizza.Special.Id;
        pizza.Special = null;
    }

    db.Orders.Attach(order);
    await db.SaveChangesAsync(cancellationToken);

    return order.OrderId;
});

app.MapGet("/orders/{orderId:int}", async (PizzaStoreContext db, int orderId, CancellationToken cancellationToken) =>
{
    var order = await db.Orders
                    .Where(o => o.OrderId == orderId)
                    .Include(o => o.Pizzas).ThenInclude(p => p.Special)
                    .Include(o => o.Pizzas).ThenInclude(p => p.Toppings).ThenInclude(t => t.Topping)
                    .SingleOrDefaultAsync(cancellationToken);

    if (order == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(OrderWithStatus.FromOrder(order));
});

app.Run();