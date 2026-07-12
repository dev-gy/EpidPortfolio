using EpidPortfolio.Endpoints.Mail;

var builder = WebApplication.CreateBuilder(args);

// Endpoint에서 Orleans Client를 사용한다.
// Microsoft Learn: Orleans 클라이언트
// https://learn.microsoft.com/ko-kr/dotnet/orleans/host/client
builder.Host.UseOrleansClient(clientBuilder =>
{
    // Endpoint는 로컬 Silo의 Gateway에 연결한다.
    // Microsoft Learn: Orleans 로컬 클라이언트 구성
    // https://learn.microsoft.com/ko-kr/dotnet/orleans/host/configuration-guide/local-development-configuration
    clientBuilder.UseLocalhostClustering();
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }));
app.MapMailEndpoints();

app.Run();
