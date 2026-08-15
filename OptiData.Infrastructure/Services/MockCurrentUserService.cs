using System;
using OptiData.Application.Interfaces;
using OptiData.Infrastructure.Data;

namespace OptiData.Infrastructure.Services
{
    // For this portfolio, we simulate a logged-in user so recruiters do not have to 
    // fill out a login form to test the application.
    public class MockCurrentUserService : ICurrentUserService
    {
        // This is the permanent Test User we created in the DataSeeder
        public Guid UserId => DataSeeder.TestUserId;
    }
}
