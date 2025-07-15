var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder.AddKeycloakContainer("keycloak").WithDataVolume();
var realm = keycloak.AddRealm("Test");

var sql = builder
    .AddSqlServer("sql")
    .WithDataVolume()
    .WithEndpoint(name: "mssql", port: 1433, isProxied: false)
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SA_PASSWORD", "YourStrong!Passw0rd");

var api = builder
    .AddProject<Projects.Api>("api")
    .WithReference(keycloak)
    .WithReference(realm)
    .WithReference(sql)
    .WithHttpEndpoint(name: "api", port: 5000);

builder
    .AddNpmApp("frontend", "../React")
    .WithEndpoint(port: 5173, scheme: "http", isProxied: false)
    .WithReference(api);

builder.Build().Run();
