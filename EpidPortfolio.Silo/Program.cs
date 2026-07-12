var builder = WebApplication.CreateBuilder(args);

// 이 프로젝트를 Orleans Silo로 실행한다.
// Microsoft Learn: Orleans 서버 구성
// https://learn.microsoft.com/ko-kr/dotnet/orleans/host/configuration-guide/server-configuration
builder.Host.UseOrleans(siloBuilder =>
{
    // 단일 로컬 Silo와 메모리 Grain 저장소를 사용한다.
    // Microsoft Learn: Orleans 로컬 개발 구성
    // https://learn.microsoft.com/ko-kr/dotnet/orleans/host/configuration-guide/local-development-configuration
    siloBuilder
        .UseLocalhostClustering()
        .AddMemoryGrainStorage("mail");
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }));

app.Run();
