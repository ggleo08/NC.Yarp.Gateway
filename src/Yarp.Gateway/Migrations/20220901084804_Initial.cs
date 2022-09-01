using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yarp.Gateway.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YarpClusters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClusterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadBalancingPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpClusters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarpSessionAffinityOptionSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpSessionAffinityOptionSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarpDestinations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Health = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpDestinations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpDestinations_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpHealthCheckOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AvailableDestinationsPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpHealthCheckOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpHealthCheckOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpProxyHttpClientOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SslProtocols = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DangerousAcceptAnyServerCertificate = table.Column<bool>(type: "bit", nullable: true),
                    MaxConnectionsPerServer = table.Column<int>(type: "int", nullable: true),
                    EnableMultipleHttp2Connections = table.Column<bool>(type: "bit", nullable: true),
                    RequestHeaderEncoding = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpProxyHttpClientOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpProxyHttpClientOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpRequestProxyOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActivityTimeout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VersionPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AllowResponseBuffering = table.Column<bool>(type: "bit", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRequestProxyOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRequestProxyOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AuthorizationPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorsPolicy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRoutes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRoutes_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpSessionAffinityOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    Policy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FailurePolicy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AffinityKeyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpSessionAffinityOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpSessionAffinityOptions_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpActiveHealthCheckOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    Interval = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timeout = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Policy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HealthCheckOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpActiveHealthCheckOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpActiveHealthCheckOptions_YarpHealthCheckOptions_HealthCheckOptionId",
                        column: x => x.HealthCheckOptionId,
                        principalTable: "YarpHealthCheckOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpPassiveHealthCheckOptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: true),
                    Policy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReactivationPeriod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HealthCheckOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpPassiveHealthCheckOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpPassiveHealthCheckOptions_YarpHealthCheckOptions_HealthCheckOptionId",
                        column: x => x.HealthCheckOptionId,
                        principalTable: "YarpHealthCheckOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpWebProxyConfig",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BypassOnLocal = table.Column<bool>(type: "bit", nullable: true),
                    UseDefaultCredentials = table.Column<bool>(type: "bit", nullable: true),
                    HttpClientOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpWebProxyConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpWebProxyConfig_YarpProxyHttpClientOptions_HttpClientOptionId",
                        column: x => x.HttpClientOptionId,
                        principalTable: "YarpProxyHttpClientOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Methods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hosts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpMatches_YarpRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "YarpRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpMetadatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ClusterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpMetadatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpClusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "YarpClusters",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpDestinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "YarpDestinations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_YarpMetadatas_YarpRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "YarpRoutes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpTransforms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpTransforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpTransforms_YarpRoutes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "YarpRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpSessionAffinityCookie",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Domain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HttpOnly = table.Column<bool>(type: "bit", nullable: true),
                    SecurePolicy = table.Column<int>(type: "int", nullable: true),
                    SameSite = table.Column<int>(type: "int", nullable: true),
                    Expiration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxAge = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsEssential = table.Column<bool>(type: "bit", nullable: true),
                    SessionAffinityConfigId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpSessionAffinityCookie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpSessionAffinityCookie_YarpSessionAffinityOptions_SessionAffinityConfigId",
                        column: x => x.SessionAffinityConfigId,
                        principalTable: "YarpSessionAffinityOptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "YarpRouteHeaders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "bit", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRouteHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRouteHeaders_YarpMatches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "YarpMatches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YarpRouteQueryParameter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mode = table.Column<int>(type: "int", nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "bit", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarpRouteQueryParameter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YarpRouteQueryParameter_YarpMatches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "YarpMatches",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "YarpClusters",
                columns: new[] { "Id", "ClusterId", "LoadBalancingPolicy" },
                values: new object[] { new Guid("347c00cf-6e72-4f9f-8f8e-2d695a3a2cd6"), "dapr-sidercar", null });

            migrationBuilder.InsertData(
                table: "YarpDestinations",
                columns: new[] { "Id", "Address", "ClusterId", "Health", "Name" },
                values: new object[] { new Guid("a0a0c55a-c3bf-457b-b6cb-9b609fee0fb5"), "http://127.0.0.1:3500", new Guid("347c00cf-6e72-4f9f-8f8e-2d695a3a2cd6"), null, "dapr-sidercar/destination1" });

            migrationBuilder.InsertData(
                table: "YarpRoutes",
                columns: new[] { "Id", "AuthorizationPolicy", "ClusterId", "CorsPolicy", "Order", "RouteId" },
                values: new object[] { new Guid("a4ad8801-0d3c-4737-9043-5b90fb1dac23"), null, new Guid("347c00cf-6e72-4f9f-8f8e-2d695a3a2cd6"), null, null, "second-service" });

            migrationBuilder.InsertData(
                table: "YarpRoutes",
                columns: new[] { "Id", "AuthorizationPolicy", "ClusterId", "CorsPolicy", "Order", "RouteId" },
                values: new object[] { new Guid("cbeaf261-a0e4-42a0-b19b-8cd66da30eaf"), "Default", new Guid("347c00cf-6e72-4f9f-8f8e-2d695a3a2cd6"), null, null, "first-service" });

            migrationBuilder.InsertData(
                table: "YarpMatches",
                columns: new[] { "Id", "Hosts", "Methods", "Path", "RouteId" },
                values: new object[,]
                {
                    { new Guid("a0b45724-958c-42b4-b4fa-538f0645eb6d"), null, null, "/api/second/{**catch-all}", new Guid("a4ad8801-0d3c-4737-9043-5b90fb1dac23") },
                    { new Guid("f2db8dfd-8a12-4c41-bb86-14f8536a07d1"), null, null, "/api/first/{**catch-all}", new Guid("cbeaf261-a0e4-42a0-b19b-8cd66da30eaf") }
                });

            migrationBuilder.InsertData(
                table: "YarpMetadatas",
                columns: new[] { "Id", "ClusterId", "DestinationId", "Key", "RouteId", "Value" },
                values: new object[,]
                {
                    { new Guid("2b6d3ad8-2bf9-4347-934f-9503609fd364"), null, null, "Dapr", new Guid("cbeaf261-a0e4-42a0-b19b-8cd66da30eaf"), "method" },
                    { new Guid("410ea990-d67b-4004-8843-cfcce14b9251"), null, null, "Dapr", new Guid("a4ad8801-0d3c-4737-9043-5b90fb1dac23"), "method" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_YarpActiveHealthCheckOptions_HealthCheckOptionId",
                table: "YarpActiveHealthCheckOptions",
                column: "HealthCheckOptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpDestinations_ClusterId",
                table: "YarpDestinations",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpHealthCheckOptions_ClusterId",
                table: "YarpHealthCheckOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpMatches_RouteId",
                table: "YarpMatches",
                column: "RouteId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_ClusterId",
                table: "YarpMetadatas",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_DestinationId",
                table: "YarpMetadatas",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpMetadatas_RouteId",
                table: "YarpMetadatas",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpPassiveHealthCheckOptions_HealthCheckOptionId",
                table: "YarpPassiveHealthCheckOptions",
                column: "HealthCheckOptionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpProxyHttpClientOptions_ClusterId",
                table: "YarpProxyHttpClientOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpRequestProxyOptions_ClusterId",
                table: "YarpRequestProxyOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpRouteHeaders_MatchId",
                table: "YarpRouteHeaders",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpRouteQueryParameter_MatchId",
                table: "YarpRouteQueryParameter",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpRoutes_ClusterId",
                table: "YarpRoutes",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpSessionAffinityCookie_SessionAffinityConfigId",
                table: "YarpSessionAffinityCookie",
                column: "SessionAffinityConfigId",
                unique: true,
                filter: "[SessionAffinityConfigId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_YarpSessionAffinityOptions_ClusterId",
                table: "YarpSessionAffinityOptions",
                column: "ClusterId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YarpTransforms_RouteId",
                table: "YarpTransforms",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_YarpWebProxyConfig_HttpClientOptionId",
                table: "YarpWebProxyConfig",
                column: "HttpClientOptionId",
                unique: true,
                filter: "[HttpClientOptionId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YarpActiveHealthCheckOptions");

            migrationBuilder.DropTable(
                name: "YarpMetadatas");

            migrationBuilder.DropTable(
                name: "YarpPassiveHealthCheckOptions");

            migrationBuilder.DropTable(
                name: "YarpRequestProxyOptions");

            migrationBuilder.DropTable(
                name: "YarpRouteHeaders");

            migrationBuilder.DropTable(
                name: "YarpRouteQueryParameter");

            migrationBuilder.DropTable(
                name: "YarpSessionAffinityCookie");

            migrationBuilder.DropTable(
                name: "YarpSessionAffinityOptionSettings");

            migrationBuilder.DropTable(
                name: "YarpTransforms");

            migrationBuilder.DropTable(
                name: "YarpWebProxyConfig");

            migrationBuilder.DropTable(
                name: "YarpDestinations");

            migrationBuilder.DropTable(
                name: "YarpHealthCheckOptions");

            migrationBuilder.DropTable(
                name: "YarpMatches");

            migrationBuilder.DropTable(
                name: "YarpSessionAffinityOptions");

            migrationBuilder.DropTable(
                name: "YarpProxyHttpClientOptions");

            migrationBuilder.DropTable(
                name: "YarpRoutes");

            migrationBuilder.DropTable(
                name: "YarpClusters");
        }
    }
}
