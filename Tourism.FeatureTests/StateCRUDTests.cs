﻿using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tourism.DataAccess;
using Tourism.Models;

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
        public async Task Index_ViewHasNewStateLink()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/states");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains($"<a href=\"states/new\">", html);
        }

        [Fact]
        public async Task New_ReturnsForm()
        {
            var context = GetDbContext();
            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/states/new");
            var html = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains($"<form method=\"post\" action=\"/states\"", html);
        }

        [Fact]
        public async Task Add_State_ReturnsRedirectToShow()
        {
            //Arrange
            var client = _factory.CreateClient();
            var context = GetDbContext();

            var addStateFormData = new Dictionary<string, string>
            {
                {"Name", "Ohio" },
                {"Abbreviation", "OH" }
            };
            //Act
            var response = await client.PostAsync($"/states", new FormUrlEncodedContent(addStateFormData));
            var html = await response.Content.ReadAsStringAsync();

            //Assert
            Assert.Contains($"states/show/2", response.RequestMessage.RequestUri.ToString());
            Assert.Contains("Ohio", html);
            Assert.Contains("OH", html);

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
    }
}
