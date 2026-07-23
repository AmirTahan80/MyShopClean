using Data.Repositories.AdminRepositories;
using Domain.Models;
using Infra.Data;
using Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Xunit;

namespace Tests;

public class RepositorySecurityTests
{
    [Fact]
    public async Task CartDetailQuery_IsScopedToTheAuthenticatedUser()
    {
        await using var fixture = await DatabaseFixture.CreateAsync();
        var owner = CreateUser("owner");
        var otherUser = CreateUser("other");
        var product = CreateProduct(10);
        var cart = new Cart
        {
            User = owner,
            UserId = owner.Id,
            CreateTime = DateTime.UtcNow,
            IsFinally = false,
            CartDetails = new List<CartDetail>()
        };
        var detail = new CartDetail
        {
            Cart = cart,
            Product = product,
            ProductCount = 1,
            ProductPrice = product.Price,
            TotalPrice = product.Price
        };
        cart.CartDetails.Add(detail);
        fixture.Context.AddRange(owner, otherUser, product, cart);
        await fixture.Context.SaveChangesAsync();

        var repository = new CartRepository(fixture.Context);

        Assert.NotNull(await repository.GetCartDetailAsync(detail.CartDetailId, owner.Id));
        Assert.Null(await repository.GetCartDetailAsync(detail.CartDetailId, otherUser.Id));
    }

    [Fact]
    public async Task FinalizePayment_IsAtomicAndIdempotent()
    {
        await using var fixture = await DatabaseFixture.CreateAsync();
        var user = CreateUser("buyer");
        var product = CreateProduct(5);
        var cart = new Cart
        {
            User = user,
            UserId = user.Id,
            CreateTime = DateTime.UtcNow,
            IsFinally = false,
            Discounts = new List<Discount>(),
            CartDetails = new List<CartDetail>()
        };
        cart.CartDetails.Add(new CartDetail
        {
            Cart = cart,
            Product = product,
            ProductCount = 2,
            ProductPrice = product.Price,
            TotalPrice = product.Price * 2
        });
        var request = new RequestPay
        {
            Id = Guid.NewGuid().ToString(),
            ApplicationUser = user,
            Cart = cart,
            Amount = product.Price * 2,
            CreateTime = DateTime.UtcNow,
            IsPay = false
        };
        fixture.Context.AddRange(user, product, cart, request);
        await fixture.Context.SaveChangesAsync();

        var repository = new PayRepository(fixture.Context);
        var firstResult = await repository.FinalizePaymentAsync(request.Id, 123456);
        var secondResult = await repository.FinalizePaymentAsync(request.Id, 123456);

        Assert.NotNull(firstResult);
        Assert.NotNull(secondResult);
        Assert.Equal(firstResult.Id, secondResult.Id);
        Assert.Equal(3, (await fixture.Context.Products.SingleAsync()).Count);
        Assert.True((await fixture.Context.RequestPays.SingleAsync()).IsPay);
        Assert.True((await fixture.Context.Carts.SingleAsync()).IsFinally);
        Assert.Equal(1, await fixture.Context.Factors.CountAsync());
    }

    private static ApplicationUser CreateUser(string id) => new()
    {
        Id = id,
        UserName = id,
        NormalizedUserName = id.ToUpperInvariant(),
        Email = $"{id}@example.com",
        NormalizedEmail = $"{id}@example.com".ToUpperInvariant(),
        UserDetail = new UserDetail
        {
            FirstName = id,
            LastName = "Test",
            Address = "Test address"
        }
    };

    private static Product CreateProduct(int stock) => new()
    {
        Name = "Test product",
        Detail = "Test",
        Count = stock,
        Price = 1000,
        InsertTime = DateTime.UtcNow,
        IsProductHaveAttributes = false,
        ProductImages = new List<ProductImages>
        {
            new() { ImgFile = string.Empty, ImgSrc = "test.png" }
        },
        ProductAttributes = new List<ProductAttribute>(),
        AttributeTemplates = new List<AttributeTemplate>()
    };

    private sealed class DatabaseFixture : IAsyncDisposable
    {
        private DatabaseFixture(AppWebContext context)
        {
            Context = context;
        }

        public AppWebContext Context { get; }

        public static async Task<DatabaseFixture> CreateAsync()
        {
            var options = new DbContextOptionsBuilder<AppWebContext>()
                .UseInMemoryDatabase($"myshop-tests-{Guid.NewGuid():N}")
                .ConfigureWarnings(warnings =>
                    warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var context = new AppWebContext(options);
            await context.Database.EnsureCreatedAsync();
            return new DatabaseFixture(context);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
        }
    }
}
