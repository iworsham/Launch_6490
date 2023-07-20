using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Tourism.DataAccess;
using Tourism.Models;

using System.Collections.Generic;

namespace Tourism.FeatureTests
{
    public class StateCRUDTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StateCRUDTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async void Index_ReturnsAllStates()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            context.States.Add(new State { Name = "Iowa", Abbreviation = "IA" });
            context.States.Add(new State { Name = "Colorado", Abbreviation = "CO" });
            context.SaveChanges();

            var response = await client.GetAsync("/states");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("IA", html);
            Assert.Contains("Iowa", html);
            Assert.Contains("CO", html);
            Assert.Contains("Colorado", html);
            Assert.DoesNotContain("California", html);
        }

        [Fact]
        public async void Show_ReturnsSingleState()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            context.States.Add(new State { Name = "Iowa", Abbreviation = "IA" });
            context.States.Add(new State { Name = "Colorado", Abbreviation = "CO" });
            context.SaveChanges();

            var response = await client.GetAsync("/states/1");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Contains("Iowa", html);
            Assert.DoesNotContain("Colorado", html);
        }

        private TourismContext GetDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TourismContext>();
            optionsBuilder.UseInMemoryDatabase("TestDatabase");

            var context = new TourismContext(optionsBuilder.Options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }
        [Fact]
        public async Task New_ReturnsForm()
        {
            //Arrange
            var client = _factory.CreateClient();
            var context = GetDbContext();
            State state = new State { Name = "Ohio",Abbreviation = "OH" };
            context.States.Add(state);
            context.SaveChanges();

            //Act
            var response = await client.GetAsync("/states/new");
            var html = await response.Content.ReadAsStringAsync();

            //Assert
           
            Assert.Contains("State:", html);
            Assert.Contains("Abbreviation:", html);
           
        }
        [Fact]
        public async Task  RedirectsToIndex()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();
            State state = new State{Name = "Ohio", Abbreviation = "OH" };
            City city = new City { Name = "Columbus"};
            context.States.Add(state);
            context.Cities.Add(city);
            context.SaveChanges();

            var response = await client.GetAsync($"/states/{state.Id}/cities");
            var html = await response.Content.ReadAsStringAsync();

            response.EnsureSuccessStatusCode();
            //Assert.Contains("New York", html);
            //Assert.Contains("Ohio", html);
            Assert.Contains("Columbus", html);
           
        }
    }
}
