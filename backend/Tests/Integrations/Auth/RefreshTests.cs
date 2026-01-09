using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;
using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.Integrations.Auth;

public class RefreshTests : IClassFixture<AuthTestFactory>
{
    private readonly AuthTestFactory _factory;

    public RefreshTests(AuthTestFactory factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task Login_WithValidCredentials_Returns200AndSetsCookie()
    {
        var clientOptions = new WebApplicationFactoryClientOptions { HandleCookies = true };
        var client1 = _factory.CreateClient(clientOptions);
        var client2 = _factory.CreateClient(clientOptions);

        var content1 = await ExecuteLogin(client1, "admin.1.@admin.com", "admin1Password");
        var token1 = GetToken(content1);

        var content2 = await ExecuteLogin(client2, "admin.2.@admin.com", "admin2Password");
        var token2 = GetToken(content2);
        
        var newToken1 = GetToken(await ExecuteRefresh(client1, token1));
        var newToken2 = GetToken(await ExecuteRefresh(client2, token2));
        
        newToken1.Should().NotBe(token1);
        newToken2.Should().NotBe(token2);
        
        await ExecuteLogout(client1);
        
        client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newToken1);
        var failRes = await client1.PostAsync("/api/admin/auth/refresh", null);
        failRes.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        
        var successRes2 = await client2.PostAsync("/api/admin/auth/refresh", null);
        successRes2.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private void AssertAuthCookie(HttpResponseMessage response)
    {
        response.Headers.Should().ContainKey("Set-Cookie");
        var cookieHeader = response.Headers.GetValues("Set-Cookie").First();

        cookieHeader.Should().Contain("refreshToken=");
        cookieHeader.ToLower().Should().Contain("httponly");
    }
    
    private void AssertCookieDeleted(HttpResponseMessage response)
    {
        var cookieHeader = response.Headers.GetValues("Set-Cookie").First();
        cookieHeader.Should().Contain("1970"); 
    }
    
    private async Task<string> ExecuteLogin(HttpClient client, string email, string password)
    {
        var response = await client.PostAsJsonAsync("/api/admin/auth/login", new { Email = email, Password = password });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertAuthCookie(response);
        return await response.Content.ReadAsStringAsync();
    }
    
    private string GetToken(string jsonContent)
    {
        dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
        string token = data.accessToken;
        token.Should().NotBeNullOrEmpty();
        return token;
    }
    
    private async Task<string> ExecuteRefresh(HttpClient client, string oldAccessToken)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oldAccessToken);
    
        var response = await client.PostAsync("/api/admin/auth/refresh", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertAuthCookie(response);
    
        return await response.Content.ReadAsStringAsync();
    }

    private async Task ExecuteLogout(HttpClient client)
    {
        var response = await client.PostAsync("/api/admin/auth/logout", null);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        AssertCookieDeleted(response);
    }
}